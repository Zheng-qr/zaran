import type { UserRole, UserStatus } from '~/api'

// Role hierarchy mapping
export const ROLES = {
  GUEST: 0,
  USER: 1,
  PUBLISHER: 2,
  AUTHORIZED: 3,
  ADMIN: 4
} as const

export const ROLE_NAMES = {
  [ROLES.GUEST]: 'Guest',
  [ROLES.USER]: 'User',
  [ROLES.PUBLISHER]: 'Publisher',
  [ROLES.AUTHORIZED]: 'Authorized',
  [ROLES.ADMIN]: 'Admin'
} as const

// User status mapping
export const USER_STATUS = {
  UNVERIFIED: 0,
  NORMAL: 1
} as const

export const STATUS_NAMES = {
  [USER_STATUS.UNVERIFIED]: 'Unverified',
  [USER_STATUS.NORMAL]: 'Normal'
} as const

// Permission definitions
export interface Permission {
  name: string
  description: string
  requiredRole: number
  requiredStatus?: number[]
}

export const PERMISSIONS = {
  // User management permissions
  VIEW_USERS: {
    name: 'view_users',
    description: 'View user list and details',
    requiredRole: ROLES.ADMIN
  },
  MANAGE_USERS: {
    name: 'manage_users',
    description: 'Create, update, and delete users',
    requiredRole: ROLES.ADMIN
  },
  MANAGE_USER_ROLES: {
    name: 'manage_user_roles',
    description: 'Change user roles',
    requiredRole: ROLES.ADMIN
  },
  MANAGE_USER_STATUS: {
    name: 'manage_user_status',
    description: 'Ban/unban users',
    requiredRole: ROLES.ADMIN
  },
  RESET_USER_PASSWORD: {
    name: 'reset_user_password',
    description: 'Reset user passwords',
    requiredRole: ROLES.ADMIN
  },

  // Content management permissions
  VIEW_ALL_ARTICLES: {
    name: 'view_all_articles',
    description: 'View all articles including drafts',
    requiredRole: ROLES.ADMIN
  },
  MANAGE_ALL_ARTICLES: {
    name: 'manage_all_articles',
    description: 'Edit and delete any article',
    requiredRole: ROLES.ADMIN
  },
  MODERATE_ARTICLES: {
    name: 'moderate_articles',
    description: 'Approve, reject, or ban articles',
    requiredRole: ROLES.ADMIN
  },
  CREATE_ARTICLES: {
    name: 'create_articles',
    description: 'Create new articles',
    requiredRole: ROLES.PUBLISHER,
    requiredStatus: [USER_STATUS.NORMAL]
  },
  EDIT_OWN_ARTICLES: {
    name: 'edit_own_articles',
    description: 'Edit own articles',
    requiredRole: ROLES.PUBLISHER,
    requiredStatus: [USER_STATUS.NORMAL]
  },

  // Commerce management permissions
  VIEW_ALL_GOODS: {
    name: 'view_all_goods',
    description: 'View all goods including drafts',
    requiredRole: ROLES.ADMIN
  },
  MANAGE_ALL_GOODS: {
    name: 'manage_all_goods',
    description: 'Edit and delete any good',
    requiredRole: ROLES.ADMIN
  },
  MODERATE_GOODS: {
    name: 'moderate_goods',
    description: 'Approve, reject, or ban goods',
    requiredRole: ROLES.ADMIN
  },
  CREATE_GOODS: {
    name: 'create_goods',
    description: 'Create new goods',
    requiredRole: ROLES.PUBLISHER,
    requiredStatus: [USER_STATUS.NORMAL]
  },
  EDIT_OWN_GOODS: {
    name: 'edit_own_goods',
    description: 'Edit own goods',
    requiredRole: ROLES.PUBLISHER,
    requiredStatus: [USER_STATUS.NORMAL]
  },

  // Transaction management permissions
  VIEW_ALL_TRANSACTIONS: {
    name: 'view_all_transactions',
    description: 'View all transactions',
    requiredRole: ROLES.ADMIN
  },
  MANAGE_TRANSACTIONS: {
    name: 'manage_transactions',
    description: 'Manage transaction status',
    requiredRole: ROLES.ADMIN
  },

  // Collection management permissions
  VIEW_ALL_COLLECTIONS: {
    name: 'view_all_collections',
    description: 'View all collections',
    requiredRole: ROLES.ADMIN
  },
  MANAGE_ALL_COLLECTIONS: {
    name: 'manage_all_collections',
    description: 'Edit and delete any collection',
    requiredRole: ROLES.ADMIN
  },
  CREATE_COLLECTIONS: {
    name: 'create_collections',
    description: 'Create new collections',
    requiredRole: ROLES.USER,
    requiredStatus: [USER_STATUS.NORMAL]
  },

  // Communication management permissions
  VIEW_ALL_MESSAGES: {
    name: 'view_all_messages',
    description: 'View all messages',
    requiredRole: ROLES.ADMIN
  },
  MODERATE_COMMENTS: {
    name: 'moderate_comments',
    description: 'Moderate comments',
    requiredRole: ROLES.ADMIN
  },

  // System management permissions
  VIEW_SYSTEM_STATS: {
    name: 'view_system_stats',
    description: 'View system statistics',
    requiredRole: ROLES.ADMIN
  },
  MANAGE_SYSTEM: {
    name: 'manage_system',
    description: 'System administration',
    requiredRole: ROLES.ADMIN
  },

  // File management permissions
  UPLOAD_FILES: {
    name: 'upload_files',
    description: 'Upload files',
    requiredRole: ROLES.USER,
    requiredStatus: [USER_STATUS.NORMAL]
  },
  MANAGE_ALL_FILES: {
    name: 'manage_all_files',
    description: 'Manage all uploaded files',
    requiredRole: ROLES.ADMIN
  }
} as const

