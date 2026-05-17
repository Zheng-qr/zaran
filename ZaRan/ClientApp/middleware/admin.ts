export default defineNuxtRouteMiddleware((to, from) => {
  const userStore = useUserStore()
  const { isActiveUser, isPublisher } = usePermissions()

  // Check if user is logged in
  if (!userStore.isLoggedIn) {
    // Save the intended destination for redirect after login
    const redirectPath = to.fullPath
    return navigateTo({
      path: '/login',
      query: { redirect: redirectPath }
    })
  }

  // Check if user has at least publisher role (can create content)
  if (!isPublisher()) {
    throw createError({
      statusCode: 403,
      statusMessage: '您需要发布者权限才能访问管理后台'
    })
  }

  // Check if user account is active
  if (!isActiveUser()) {
    throw createError({
      statusCode: 403,
      statusMessage: '您的账户状态不允许访问管理后台'
    })
  }
})
