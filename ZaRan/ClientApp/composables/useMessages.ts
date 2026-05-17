import { ref, computed, readonly } from 'vue'
import { MessagesService, type MessageDetailResponse, type MessagePostEndpointRequest, MessageType } from '~/api'
import { useApi } from '~/composables/useApi'

export function useMessages() {
  const messages = ref<MessageDetailResponse[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const total = ref(0)
  const unreadCount = ref(0)

  /**
   * 获取消息列表
   */
  const fetchMessages = async (page: number = 1, pageSize: number = 20, type?: MessageType) => {
    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize, true)

      const response = await executeApi(
        () => MessagesService.messageListEndpoint(
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc,
          type,
          undefined // keyword
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        messages.value = []
        total.value = 0
      } else if (response.data) {
        messages.value = response.data.items
        total.value = response.data.total
        // 计算未读消息数量
        unreadCount.value = messages.value.filter(msg => !msg.isRead).length
      }

    } catch (err) {
      error.value = '加载消息失败，请稍后重试'
      console.error('获取消息失败:', err)
      messages.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取单个消息详情
   */
  const getMessageById = async (id: string): Promise<MessageDetailResponse | null> => {
    const { executeApi } = useApi()

    const response = await executeApi(
      () => MessagesService.messageGetEndpoint(id),
      { showError: false }
    )

    if (response.data && !response.data.isRead) {
      // 标记为已读
      await markAsRead(id)
    }

    return response.data
  }

  /**
   * 发送消息
   */
  const sendMessage = async (messageData: MessagePostEndpointRequest): Promise<MessageDetailResponse | null> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return null

    const response = await executeApi(
      () => MessagesService.messagePostEndpoint(messageData),
      { errorMessage: '发送消息失败' }
    )

    if (response.data) {
      // 添加到消息列表
      messages.value.unshift(response.data)
      total.value++
    }

    return response.data
  }

  /**
   * 标记消息为已读
   */
  const markAsRead = async (id: string): Promise<boolean> => {
    const { executeApi } = useApi()

    const response = await executeApi(
      () => MessagesService.messageReadEndpoint(id),
      { showError: false }
    )

    if (!response.error) {
      // 更新本地消息状态
      const message = messages.value.find(msg => msg.id === id)
      if (message && !message.isRead) {
        message.isRead = true
        unreadCount.value = Math.max(0, unreadCount.value - 1)
      }
    }

    return !response.error
  }

  /**
   * 标记所有消息为已读
   */
  const markAllAsRead = async (): Promise<boolean> => {
    const { executeApi } = useApi()

    const response = await executeApi(
      () => MessagesService.messageReadAllEndpoint(),
      { errorMessage: '标记已读失败' }
    )

    if (!response.error) {
      // 更新本地消息状态
      messages.value.forEach(message => {
        message.isRead = true
      })
      unreadCount.value = 0
    }

    return !response.error
  }

  /**
   * 删除消息
   */
  const deleteMessage = async (id: string): Promise<boolean> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return false

    const response = await executeApi(
      () => MessagesService.messageDeleteEndpoint(id),
      { errorMessage: '删除消息失败' }
    )

    if (!response.error) {
      // 从消息列表中移除
      const index = messages.value.findIndex(message => message.id === id)
      if (index !== -1) {
        const deletedMessage = messages.value[index]
        messages.value.splice(index, 1)
        total.value--
        
        // 如果删除的是未读消息，更新未读计数
        if (!deletedMessage.isRead) {
          unreadCount.value = Math.max(0, unreadCount.value - 1)
        }
      }
    }

    return !response.error
  }

  /**
   * 获取消息类型的中文名称
   */
  const getMessageTypeName = (type: MessageType): string => {
    const typeNames: Record<MessageType, string> = {
      [MessageType._0]: '系统通知',
      [MessageType._1]: '订单消息',
      [MessageType._2]: '评论回复',
      [MessageType._3]: '关注通知',
      [MessageType._4]: '私信'
    }
    return typeNames[type] || '未知类型'
  }

  /**
   * 获取消息类型的图标
   */
  const getMessageTypeIcon = (type: MessageType): string => {
    const typeIcons: Record<MessageType, string> = {
      [MessageType._0]: 'fas fa-bell',
      [MessageType._1]: 'fas fa-shopping-cart',
      [MessageType._2]: 'fas fa-comment',
      [MessageType._3]: 'fas fa-user-plus',
      [MessageType._4]: 'fas fa-envelope'
    }
    return typeIcons[type] || 'fas fa-envelope'
  }

  /**
   * 获取消息类型的颜色类
   */
  const getMessageTypeColor = (type: MessageType): string => {
    const typeColors: Record<MessageType, string> = {
      [MessageType._0]: 'text-blue-500',
      [MessageType._1]: 'text-green-500',
      [MessageType._2]: 'text-purple-500',
      [MessageType._3]: 'text-orange-500',
      [MessageType._4]: 'text-pink-500'
    }
    return typeColors[type] || 'text-gray-500'
  }

  /**
   * 格式化消息时间
   */
  const formatMessageTime = (dateString: string): string => {
    const date = new Date(dateString)
    const now = new Date()
    const diffInMinutes = (now.getTime() - date.getTime()) / (1000 * 60)

    if (diffInMinutes < 1) {
      return '刚刚'
    } else if (diffInMinutes < 60) {
      return `${Math.floor(diffInMinutes)}分钟前`
    } else if (diffInMinutes < 24 * 60) {
      return `${Math.floor(diffInMinutes / 60)}小时前`
    } else if (diffInMinutes < 7 * 24 * 60) {
      return `${Math.floor(diffInMinutes / (24 * 60))}天前`
    } else {
      return date.toLocaleDateString('zh-CN', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      })
    }
  }

  /**
   * 验证消息内容
   */
  const validateMessage = (content: string): string[] => {
    const errors: string[] = []

    if (!content?.trim()) {
      errors.push('消息内容不能为空')
    } else if (content.length > 1000) {
      errors.push('消息内容不能超过1000个字符')
    }

    return errors
  }

  // 计算总页数
  const totalPages = computed(() => {
    return Math.ceil(total.value / 20) // 默认每页20条
  })

  return {
    messages: readonly(messages),
    loading: readonly(loading),
    error: readonly(error),
    total: readonly(total),
    unreadCount: readonly(unreadCount),
    totalPages: readonly(totalPages),
    fetchMessages,
    getMessageById,
    sendMessage,
    markAsRead,
    markAllAsRead,
    deleteMessage,
    getMessageTypeName,
    getMessageTypeIcon,
    getMessageTypeColor,
    formatMessageTime,
    validateMessage
  }
}
