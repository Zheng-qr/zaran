import { ref, readonly } from 'vue'
import { ApiError } from '~/api/generated/core/ApiError'

export interface ApiResponse<T> {
  data: T | null
  error: string | null
  loading: boolean
}

export interface PaginationParams {
  offset: number
  limit: number
  desc?: boolean
  keyword?: string
}

export interface PaginatedResponse<T> {
  total: number
  items: T[]
}

/**
 * Composable for handling API operations with consistent error handling and loading states
 */
export function useApi() {
  // 延迟获取 userStore 以避免循环依赖
  const getUserStore = () => {
    return useUserStore()
  }

  /**
   * Handle API errors consistently
   */
  const handleApiError = (error: unknown): string => {
    console.error('API Error:', error)

    if (error instanceof ApiError) {
      // Handle specific HTTP status codes
      switch (error.status) {
        case 401: {
          // Unauthorized - clear user session and redirect to login
          const userStore = getUserStore()
          userStore.logout()
          navigateTo('/login')
          return '登录已过期，请重新登录'
        }
        case 403:
          return '没有权限执行此操作'
        case 404:
          return '请求的资源不存在'
        case 400:
          return error.body || '请求参数错误'
        case 500:
          return '服务器内部错误，请稍后重试'
        default:
          return error.body || `请求失败 (${error.status})`
      }
    }

    if (error.message) {
      return error.message
    }

    return '网络错误，请检查网络连接'
  }

  /**
   * Execute API call with error handling and loading state
   */
  const executeApi = async <T>(
    apiCall: () => Promise<T>,
    options: {
      showError?: boolean
      errorMessage?: string
      showSuccess?: boolean
      successMessage?: string
    } = {}
  ): Promise<ApiResponse<T>> => {
    const { showError = true, errorMessage, showSuccess = false, successMessage } = options

    const response: ApiResponse<T> = {
      data: null,
      error: null,
      loading: true
    }

    try {
      response.data = await apiCall()
      response.error = null

      if (showSuccess && successMessage) {
        const { success } = useGlobalNotifications()
        success(successMessage)
      }
    } catch (error) {
      const errorMsg = errorMessage || handleApiError(error)
      response.error = errorMsg

      if (showError) {
        const { error: showErrorNotification } = useGlobalNotifications()
        showErrorNotification('操作失败', errorMsg)
      }
    } finally {
      response.loading = false
    }

    return response
  }

  /**
   * Create pagination parameters
   */
  const createPaginationParams = (
    page: number = 1,
    pageSize: number = 12,
    desc: boolean = true,
    keyword?: string
  ): PaginationParams => {
    // Ensure page and pageSize are valid numbers
    const validPage = Math.max(1, Math.floor(Number(page)) || 1)
    const validPageSize = Math.max(1, Math.floor(Number(pageSize)) || 12)

    return {
      offset: (validPage - 1) * validPageSize,
      limit: validPageSize,
      desc: Boolean(desc),
      keyword: keyword?.trim() || undefined
    }
  }

  /**
   * Check if user is authenticated
   */
  const requireAuth = (): boolean => {
    const userStore = getUserStore()
    if (!userStore.isLoggedIn) {
      navigateTo('/login')
      return false
    }
    return true
  }

  /**
   * Check if user has specific role
   */
  const hasRole = (requiredRole: string | number): boolean => {
    const userStore = getUserStore()
    if (!userStore.user) return false

    const userRole = userStore.user.userRole

    // Convert role names to numeric values for comparison
    const roleMap: Record<string, number> = {
      'Guest': 0,
      'User': 1,
      'Publisher': 2,
      'Authorized': 3,
      'Admin': 4
    }

    const userRoleValue = typeof userRole === 'number' ? userRole : roleMap[userRole] || 0
    const requiredRoleValue = typeof requiredRole === 'number' ? requiredRole : roleMap[requiredRole] || 0

    return userRoleValue >= requiredRoleValue
  }

  /**
   * Upload file to static endpoint
   */
  const uploadFile = async (file: File): Promise<ApiResponse<string>> => {
    const formData = new FormData()
    formData.append('file', file)

    return executeApi(async () => {
      const userStore = getUserStore()
      const response = await $fetch<{ fileUrl: string }>('/api/static/upload', {
        method: 'POST',
        body: formData,
        headers: {
          'Authorization': `Bearer ${userStore.getToken()}`
        }
      })
      return response.fileUrl
    }, {
      errorMessage: '文件上传失败'
    })
  }

  return {
    executeApi,
    handleApiError,
    createPaginationParams,
    requireAuth,
    hasRole,
    uploadFile
  }
}

/**
 * Reactive API state composable
 */
export function useApiState<T>(initialData: T | null = null) {
  const data = ref<T | null>(initialData)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const execute = async (apiCall: () => Promise<T>) => {
    loading.value = true
    error.value = null

    try {
      data.value = await apiCall()
    } catch (err) {
      const { handleApiError } = useApi()
      error.value = handleApiError(err)
    } finally {
      loading.value = false
    }
  }

  const reset = () => {
    data.value = initialData
    loading.value = false
    error.value = null
  }

  return {
    data: readonly(data),
    loading: readonly(loading),
    error: readonly(error),
    execute,
    reset
  }
}
