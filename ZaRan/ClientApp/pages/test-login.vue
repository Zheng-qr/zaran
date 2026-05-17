<template>
  <div class="test-login-page">
    <div class="container mx-auto px-4 py-8">
      <h1 class="text-3xl font-bold mb-8">登录状态测试页面</h1>
      
      <!-- 当前登录状态显示 -->
      <div class="bg-gray-100 p-6 rounded-lg mb-8">
        <h2 class="text-xl font-semibold mb-4">当前登录状态</h2>
        <div class="space-y-2">
          <p><strong>登录状态:</strong> 
            <span :class="userStore.isLoggedIn ? 'text-green-600' : 'text-red-600'">
              {{ userStore.isLoggedIn ? '已登录' : '未登录' }}
            </span>
          </p>
          <p><strong>用户信息:</strong> {{ userStore.user ? userStore.user.nickname : '无' }}</p>
          <p><strong>Token存在:</strong> {{ !!userStore.getToken() ? '是' : '否' }}</p>
          <p><strong>用户ID:</strong> {{ userStore.userId || '无' }}</p>
        </div>
      </div>

      <!-- 快速登录测试 -->
      <div class="bg-blue-50 p-6 rounded-lg mb-8">
        <h2 class="text-xl font-semibold mb-4">快速登录测试</h2>
        <div class="space-y-4">
          <button 
            @click="quickLogin('admin', 'admin123')"
            :disabled="userStore.loading"
            class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 disabled:opacity-50"
          >
            {{ userStore.loading ? '登录中...' : '管理员登录 (admin/admin123)' }}
          </button>
          
          <button 
            @click="quickLogin('zhaoshewen', 'password123')"
            :disabled="userStore.loading"
            class="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 disabled:opacity-50 ml-4"
          >
            {{ userStore.loading ? '登录中...' : '普通用户登录 (zhaoshewen/password123)' }}
          </button>
          
          <button 
            @click="logout"
            :disabled="userStore.loading || !userStore.isLoggedIn"
            class="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600 disabled:opacity-50 ml-4"
          >
            退出登录
          </button>
        </div>
      </div>

      <!-- 导航栏状态预览 -->
      <div class="bg-yellow-50 p-6 rounded-lg mb-8">
        <h2 class="text-xl font-semibold mb-4">导航栏按钮状态预览</h2>
        <div class="border p-4 rounded bg-white">
          <div class="flex items-center space-x-4">
            <template v-if="!userStore.isLoggedIn">
              <button class="bg-blue-500 text-white px-3 py-1 rounded text-sm">登录</button>
              <button class="bg-green-500 text-white px-3 py-1 rounded text-sm">注册</button>
            </template>
            <template v-else>
              <span class="text-gray-700">{{ userStore.user?.nickname || '用户' }}</span>
              <button class="bg-red-500 text-white px-3 py-1 rounded text-sm">退出</button>
            </template>
          </div>
        </div>
      </div>

      <!-- 实时状态监控 -->
      <div class="bg-green-50 p-6 rounded-lg">
        <h2 class="text-xl font-semibold mb-4">实时状态监控</h2>
        <div class="space-y-2 text-sm">
          <p><strong>localStorage auth_token:</strong> {{ localStorageToken || '无' }}</p>
          <p><strong>响应式 authToken:</strong> {{ reactiveToken || '无' }}</p>
          <p><strong>状态同步:</strong> 
            <span :class="isStateSynced ? 'text-green-600' : 'text-red-600'">
              {{ isStateSynced ? '同步' : '不同步' }}
            </span>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// 页面元数据
useHead({
  title: '登录状态测试 - 檐下风铃',
  meta: [
    { name: 'description', content: '测试登录状态更新功能' }
  ]
})

const userStore = useUserStore()

// 实时监控状态
const localStorageToken = ref('')
const reactiveToken = ref('')

// 更新监控状态
const updateMonitoringState = () => {
  if (process.client) {
    localStorageToken.value = localStorage.getItem('auth_token') || ''
  }
  reactiveToken.value = userStore.getToken() || ''
}

// 检查状态是否同步
const isStateSynced = computed(() => {
  return localStorageToken.value === reactiveToken.value
})

// 快速登录函数
const quickLogin = async (username: string, password: string) => {
  try {
    const success = await userStore.login({
      account: username,
      password: password
    })
    
    if (success) {
      console.log('登录成功，状态应该已更新')
      updateMonitoringState()
    } else {
      console.error('登录失败:', userStore.error)
    }
  } catch (error) {
    console.error('登录过程中发生错误:', error)
  }
}

// 退出登录
const logout = async () => {
  await userStore.logout()
  updateMonitoringState()
}

// 定期更新监控状态
onMounted(() => {
  updateMonitoringState()
  
  // 每秒更新一次监控状态
  const interval = setInterval(updateMonitoringState, 1000)
  
  onUnmounted(() => {
    clearInterval(interval)
  })
})
</script>

<style scoped>
.test-login-page {
  min-height: 100vh;
  background-color: #f9fafb;
}
</style>
