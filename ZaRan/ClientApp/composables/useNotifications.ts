export interface Notification {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message?: string
  duration?: number
  persistent?: boolean
}

export function useNotifications() {
  const notifications = ref<Notification[]>([])

  /**
   * 添加通知
   */
  const addNotification = (notification: Omit<Notification, 'id'>): string => {
    const id = Date.now().toString() + Math.random().toString(36).substr(2, 9)
    const newNotification: Notification = {
      id,
      duration: 5000, // 默认5秒
      persistent: false,
      ...notification
    }

    notifications.value.push(newNotification)

    // 自动移除非持久化通知
    if (!newNotification.persistent && newNotification.duration && newNotification.duration > 0) {
      setTimeout(() => {
        removeNotification(id)
      }, newNotification.duration)
    }

    return id
  }

  /**
   * 移除通知
   */
  const removeNotification = (id: string) => {
    const index = notifications.value.findIndex(n => n.id === id)
    if (index > -1) {
      notifications.value.splice(index, 1)
    }
  }

  /**
   * 清空所有通知
   */
  const clearNotifications = () => {
    notifications.value = []
  }

  /**
   * 显示成功通知
   */
  const success = (title: string, message?: string, duration?: number) => {
    return addNotification({
      type: 'success',
      title,
      message,
      duration
    })
  }

  /**
   * 显示错误通知
   */
  const error = (title: string, message?: string, persistent: boolean = false) => {
    return addNotification({
      type: 'error',
      title,
      message,
      persistent,
      duration: persistent ? 0 : 8000 // 错误通知显示更久
    })
  }

  /**
   * 显示警告通知
   */
  const warning = (title: string, message?: string, duration?: number) => {
    return addNotification({
      type: 'warning',
      title,
      message,
      duration
    })
  }

  /**
   * 显示信息通知
   */
  const info = (title: string, message?: string, duration?: number) => {
    return addNotification({
      type: 'info',
      title,
      message,
      duration
    })
  }

  /**
   * 获取通知图标
   */
  const getNotificationIcon = (type: Notification['type']): string => {
    const icons = {
      success: 'fas fa-check-circle',
      error: 'fas fa-exclamation-circle',
      warning: 'fas fa-exclamation-triangle',
      info: 'fas fa-info-circle'
    }
    return icons[type]
  }

  /**
   * 获取通知颜色类
   */
  const getNotificationColor = (type: Notification['type']): string => {
    const colors = {
      success: 'bg-green-500',
      error: 'bg-red-500',
      warning: 'bg-yellow-500',
      info: 'bg-blue-500'
    }
    return colors[type]
  }

  return {
    notifications: readonly(notifications),
    addNotification,
    removeNotification,
    clearNotifications,
    success,
    error,
    warning,
    info,
    getNotificationIcon,
    getNotificationColor
  }
}

// 全局通知实例
let globalNotifications: ReturnType<typeof useNotifications> | null = null

/**
 * 获取全局通知实例
 */
export function useGlobalNotifications() {
  if (!globalNotifications) {
    globalNotifications = useNotifications()
  }
  return globalNotifications
}
