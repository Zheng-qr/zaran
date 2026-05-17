import { ref, computed, readonly } from 'vue'
import { CollectionsService, type CollectionDetailResponse, type CollectionPostEndpointRequest, type CollectionPatchEndpointRequest, CollectionType } from '~/api'
import { useApi } from '~/composables/useApi'

export function useCollections(type?: CollectionType, creatorId?: string) {
  const collections = ref<CollectionDetailResponse[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const total = ref(0)

  /**
   * 获取集合列表
   */
  const fetchCollections = async (page: number = 1, pageSize: number = 12, keyword: string = '') => {
    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize, true, keyword)

      const response = await executeApi(
        () => CollectionsService.collectionListEndpoint(
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc,
          type?.toString(),
          creatorId,
          paginationParams.keyword
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        collections.value = []
        total.value = 0
      } else if (response.data) {
        collections.value = response.data.items
        total.value = response.data.total
      }

    } catch (err) {
      error.value = '加载集合失败，请稍后重试'
      console.error('获取集合失败:', err)
      collections.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取单个集合详情
   */
  const getCollectionById = async (id: string): Promise<CollectionDetailResponse | null> => {
    loading.value = true
    error.value = null

    try {
      const { executeApi } = useApi()

      const response = await executeApi(
        () => CollectionsService.collectionGetEndpoint(id),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        return null
      }

      return response.data
    } catch (err) {
      error.value = '加载集合详情失败'
      console.error('获取集合详情失败:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取集合中的项目
   */
  const getCollectionItems = async (id: string, page: number = 1, pageSize: number = 12) => {
    const { executeApi, createPaginationParams } = useApi()
    const paginationParams = createPaginationParams(page, pageSize, true)

    const response = await executeApi(
      () => CollectionsService.collectionItemsEndpoint(
        id,
        paginationParams.offset,
        paginationParams.limit,
        paginationParams.desc
      ),
      { showError: false }
    )

    return response.data
  }

  /**
   * 创建集合
   */
  const createCollection = async (collectionData: CollectionPostEndpointRequest): Promise<CollectionDetailResponse | null> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return null

    const response = await executeApi(
      () => CollectionsService.collectionPostEndpoint(collectionData),
      { errorMessage: '创建集合失败' }
    )

    return response.data
  }

  /**
   * 更新集合
   */
  const updateCollection = async (id: string, collectionData: CollectionPatchEndpointRequest): Promise<CollectionDetailResponse | null> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return null

    const response = await executeApi(
      () => CollectionsService.collectionPatchEndpoint(id, collectionData),
      { errorMessage: '更新集合失败' }
    )

    return response.data
  }

  /**
   * 删除集合
   */
  const deleteCollection = async (id: string): Promise<boolean> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return false

    const response = await executeApi(
      () => CollectionsService.collectionDeleteEndpoint(id),
      { errorMessage: '删除集合失败' }
    )

    return !response.error
  }

  /**
   * 获取集合类型的中文名称
   */
  const getCollectionTypeName = (type: CollectionType): string => {
    const typeNames: Record<CollectionType, string> = {
      [CollectionType._0]: '文章集合',
      [CollectionType._1]: '商品集合',
      [CollectionType._2]: '用户集合'
    }
    return typeNames[type] || '未知类型'
  }

  /**
   * 获取集合类型的图标
   */
  const getCollectionTypeIcon = (type: CollectionType): string => {
    const typeIcons: Record<CollectionType, string> = {
      [CollectionType._0]: 'fas fa-file-alt',
      [CollectionType._1]: 'fas fa-shopping-bag',
      [CollectionType._2]: 'fas fa-users'
    }
    return typeIcons[type] || 'fas fa-folder'
  }

  /**
   * 验证集合数据
   */
  const validateCollectionData = (data: Partial<CollectionPostEndpointRequest>): string[] => {
    const errors: string[] = []

    if (!data.name?.trim()) {
      errors.push('集合名称不能为空')
    } else if (data.name.length > 100) {
      errors.push('集合名称长度不能超过100个字符')
    }

    if (!data.summary?.trim()) {
      errors.push('集合摘要不能为空')
    } else if (data.summary.length > 500) {
      errors.push('集合摘要长度不能超过500个字符')
    }

    if (!data.description?.trim()) {
      errors.push('集合描述不能为空')
    } else if (data.description.length > 2000) {
      errors.push('集合描述长度不能超过2000个字符')
    }

    if (!data.childrenIds || data.childrenIds.length === 0) {
      errors.push('集合至少需要包含一个项目')
    }

    return errors
  }

  // 计算总页数
  const totalPages = computed(() => {
    return Math.ceil(total.value / 12) // 默认每页12条
  })

  return {
    collections: readonly(collections),
    loading: readonly(loading),
    error: readonly(error),
    total: readonly(total),
    totalPages: readonly(totalPages),
    fetchCollections,
    getCollectionById,
    getCollectionItems,
    createCollection,
    updateCollection,
    deleteCollection,
    getCollectionTypeName,
    getCollectionTypeIcon,
    validateCollectionData
  }
}
