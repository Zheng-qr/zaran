import type { Permission } from '~/composables/usePermissions'

export default defineNuxtRouteMiddleware((to, from) => {
  const userStore = useUserStore()
  const { hasPermission } = usePermissions()
  
  // Get required permission from route meta
  const requiredPermission = to.meta.requiresPermission as Permission
  
  if (!requiredPermission) {
    // If no permission specified, allow access
    return
  }
  
  // Check if user is logged in
  if (!userStore.isLoggedIn) {
    const redirectPath = to.fullPath
    return navigateTo({
      path: '/login',
      query: { redirect: redirectPath }
    })
  }
  
  // Check if user has the required permission
  if (!hasPermission(requiredPermission)) {
    throw createError({
      statusCode: 403,
      statusMessage: `您没有权限执行此操作: ${requiredPermission.description}`
    })
  }
})
