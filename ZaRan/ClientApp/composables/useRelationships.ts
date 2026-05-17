import { ref, readonly, onMounted } from 'vue'
import { RelationshipService, type GoodDetailResponse } from '~/api'
import { useApi } from '~/composables/useApi'

export type RelationshipType = 'like' | 'follow' | 'cart' | 'favorite'

// Determine the correct backend type based on context
const getBackendRelationshipType = (frontendType: RelationshipType, context?: 'article' | 'good' | 'user'): string => {
  if (frontendType === 'like') {
    // If context is provided, use it; otherwise default to ArticleLike
    if (context === 'good') return 'GoodLike'
    if (context === 'article') return 'ArticleLike'
    // Default to ArticleLike if no context provided
    return 'ArticleLike'
  }

  const typeMapping: Record<RelationshipType, string> = {
    'like': 'ArticleLike',      // Default for likes
    'follow': 'Follow',         // User follow
    'cart': 'GoodCart',         // Shopping cart for goods
    'favorite': 'ArticleLike'   // Using ArticleLike for favorites (could be extended)
  }
  return typeMapping[frontendType]
}

export function useRelationships() {
  const { executeApi, requireAuth } = useApi()
  const userStore = useUserStore()

  /**
   * 添加关系（点赞、关注、加入购物车等）
   */
  const addRelationship = async (targetType: RelationshipType, targetId: string, context?: 'article' | 'good' | 'user'): Promise<boolean> => {
    if (!requireAuth()) return false

    const userId = userStore.userId
    if (!userId) return false

    const backendType = getBackendRelationshipType(targetType, context)
    const response = await executeApi(
      () => RelationshipService.userRelationshipAddEndpoint(userId, backendType, targetId),
      { errorMessage: `${getRelationshipActionName(targetType)}失败` }
    )

    return !response.error
  }

  /**
   * 移除关系
   */
  const removeRelationship = async (targetType: RelationshipType, targetId: string, context?: 'article' | 'good' | 'user'): Promise<boolean> => {
    if (!requireAuth()) return false

    const userId = userStore.userId
    if (!userId) return false

    const backendType = getBackendRelationshipType(targetType, context)
    const response = await executeApi(
      () => RelationshipService.userRelationshipDeleteEndpoint(userId, backendType, targetId),
      { errorMessage: `取消${getRelationshipActionName(targetType)}失败` }
    )

    return !response.error
  }

  /**
   * 获取用户的关系列表
   */
  const getUserRelationships = async (
    targetType: RelationshipType,
    page: number = 1,
    pageSize: number = 12,
    context?: 'article' | 'good' | 'user'
  ) => {
    if (!requireAuth()) return null

    const userId = userStore.userId
    if (!userId) return null

    const { createPaginationParams } = useApi()
    const paginationParams = createPaginationParams(page, pageSize, true)
    const backendType = getBackendRelationshipType(targetType, context)

    const response = await executeApi(
      () => RelationshipService.userRelationshipGetEndpoint(
        backendType,
        userId,
        true, // target = true means get relationships where user is the actor
        paginationParams.offset,
        paginationParams.limit,
        paginationParams.desc
      ),
      { showError: false }
    )

    return response.data
  }

  /**
   * 检查是否存在关系
   * Since there's no dedicated check endpoint, we use the list endpoint with pagination to check existence
   */
  const checkRelationship = async (targetType: RelationshipType, targetId: string, context?: 'article' | 'good' | 'user'): Promise<boolean> => {
    if (!userStore.isLoggedIn) return false

    const userId = userStore.userId
    if (!userId) return false

    const backendType = getBackendRelationshipType(targetType, context)

    const response = await executeApi(
      () => RelationshipService.userRelationshipGetEndpoint(
        backendType,
        userId,
        true, // target = true means get relationships where user is the actor
        0,    // offset = 0
        1000, // limit = 1000 (should be enough to check if relationship exists)
        false // desc = false
      ),
      { showError: false }
    )

    if (response.error || !response.data) return false

    // Check if the targetId exists in the returned items
    const items = response.data.items || []
    return items.some((item: { id: string }) => item.id === targetId)
  }

  /**
   * 切换关系状态（如果存在则移除，不存在则添加）
   */
  const toggleRelationship = async (targetType: RelationshipType, targetId: string, context?: 'article' | 'good' | 'user'): Promise<boolean> => {
    const exists = await checkRelationship(targetType, targetId, context)

    if (exists) {
      return await removeRelationship(targetType, targetId, context)
    } else {
      return await addRelationship(targetType, targetId, context)
    }
  }

  /**
   * 获取关系操作的中文名称
   */
  const getRelationshipActionName = (type: RelationshipType): string => {
    const actionNames: Record<RelationshipType, string> = {
      'like': '点赞',
      'follow': '关注',
      'cart': '加入购物车',
      'favorite': '收藏'
    }
    return actionNames[type] || '操作'
  }

  /**
   * 获取关系的图标
   */
  const getRelationshipIcon = (type: RelationshipType, isActive: boolean = false): string => {
    const icons: Record<RelationshipType, { active: string, inactive: string }> = {
      'like': { active: 'fas fa-heart', inactive: 'far fa-heart' },
      'follow': { active: 'fas fa-user-check', inactive: 'fas fa-user-plus' },
      'cart': { active: 'fas fa-shopping-cart', inactive: 'fas fa-cart-plus' },
      'favorite': { active: 'fas fa-star', inactive: 'far fa-star' }
    }
    
    return isActive ? icons[type].active : icons[type].inactive
  }

  /**
   * 获取关系的颜色类
   */
  const getRelationshipColor = (type: RelationshipType, isActive: boolean = false): string => {
    if (!isActive) return 'text-gray-500'
    
    const colors: Record<RelationshipType, string> = {
      'like': 'text-red-500',
      'follow': 'text-blue-500',
      'cart': 'text-green-500',
      'favorite': 'text-yellow-500'
    }
    
    return colors[type] || 'text-gray-500'
  }

  return {
    addRelationship,
    removeRelationship,
    getUserRelationships,
    checkRelationship,
    toggleRelationship,
    getRelationshipActionName,
    getRelationshipIcon,
    getRelationshipColor
  }
}

