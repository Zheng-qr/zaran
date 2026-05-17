import { ref, readonly } from 'vue'
import {
  ArticlesService,
  CollectionsService,
  type ArticleDetailResponse,
  type CollectionDetailResponse,
  ArticleType,
  CollectionType
} from '~/api'
import { useApi } from '~/composables/useApi'

export function useCommunity() {
  const posts = ref<ArticleDetailResponse[]>([])
  const collections = ref<CollectionDetailResponse[]>([])
  const events = ref<ArticleDetailResponse[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  /**
   * 获取社区帖子 (Post类型的文章)
   */
  const fetchPosts = async (page: number = 1, pageSize: number = 12, keyword: string = '') => {
    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize, true, keyword)

      // 获取 Post 类型的文章 (ArticleType._4 = 4)
      const response = await executeApi(
        () => ArticlesService.articleListEndpoint(
          '4', // Post 类型
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc,
          null, // userId - 不筛选特定用户
          paginationParams.keyword || null
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        posts.value = []
      } else if (response.data) {
        posts.value = response.data.items
      }

    } catch (err) {
      error.value = '加载社区帖子失败，请稍后重试'
      console.error('获取社区帖子失败:', err)
      posts.value = []
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取用户作品集合
   */
  const fetchUserWorks = async (page: number = 1, pageSize: number = 12) => {
    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize, true)

      // 获取用户类型的集合 (CollectionType._2 = 2 表示用户集合)
      const response = await executeApi(
        () => CollectionsService.collectionListEndpoint(
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc,
          '2', // User 类型的集合
          null, // creatorId
          null  // keyword
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        collections.value = []
      } else if (response.data) {
        collections.value = response.data.items
      }

    } catch (err) {
      error.value = '加载用户作品失败，请稍后重试'
      console.error('获取用户作品失败:', err)
      collections.value = []
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取活动公告 (可以使用特定标签的文章)
   */
  const fetchEvents = async (page: number = 1, pageSize: number = 6) => {
    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize, true, '活动')

      // 使用关键词搜索包含"活动"的文章
      const response = await executeApi(
        () => ArticlesService.articleListEndpoint(
          '4', // Post 类型
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc,
          null, // userId - 不筛选特定用户
          '活动' // 搜索活动相关的帖子
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        events.value = []
      } else if (response.data) {
        events.value = response.data.items
      }

    } catch (err) {
      error.value = '加载活动公告失败，请稍后重试'
      console.error('获取活动公告失败:', err)
      events.value = []
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取单个帖子详情
   */
  const getPostById = async (id: string): Promise<ArticleDetailResponse | null> => {
    loading.value = true
    error.value = null

    try {
      const { executeApi } = useApi()

      const response = await executeApi(
        () => ArticlesService.articleGetEndpoint(id),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        return null
      }

      return response.data
    } catch (err) {
      error.value = '加载帖子详情失败'
      console.error('获取帖子详情失败:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取集合详情及其内容
   */
  const getCollectionWithItems = async (id: string) => {
    loading.value = true
    error.value = null

    try {
      const { executeApi } = useApi()

      // 获取集合基本信息
      const collectionResponse = await executeApi(
        () => CollectionsService.collectionGetEndpoint(id),
        { showError: false }
      )

      if (collectionResponse.error) {
        error.value = collectionResponse.error
        return null
      }

      // 获取集合内容
      const itemsResponse = await executeApi(
        () => CollectionsService.collectionItemsEndpoint(id, 0, 20, true),
        { showError: false }
      )

      return {
        collection: collectionResponse.data,
        items: itemsResponse.data?.items || []
      }

    } catch (err) {
      error.value = '加载集合详情失败'
      console.error('获取集合详情失败:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  /**
   * 格式化时间显示
   */
  const formatTime = (timeString?: string): string => {
    if (!timeString) return ''
    
    const date = new Date(timeString)
    const now = new Date()
    const diffMs = now.getTime() - date.getTime()
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))
    
    if (diffDays === 0) {
      return '今天'
    } else if (diffDays === 1) {
      return '昨天'
    } else if (diffDays < 7) {
      return `${diffDays}天前`
    } else {
      return date.toLocaleDateString('zh-CN')
    }
  }

  /**
   * 获取文章摘要
   */
  const getArticleSummary = (article: ArticleDetailResponse, maxLength: number = 100): string => {
    if (article.summary) {
      return article.summary.length > maxLength 
        ? article.summary.substring(0, maxLength) + '...'
        : article.summary
    }
    
    if (article.body) {
      // 移除HTML标签
      const textContent = article.body.replace(/<[^>]*>/g, '')
      return textContent.length > maxLength 
        ? textContent.substring(0, maxLength) + '...'
        : textContent
    }
    
    return '暂无内容'
  }

  return {
    posts: readonly(posts),
    collections: readonly(collections),
    events: readonly(events),
    loading: readonly(loading),
    error: readonly(error),
    fetchPosts,
    fetchUserWorks,
    fetchEvents,
    getPostById,
    getCollectionWithItems,
    formatTime,
    getArticleSummary
  }
}