/**
 * Composable for handling permissions and role-based access control
 */
export function usePermissions() {
  const userStore = useUserStore()

  /**
   * Get user's role as number
   */
  const getUserRole = (): number => {
    if (!userStore.user) return ROLES.GUEST
    return typeof userStore.user.userRole === 'number' 
      ? userStore.user.userRole 
      : ROLES.GUEST
  }

  /**
   * Get user's status as number
   */
  const getUserStatus = (): number => {
    if (!userStore.user) return USER_STATUS.UNVERIFIED
    return typeof userStore.user.userStatus === 'number' 
      ? userStore.user.userStatus 
      : USER_STATUS.UNVERIFIED
  }

  /**
   * Check if user has specific role or higher
   */
  const hasRole = (requiredRole: number): boolean => {
    const userRole = getUserRole()
    return userRole >= requiredRole
  }

  /**
   * Check if user has required status
   */
  const hasStatus = (requiredStatuses: number[]): boolean => {
    const userStatus = getUserStatus()
    return requiredStatuses.includes(userStatus)
  }

  /**
   * Check if user has specific permission
   */
  const hasPermission = (permission: Permission): boolean => {
    // Check role requirement
    if (!hasRole(permission.requiredRole)) {
      return false
    }

    // Check status requirement if specified
    if (permission.requiredStatus && !hasStatus(permission.requiredStatus)) {
      return false
    }

    return true
  }

  /**
   * Check if user can access admin dashboard
   */
  const canAccessAdmin = (): boolean => {
    return hasRole(ROLES.ADMIN)
  }

  /**
   * Check if user can publish content
   */
  const canPublish = (): boolean => {
    return hasRole(ROLES.PUBLISHER) && isActiveUser()
  }

  /**
   * Check if user is admin
   */
  const isAdmin = (): boolean => {
    return hasRole(ROLES.ADMIN)
  }

  /**
   * Check if user is publisher or higher
   */
  const isPublisher = (): boolean => {
    return hasRole(ROLES.PUBLISHER)
  }

  /**
   * Check if user is authorized or higher
   */
  const isAuthorized = (): boolean => {
    return hasRole(ROLES.AUTHORIZED)
  }

  /**
   * Check if user is currently banned (based on unban time)
   */
  const isBanned = (): boolean => {
    if (!userStore.user?.unbanTime) return false
    const unbanTime = new Date(userStore.user.unbanTime)
    const now = new Date()
    return unbanTime > now
  }

  /**
   * Check if user account is active (not banned and has normal status)
   */
  const isActiveUser = (): boolean => {
    return hasStatus([USER_STATUS.NORMAL]) && !isBanned()
  }

  /**
   * Get role display name
   */
  const getRoleName = (role?: number): string => {
    const userRole = role ?? getUserRole()
    return ROLE_NAMES[userRole as keyof typeof ROLE_NAMES] || 'Unknown'
  }

  /**
   * Get status display name
   */
  const getStatusName = (status?: number): string => {
    const userStatus = status ?? getUserStatus()
    return STATUS_NAMES[userStatus as keyof typeof STATUS_NAMES] || 'Unknown'
  }

  /**
   * Get all available roles for selection
   */
  const getAllRoles = () => {
    return Object.entries(ROLE_NAMES).map(([value, name]) => ({
      value: parseInt(value),
      name
    }))
  }

  /**
   * Get all available statuses for selection
   */
  const getAllStatuses = () => {
    return Object.entries(STATUS_NAMES).map(([value, name]) => ({
      value: parseInt(value),
      name
    }))
  }

  return {
    // Constants
    ROLES,
    ROLE_NAMES,
    USER_STATUS,
    STATUS_NAMES,
    PERMISSIONS,

    // User info
    getUserRole,
    getUserStatus,
    getRoleName,
    getStatusName,
    getAllRoles,
    getAllStatuses,

    // Permission checks
    hasRole,
    hasStatus,
    hasPermission,

    // Convenience checks
    canAccessAdmin,
    canPublish,
    isAdmin,
    isPublisher,
    isAuthorized,
    isActiveUser,
    isBanned
  }
}
