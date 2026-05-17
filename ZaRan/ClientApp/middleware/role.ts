export default defineNuxtRouteMiddleware((to) => {
  const userStore = useUserStore()
  const { hasRole, getRoleName } = usePermissions()

  // 从路由元数据中获取所需的角色 (支持字符串和数字)
  const requiredRole = to.meta.requiresRole as string | number

  if (requiredRole === undefined) {
    // 如果没有指定角色要求，允许访问
    return
  }

  // 检查用户是否已登录
  if (!userStore.isLoggedIn) {
    const redirectPath = to.fullPath
    return navigateTo({
      path: '/login',
      query: { redirect: redirectPath }
    })
  }

  // 转换角色为数字进行比较
  let requiredRoleValue: number
  if (typeof requiredRole === 'string') {
    const roleMap: Record<string, number> = {
      'Guest': 0,
      'User': 1,
      'Publisher': 2,
      'Authorized': 3,
      'Admin': 4
    }
    requiredRoleValue = roleMap[requiredRole] ?? 0
  } else {
    requiredRoleValue = requiredRole
  }

  // 检查用户是否有足够的权限
  if (!hasRole(requiredRoleValue)) {
    const requiredRoleName = getRoleName(requiredRoleValue)
    throw createError({
      statusCode: 403,
      statusMessage: `需要 ${requiredRoleName} 或更高权限才能访问此页面`
    })
  }
})
