import { ref, computed, readonly } from 'vue'
import { CommentsService, type CommentDetailResponse, type CommentPostEndpointRequest, type CommentPatchEndpointRequest } from '~/api'
import { useApi } from '~/composables/useApi'

export function useComments(targetId?: string) {
  const comments = ref<CommentDetailResponse[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const total = ref(0)

  /**
   * 获取评论列表
   */
  const fetchComments = async (page: number = 1, pageSize: number = 20) => {
    if (!targetId) return

    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize, false) // 评论按时间正序

      const response = await executeApi(
        () => CommentsService.commentListEndpoint(
          targetId,
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        comments.value = []
        total.value = 0
      } else if (response.data) {
        comments.value = response.data.items
        total.value = response.data.total
      }

    } catch (err) {
      error.value = '加载评论失败，请稍后重试'
      console.error('获取评论失败:', err)
      comments.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取单个评论详情
   */
  const getCommentById = async (id: string): Promise<CommentDetailResponse | null> => {
    const { executeApi } = useApi()

    const response = await executeApi(
      () => CommentsService.commentGetEndpoint(id),
      { showError: false }
    )

    return response.data
  }

  /**
   * 发表评论
   */
  const createComment = async (content: string, targetId: string): Promise<CommentDetailResponse | null> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return null

    const commentData: CommentPostEndpointRequest = {
      content,
      targetId
    }

    const response = await executeApi(
      () => CommentsService.commentPostEndpoint(commentData),
      { errorMessage: '发表评论失败' }
    )

    if (response.data) {
      // 添加到评论列表
      comments.value.push(response.data)
      total.value++
    }

    return response.data
  }

  /**
   * 更新评论
   */
  const updateComment = async (id: string, content: string): Promise<CommentDetailResponse | null> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return null

    const commentData: CommentPatchEndpointRequest = {
      content
    }

    const response = await executeApi(
      () => CommentsService.commentPatchEndpoint(id, commentData),
      { errorMessage: '更新评论失败' }
    )

    if (response.data) {
      // 更新评论列表中的评论
      const index = comments.value.findIndex(comment => comment.id === id)
      if (index !== -1) {
        comments.value[index] = response.data
      }
    }

    return response.data
  }

  /**
   * 删除评论
   */
  const deleteComment = async (id: string): Promise<boolean> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return false

    const response = await executeApi(
      () => CommentsService.commentDeleteEndpoint(id),
      { errorMessage: '删除评论失败' }
    )

    if (!response.error) {
      // 从评论列表中移除
      const index = comments.value.findIndex(comment => comment.id === id)
      if (index !== -1) {
        comments.value.splice(index, 1)
        total.value--
      }
    }

    return !response.error
  }

  /**
   * 格式化评论时间
   */
  const formatCommentTime = (dateString: string): string => {
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
   * 验证评论内容
   */
  const validateComment = (content: string): string[] => {
    const errors: string[] = []

    if (!content?.trim()) {
      errors.push('评论内容不能为空')
    } else if (content.length > 1000) {
      errors.push('评论内容不能超过1000个字符')
    }

    return errors
  }

  // 计算总页数
  const totalPages = computed(() => {
    return Math.ceil(total.value / 20) // 默认每页20条
  })

  return {
    comments: readonly(comments),
    loading: readonly(loading),
    error: readonly(error),
    total: readonly(total),
    totalPages: readonly(totalPages),
    fetchComments,
    getCommentById,
    createComment,
    updateComment,
    deleteComment,
    formatCommentTime,
    validateComment
  }
}
