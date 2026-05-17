import { ref, computed, readonly } from 'vue'
import { GoodsService, type GoodDetailResponse, type GoodPostEndpointRequest, type GoodPatchEndpointRequest, GoodStatus } from '~/api'
import { useApi } from '~/composables/useApi'

export function useGoods() {
  const goods = ref<GoodDetailResponse[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const total = ref(0)

  /**
   * 获取商品列表
   */
  const fetchGoods = async (page: number = 1, pageSize: number = 12, keyword: string = '') => {
    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize, true, keyword)

      const response = await executeApi(
        () => GoodsService.goodListEndpoint(
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc,
          paginationParams.keyword
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        goods.value = []
        total.value = 0
      } else if (response.data) {
        goods.value = response.data.items
        total.value = response.data.total
      }

    } catch (err) {
      error.value = '加载商品失败，请稍后重试'
      console.error('获取商品失败:', err)
      goods.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取单个商品详情
   */
  const getGoodById = async (id: string): Promise<GoodDetailResponse | null> => {
    loading.value = true
    error.value = null

    try {
      const { executeApi } = useApi()

      const response = await executeApi(
        () => GoodsService.goodGetEndpoint(id),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        return null
      }

      return response.data
    } catch (err) {
      error.value = '加载商品详情失败'
      console.error('获取商品详情失败:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  /**
   * 创建商品
   */
  const createGood = async (goodData: GoodPostEndpointRequest): Promise<GoodDetailResponse | null> => {
    const { executeApi, requireAuth, hasRole } = useApi()
    
    if (!requireAuth()) return null
    if (!hasRole('Publisher')) {
      throw new Error('需要发布者权限才能创建商品')
    }

    const response = await executeApi(
      () => GoodsService.goodPostEndpoint(goodData),
      { errorMessage: '创建商品失败' }
    )

    return response.data
  }

  /**
   * 更新商品
   */
  const updateGood = async (id: string, goodData: GoodPatchEndpointRequest): Promise<GoodDetailResponse | null> => {
    const { executeApi, requireAuth, hasRole } = useApi()
    
    if (!requireAuth()) return null
    if (!hasRole('Publisher')) {
      throw new Error('需要发布者权限才能更新商品')
    }

    const response = await executeApi(
      () => GoodsService.goodPatchEndpoint(id, goodData),
      { errorMessage: '更新商品失败' }
    )

    return response.data
  }

  /**
   * 删除商品
   */
  const deleteGood = async (id: string): Promise<boolean> => {
    const { executeApi, requireAuth, hasRole } = useApi()
    
    if (!requireAuth()) return false
    if (!hasRole('Publisher')) {
      throw new Error('需要发布者权限才能删除商品')
    }

    const response = await executeApi(
      () => GoodsService.goodDeleteEndpoint(id),
      { errorMessage: '删除商品失败' }
    )

    return !response.error
  }

  /**
   * 获取商品状态的中文名称
   */
  const getGoodStatusName = (status: GoodStatus): string => {
    const statusNames: Record<GoodStatus, string> = {
      [GoodStatus._0]: '草稿',
      [GoodStatus._1]: '可购买',
      [GoodStatus._2]: '不可购买'
    }
    return statusNames[status] || '未知状态'
  }

  /**
   * 获取商品状态的颜色类
   */
  const getGoodStatusColor = (status: GoodStatus): string => {
    const statusColors: Record<GoodStatus, string> = {
      [GoodStatus._0]: 'text-gray-500',
      [GoodStatus._1]: 'text-green-500',
      [GoodStatus._2]: 'text-red-500'
    }
    return statusColors[status] || 'text-gray-500'
  }

  /**
   * 格式化价格
   */
  const formatPrice = (price: number): string => {
    return `¥${price.toFixed(2)}`
  }

  /**
   * 计算折扣百分比
   */
  const getDiscountPercentage = (originalPrice: number, discountedPrice: number): number => {
    if (discountedPrice >= originalPrice) return 0
    return Math.round(((originalPrice - discountedPrice) / originalPrice) * 100)
  }

  // 计算总页数
  const totalPages = computed(() => {
    return Math.ceil(total.value / 12) // 默认每页12条
  })

  return {
    goods: readonly(goods),
    loading: readonly(loading),
    error: readonly(error),
    total: readonly(total),
    totalPages: readonly(totalPages),
    fetchGoods,
    getGoodById,
    createGood,
    updateGood,
    deleteGood,
    getGoodStatusName,
    getGoodStatusColor,
    formatPrice,
    getDiscountPercentage
  }
}
