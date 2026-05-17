import { ArticlesService, type ArticleDetailResponse, type ArticlePostEndpointRequest, type ArticlePatchEndpointRequest, ArticleType, ArticleStatus } from '~/api'
import { useApi } from '~/composables/useApi'

export function useArticleManagement() {
  const { executeApi, requireAuth, hasRole } = useApi()

  /**
   * 创建文章
   */
  const createArticle = async (articleData: ArticlePostEndpointRequest): Promise<ArticleDetailResponse | null> => {
    if (!requireAuth()) return null
    if (!hasRole('Publisher')) {
      throw new Error('需要发布者权限才能创建文章')
    }

    const response = await executeApi(
      () => ArticlesService.articlePostEndpoint(articleData),
      { errorMessage: '创建文章失败' }
    )

    return response.data
  }

  /**
   * 更新文章
   */
  const updateArticle = async (id: string, articleData: ArticlePatchEndpointRequest): Promise<ArticleDetailResponse | null> => {
    if (!requireAuth()) return null
    if (!hasRole('Publisher')) {
      throw new Error('需要发布者权限才能更新文章')
    }

    const response = await executeApi(
      () => ArticlesService.articlePatchEndpoint(id, articleData),
      { errorMessage: '更新文章失败' }
    )

    return response.data
  }

  /**
   * 删除文章
   */
  const deleteArticle = async (id: string): Promise<boolean> => {
    if (!requireAuth()) return false
    if (!hasRole('Publisher')) {
      throw new Error('需要发布者权限才能删除文章')
    }

    const response = await executeApi(
      () => ArticlesService.articleDeleteEndpoint(id),
      { errorMessage: '删除文章失败' }
    )

    return !response.error
  }

  /**
   * 获取文章类型的中文名称
   */
  const getArticleTypeName = (type: ArticleType): string => {
    const typeNames: Record<ArticleType, string> = {
      [ArticleType._0]: '故事',
      [ArticleType._1]: '角色介绍',
      [ArticleType._2]: '基因库',
      [ArticleType._3]: '百科',
      [ArticleType._4]: '社区帖子'
    }
    return typeNames[type] || '未知类型'
  }

  /**
   * 获取文章状态的中文名称
   */
  const getArticleStatusName = (status: ArticleStatus): string => {
    const statusNames: Record<ArticleStatus, string> = {
      [ArticleStatus._0]: '草稿',
      [ArticleStatus._1]: '已发布',
      [ArticleStatus._2]: '已禁用',
      [ArticleStatus._3]: '已归档'
    }
    return statusNames[status] || '未知状态'
  }

  /**
   * 获取文章状态的颜色类
   */
  const getArticleStatusColor = (status: ArticleStatus): string => {
    const statusColors: Record<ArticleStatus, string> = {
      [ArticleStatus._0]: 'text-gray-500',
      [ArticleStatus._1]: 'text-green-500',
      [ArticleStatus._2]: 'text-red-500',
      [ArticleStatus._3]: 'text-blue-500'
    }
    return statusColors[status] || 'text-gray-500'
  }

  /**
   * 格式化文章创建时间
   */
  const formatArticleDate = (dateString: string): string => {
    const date = new Date(dateString)
    const now = new Date()
    const diffInHours = (now.getTime() - date.getTime()) / (1000 * 60 * 60)

    if (diffInHours < 1) {
      return '刚刚'
    } else if (diffInHours < 24) {
      return `${Math.floor(diffInHours)}小时前`
    } else if (diffInHours < 24 * 7) {
      return `${Math.floor(diffInHours / 24)}天前`
    } else {
      return date.toLocaleDateString('zh-CN', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      })
    }
  }

  /**
   * 验证文章数据
   */
  const validateArticleData = (data: Partial<ArticlePostEndpointRequest>): string[] => {
    const errors: string[] = []

    if (!data.title?.trim()) {
      errors.push('标题不能为空')
    } else if (data.title.length > 100) {
      errors.push('标题长度不能超过100个字符')
    }

    if (!data.body?.trim()) {
      errors.push('内容不能为空')
    }

    if (!data.tags || data.tags.length === 0) {
      errors.push('至少需要一个标签')
    } else if (data.tags.length > 10) {
      errors.push('标签数量不能超过10个')
    }

    if (data.summary && data.summary.length > 500) {
      errors.push('摘要长度不能超过500个字符')
    }

    return errors
  }

  return {
    createArticle,
    updateArticle,
    deleteArticle,
    getArticleTypeName,
    getArticleStatusName,
    getArticleStatusColor,
    formatArticleDate,
    validateArticleData
  }
}
