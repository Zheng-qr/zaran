<template>
  <div class="redirect-page">
    <!-- 加载状态 -->
    <div class="loading-container">
      <div class="loading-spinner"></div>
      <p>正在跳转到您的个人主页...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
// 页面元数据
useHead({
  title: '我的 - 檐下风铃',
  meta: [
    { name: 'description', content: '个人中心页面' }
  ]
})

// 使用相关的composables
const userStore = useUserStore()

// 自动跳转到用户主页
onMounted(() => {
  // 如果用户已登录，跳转到用户主页
  if (userStore.isLoggedIn && userStore.userId) {
    navigateTo(`/users/${userStore.userId}`)
  } else {
    // 如果用户未登录，跳转到登录页面
    navigateTo('/login')
  }
})

// 监听用户登录状态变化
watch(() => userStore.isLoggedIn, (isLoggedIn) => {
  if (isLoggedIn && userStore.userId) {
    navigateTo(`/users/${userStore.userId}`)
  } else {
    navigateTo('/login')
  }
})
</script>

<style scoped>
.redirect-page {
  min-height: 100vh;
  background-color: #f8fafc;
  display: flex;
  align-items: center;
  justify-content: center;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  text-align: center;
}

.loading-spinner {
  width: 2rem;
  height: 2rem;
  border: 2px solid #f3f4f6;
  border-top: 2px solid #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.loading-container p {
  color: #6b7280;
  font-size: 1.1rem;
}
</style>
