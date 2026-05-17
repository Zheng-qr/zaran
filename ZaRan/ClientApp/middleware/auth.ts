export default defineNuxtRouteMiddleware((to, from) => {
  const userStore = useUserStore()
  
  // 检查用户是否已登录
  if (!userStore.isLoggedIn) {
    // 保存用户想要访问的页面，登录后可以重定向回来
    const redirectPath = to.fullPath
    
    // 重定向到登录页面，并传递重定向参数
    return navigateTo({
      path: '/login',
      query: { redirect: redirectPath }
    })
  }
})