/**
 * 专门用于点赞功能的组合式函数
 */
export function useLikes(targetId: string, context: 'article' | 'good' = 'article') {
  const { checkRelationship, toggleRelationship } = useRelationships()
  const isLiked = ref(false)
  const loading = ref(false)

  const checkLikeStatus = async () => {
    loading.value = true
    try {
      isLiked.value = await checkRelationship('like', targetId, context)
    } finally {
      loading.value = false
    }
  }

  const toggleLike = async () => {
    loading.value = true
    try {
      const success = await toggleRelationship('like', targetId, context)
      if (success) {
        isLiked.value = !isLiked.value
      }
      return success
    } finally {
      loading.value = false
    }
  }

  // 初始化时检查点赞状态
  onMounted(() => {
    checkLikeStatus()
  })

  return {
    isLiked: readonly(isLiked),
    loading: readonly(loading),
    toggleLike,
    checkLikeStatus
  }
}

/**
 * 专门用于关注功能的组合式函数
 */
export function useFollows(targetId: string) {
  const { checkRelationship, toggleRelationship } = useRelationships()
  const isFollowing = ref(false)
  const loading = ref(false)

  const checkFollowStatus = async () => {
    loading.value = true
    try {
      isFollowing.value = await checkRelationship('follow', targetId, 'user')
    } finally {
      loading.value = false
    }
  }

  const toggleFollow = async () => {
    loading.value = true
    try {
      const success = await toggleRelationship('follow', targetId, 'user')
      if (success) {
        isFollowing.value = !isFollowing.value
      }
      return success
    } finally {
      loading.value = false
    }
  }

  // 初始化时检查关注状态
  onMounted(() => {
    checkFollowStatus()
  })

  return {
    isFollowing: readonly(isFollowing),
    loading: readonly(loading),
    toggleFollow,
    checkFollowStatus
  }
}

/**
 * 专门用于购物车功能的组合式函数
 */
export function useCart() {
  const { addRelationship, removeRelationship, getUserRelationships } = useRelationships()
  const cartItems = ref<GoodDetailResponse[]>([])
  const loading = ref(false)

  const loadCartItems = async () => {
    loading.value = true
    try {
      const response = await getUserRelationships('cart', 1, 1000, 'good')
      cartItems.value = response?.items || []
    } finally {
      loading.value = false
    }
  }

  const addToCart = async (goodId: string) => {
    const success = await addRelationship('cart', goodId, 'good')
    if (success) {
      await loadCartItems()
    }
    return success
  }

  const removeFromCart = async (goodId: string) => {
    const success = await removeRelationship('cart', goodId, 'good')
    if (success) {
      await loadCartItems()
    }
    return success
  }

  const clearCart = async () => {
    const promises = cartItems.value.map(item => removeRelationship('cart', item.id, 'good'))
    await Promise.all(promises)
    cartItems.value = []
  }

  const cartItemCount = computed(() => cartItems.value.length)

  const cartTotal = computed(() => {
    return cartItems.value.reduce((total, item) => {
      const price = item.discountedPrice || item.price || 0
      return total + price
    }, 0)
  })

  // 初始化时加载购物车
  onMounted(() => {
    loadCartItems()
  })

  return {
    cartItems: readonly(cartItems),
    loading: readonly(loading),
    cartItemCount: readonly(cartItemCount),
    cartTotal: readonly(cartTotal),
    loadCartItems,
    addToCart,
    removeFromCart,
    clearCart
  }
}
