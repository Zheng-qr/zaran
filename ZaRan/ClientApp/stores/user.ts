import type { UserDetailResponse, UserLoginRequest, UserRegisterRequest, UserLoginResponse } from '~/api'
import { UserService } from '~/api'

export const useUserStore = defineStore('user', () => {
  const user = ref<UserDetailResponse | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // 使用响应式的方式管理token和登录状态
  const authToken = ref<string | null>(null)
  const userIdRef = ref<string | null>(null)

  // 初始化时从localStorage读取
  if (process.client) {
    authToken.value = localStorage.getItem('auth_token')
    userIdRef.value = localStorage.getItem('user_id')
  }

  const isLoggedIn = computed(() => {
    return !!authToken.value
  })

  const userId = computed(() => {
    return userIdRef.value
  })

  async function fetchUserInfo() {
    if (!isLoggedIn.value) return

    loading.value = true
    error.value = null

    try {
      const response = await UserService.userStatusEndpoint()
      user.value = response
    } catch (err) {
      console.error('获取用户信息失败:', err)
      // 如果获取用户信息失败，可能是token失效，清除登录状态
      logout()
    } finally {
      loading.value = false
    }
  }

  async function login(loginData: UserLoginRequest): Promise<boolean> {
    loading.value = true
    error.value = null

    try {
      const response: UserLoginResponse = await UserService.userLoginEndpoint(loginData)

      if (response.token) {
        // 保存token和用户信息
        setToken(response.token, response.id)
        user.value = response

        // 显示成功通知
        const { success } = useGlobalNotifications()
        success('登录成功', `欢迎回来，${response.nickname}！`)

        return true
      } else {
        error.value = '登录失败：未收到有效的认证令牌'
        return false
      }
    } catch (err) {
      const { handleApiError } = useApi()
      error.value = handleApiError(err)
      return false
    } finally {
      loading.value = false
    }
  }

  async function register(registerData: UserRegisterRequest): Promise<boolean> {
    loading.value = true
    error.value = null

    try {
      const response = await UserService.userRegisterEndpoint(registerData)
      user.value = response

      // 显示注册成功通知
      const { success } = useGlobalNotifications()
      success('注册成功', '账户创建成功，正在为您自动登录...')

      // 注册成功后自动登录
      const loginSuccess = await login({
        account: registerData.username,
        password: registerData.password
      })

      return loginSuccess
    } catch (err) {
      const { handleApiError } = useApi()
      error.value = handleApiError(err)
      return false
    } finally {
      loading.value = false
    }
  }

  function setUser(userData: UserDetailResponse) {
    user.value = userData
  }

  function clearUser() {
    user.value = null
    error.value = null
  }

  async function logout() {
    if (process.client) {
      localStorage.removeItem('auth_token')
      localStorage.removeItem('user_id')
    }
    // 同时更新响应式变量
    authToken.value = null
    userIdRef.value = null
    clearUser()

    // 可选：通知服务器用户已登出
    // 这里可以调用登出API如果后端有提供
  }

  function setToken(token: string, userId: string) {
    if (process.client) {
      localStorage.setItem('auth_token', token)
      localStorage.setItem('user_id', userId)
    }
    // 同时更新响应式变量
    authToken.value = token
    userIdRef.value = userId
  }

  function getToken() {
    return authToken.value
  }

  // 初始化时尝试获取用户信息
  if (process.client && isLoggedIn.value) {
    fetchUserInfo()
  }

  // 监听localStorage变化（用于多标签页同步）
  if (process.client) {
    window.addEventListener('storage', (e) => {
      if (e.key === 'auth_token') {
        authToken.value = e.newValue
      }
      if (e.key === 'user_id') {
        userIdRef.value = e.newValue
      }
    })
  }

  return {
    user: readonly(user),
    loading: readonly(loading),
    error: readonly(error),
    isLoggedIn,
    userId,
    fetchUserInfo,
    login,
    register,
    setUser,
    clearUser,
    logout,
    setToken,
    getToken
  }
})
