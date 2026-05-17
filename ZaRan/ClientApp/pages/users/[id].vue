<template>
  <div class="user-profile-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <h1>{{ isOwnProfile ? '我的账户' : `${user?.nickname || user?.username || '用户'}的主页` }}</h1>
      <p>{{ isOwnProfile ? '管理您的个人信息和作品' : '查看用户的作品和信息' }}</p>
    </div>

    <div class="page-content container">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>加载用户信息中...</p>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="error-container">
        <p class="error-message">{{ error }}</p>
        <button @click="loadUserInfo" class="retry-btn">重试</button>
      </div>

      <!-- 用户信息 -->
      <div v-else-if="user" class="user-profile">
        <!-- 用户基本信息 -->
        <div class="user-header">
          <div class="user-avatar">
            <img
              :src="user.avatar || '/image/default-avatar.jpg'"
              :alt="user.nickname || user.username"
            >
          </div>
          <div class="user-info">
            <h2 class="user-name">{{ user.nickname || user.username }}</h2>
            <p class="user-bio">{{ user.signature || '这个用户很神秘，什么都没有留下...' }}</p>
            <div class="user-meta">
              <span class="join-date">加入时间: {{ formatDate(user.createdAt!) }}</span>
            </div>
            <!-- 统计数据 -->
            <div class="profile-stats">
              <div class="stat-item" @click="setActiveTab('works')">
                <div class="stat-number">{{ userArticles.length || 0 }}</div>
                <div class="stat-label">作品</div>
              </div>
              <div class="stat-item" @click="setActiveTab('favorites')" v-if="isOwnProfile">
                <div class="stat-number">{{ stats.favoritesCount }}</div>
                <div class="stat-label">收藏</div>
              </div>
              <div class="stat-item" @click="setActiveTab('following')">
                <div class="stat-number">{{ stats.followingCount }}</div>
                <div class="stat-label">关注</div>
              </div>
              <div class="stat-item" @click="setActiveTab('orders')" v-if="isOwnProfile">
                <div class="stat-number">{{ stats.ordersCount }}</div>
                <div class="stat-label">订单</div>
              </div>
            </div>
          </div>
          <div class="user-actions">
            <!-- 自己的资料 -->
            <template v-if="isOwnProfile">
              <button class="btn btn-primary" @click="showEditProfile = true">
                <i class="fas fa-edit"></i>编辑资料
              </button>
              <button class="btn btn-outline">
                <i class="fas fa-share-alt"></i>分享主页
              </button>
              <button class="btn btn-outline" @click="setActiveTab('settings')">
                <i class="fas fa-cog"></i>账户设置
              </button>
            </template>
            <!-- 他人的资料 -->
            <template v-else-if="userStore.isLoggedIn">
              <button
                @click="toggleFollow"
                :class="['follow-btn', { 'following': isFollowing }]"
                :disabled="followLoading"
              >
                <i :class="isFollowing ? 'fas fa-user-check' : 'fas fa-user-plus'"></i>
                {{ isFollowing ? '已关注' : '关注' }}
              </button>
              <button class="btn btn-outline">
                <i class="fas fa-envelope"></i>发消息
              </button>
              <button class="btn btn-outline">
                <i class="fas fa-share-alt"></i>分享主页
              </button>
            </template>
          </div>
        </div>

        <!-- 标签页导航 -->
        <div class="tabs-nav">
          <button
            v-for="tab in availableTabs"
            :key="tab.id"
            @click="setActiveTab(tab.id)"
            :class="['tab-btn', { active: activeTab === tab.id }]"
          >
            <i :class="tab.icon"></i>
            {{ tab.name }}
          </button>
        </div>

        <!-- 标签页内容 -->
        <div class="tab-content">
          <!-- 用户作品 -->
          <div v-if="activeTab === 'works'" class="tab-panel">
            <div class="card-header">
              <h3 class="card-title">{{ isOwnProfile ? '我的作品' : `${user?.nickname || '用户'}的作品` }}</h3>
              <div class="card-actions" v-if="isOwnProfile">
                <button class="card-action" title="添加作品" @click="navigateToCreate">
                  <i class="fas fa-plus"></i>
                </button>
                <button class="card-action" title="排序">
                  <i class="fas fa-sort"></i>
                </button>
                <button class="card-action" title="筛选">
                  <i class="fas fa-filter"></i>
                </button>
              </div>
            </div>
            <UserArticlesList
              v-if="user?.id"
              :user-id="user.id"
              article-type="pattern"
              :show-type="true"
              :empty-message="isOwnProfile ? '还没有发布作品，快来创作第一个作品吧！' : '该用户还没有发布作品'"
              @article-click="handleArticleClick"
            />
            <!-- 创建作品按钮 (仅自己可见) -->
            <div v-if="isOwnProfile" class="create-work-section">
              <button class="create-btn" @click="navigateToCreate">
                <i class="fas fa-plus"></i> 创建新作品
              </button>
            </div>
          </div>

          <!-- 我的收藏 -->
          <div v-if="activeTab === 'favorites'" class="tab-panel">
            <div class="card-header">
              <h3 class="card-title">我的收藏</h3>
              <div class="card-actions">
                <button class="card-action" title="管理">
                  <i class="fas fa-ellipsis-h"></i>
                </button>
              </div>
            </div>

            <div v-if="favoritesLoading" class="loading-container">
              <div class="loading-spinner"></div>
              <p>加载收藏中...</p>
            </div>

            <div v-else-if="myFavorites.length === 0" class="empty-state">
              <i class="fas fa-heart empty-icon"></i>
              <p>您还没有收藏任何内容</p>
            </div>

            <div v-else class="favorites-grid">
              <div class="work-card" v-for="item in myFavorites" :key="item.id" @click="navigateToFavorite(item)">
                <div class="work-image" :style="{ backgroundImage: `url(${getFavoriteImage(item)})` }"></div>
                <div class="work-info">
                  <div class="work-title">{{ getFavoriteTitle(item) }}</div>
                  <div class="work-meta">
                    <span>{{ getFavoriteType(item) }}</span>
                    <span>{{ formatDate(item.createdAt) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- 我的关注 -->
          <div v-if="activeTab === 'following'" class="tab-panel">
            <div class="card-header">
              <h3 class="card-title">{{ isOwnProfile ? '我的关注' : `${user?.nickname || '用户'}的关注` }}</h3>
            </div>

            <div v-if="followingLoading" class="loading-container">
              <div class="loading-spinner"></div>
              <p>加载关注列表中...</p>
            </div>

            <div v-else-if="myFollowing.length === 0" class="empty-state">
              <i class="fas fa-user-friends empty-icon"></i>
              <p>{{ isOwnProfile ? '您还没有关注任何用户' : '该用户还没有关注任何人' }}</p>
            </div>

            <div v-else class="favorites-grid">
              <div class="work-card" v-for="followedUser in myFollowing" :key="followedUser.id" @click="navigateToUser(followedUser)">
                <div class="work-image" :style="{ backgroundImage: `url(${followedUser.avatar || '/image/default-avatar.jpg'})` }"></div>
                <div class="work-info">
                  <div class="work-title">{{ followedUser.nickname || followedUser.username }}</div>
                  <div class="work-meta">
                    <span>{{ followedUser.signature || '暂无签名' }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- 我的订单 -->
          <div v-if="activeTab === 'orders'" class="tab-panel">
            <div class="card-header">
              <h3 class="card-title">我的订单</h3>
              <div class="card-actions">
                <button class="card-action" title="查看全部">
                  <i class="fas fa-ellipsis-h"></i>
                </button>
              </div>
            </div>

            <div v-if="ordersLoading" class="loading-container">
              <div class="loading-spinner"></div>
              <p>加载订单中...</p>
            </div>

            <div v-else-if="myOrders.length === 0" class="empty-state">
              <i class="fas fa-receipt empty-icon"></i>
              <p>您还没有任何订单</p>
            </div>

            <div v-else class="orders-list">
              <div class="order-item" v-for="order in myOrders" :key="order.id" @click="navigateToOrder(order)">
                <div class="order-image" :style="{ backgroundImage: `url(${getOrderImage(order)})` }"></div>
                <div class="order-detail">
                  <div class="order-title">{{ getOrderTitle(order) }}</div>
                  <div class="order-meta">
                    <div>订单号: {{ order.id?.substring(0, 12) }}...</div>
                    <div>下单时间: {{ formatDate(order.createdAt) }}</div>
                    <div>金额: ¥{{ order.totalAmount?.toFixed(2) || '0.00' }}</div>
                  </div>
                  <span :class="['order-status', getOrderStatusClass(order)]">{{ getOrderStatusText(order) }}</span>
                </div>
                <div class="order-action" @click.stop>
                  <button class="btn btn-outline" @click="navigateToOrder(order)">查看详情</button>
                </div>
              </div>
            </div>
          </div>

          <!-- 账户设置 -->
          <div v-if="activeTab === 'settings'" class="tab-panel">
            <div class="card-header">
              <h3 class="card-title">账户设置</h3>
            </div>

            <div class="settings-form">
              <div class="setting-group">
                <label class="setting-label">个人信息</label>
                <div class="setting-item">
                  <span class="setting-name">用户名</span>
                  <span class="setting-value">{{ userStore.user?.username }}</span>
                  <button class="setting-edit-btn" @click="showEditProfile = true">修改</button>
                </div>
                <div class="setting-item">
                  <span class="setting-name">邮箱</span>
                  <span class="setting-value">{{ userStore.user?.email || '未设置' }}</span>
                  <button class="setting-edit-btn" @click="showEditProfile = true">修改</button>
                </div>
                <div class="setting-item">
                  <span class="setting-name">余额</span>
                  <span class="setting-value">¥{{ userStore.user?.balance?.toFixed(2) || '0.00' }}</span>
                  <button class="setting-edit-btn">充值</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { UserDetailResponse, ArticleDetailResponse, TransactionDetailResponse } from '~/api'
import { UsersService } from '~/api'

// 获取路由参数
const route = useRoute()
const userId = route.params.id as string

// 使用相关的composables
const userStore = useUserStore()
const { getUserRelationships, toggleRelationship, checkRelationship } = useRelationships()
const { articles: userArticles, loading: articlesLoading, fetchUserArticles } = useArticles('story')
const { fetchTransactions } = useTransactions()

// 状态管理
const user = ref<UserDetailResponse | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)
const isFollowing = ref(false)
const followLoading = ref(false)
const activeTab = ref('works')
const showEditProfile = ref(false)

// 判断是否为自己的资料
const isOwnProfile = computed(() => {
  if (!userId) return true // 没有ID参数，默认为自己的资料
  return userId === userStore.userId
})

// 页面元数据
useHead({
  title: computed(() => isOwnProfile.value ? '我的 - 檐下风铃' : `${user.value?.nickname || '用户'}的主页 - 檐下风铃`),
  meta: [
    { name: 'description', content: computed(() => isOwnProfile.value ? '个人中心页面' : '用户主页') }
  ]
})

// 定义收藏项类型
interface FavoriteItem {
  id: string
  title?: string
  coverImage?: string
  image?: string
  articleType?: number
  createdAt?: string
}

// 数据状态
const myFavorites = ref<FavoriteItem[]>([])
const myOrders = ref<TransactionDetailResponse[]>([])
const myFollowing = ref<UserDetailResponse[]>([])

// 加载状态
const favoritesLoading = ref(false)
const ordersLoading = ref(false)
const followingLoading = ref(false)

// 统计数据
const stats = ref({
  favoritesCount: 0,
  ordersCount: 0,
  followingCount: 0
})

// 标签页数据
const allTabs = [
  { id: 'works', name: '作品', icon: 'fas fa-paint-brush' },
  { id: 'favorites', name: '收藏', icon: 'fas fa-heart' },
  { id: 'orders', name: '订单', icon: 'fas fa-receipt' },
  { id: 'following', name: '关注', icon: 'fas fa-user-friends' },
  { id: 'settings', name: '设置', icon: 'fas fa-cog' }
]

// 根据是否为自己的资料显示不同的标签
const availableTabs = computed(() => {
  if (isOwnProfile.value) {
    return allTabs
  } else {
    // 他人资料只显示作品和关注
    return allTabs.filter(tab => ['works', 'following'].includes(tab.id))
  }
})

// 方法
const setActiveTab = (tabId: string) => {
  activeTab.value = tabId

  // 根据选中的标签页加载对应数据
  switch (tabId) {
    case 'works':
      // UserArticlesList 组件会自动处理数据加载，无需手动调用
      break
    case 'favorites':
      loadMyFavorites()
      break
    case 'orders':
      loadMyOrders()
      break
    case 'following':
      loadMyFollowing()
      break
  }
}

// 加载用户信息
const loadUserInfo = async () => {
  if (isOwnProfile.value) {
    // 自己的资料，使用userStore中的数据
    user.value = userStore.user
    // 加载用户文章以获取数量统计
    if (userStore.userId) {
      await fetchUserArticles(userStore.userId, 1, 100) // 获取足够多的文章来统计数量
    }
  } else if (userId) {
    // 他人的资料，从API获取
    loading.value = true
    error.value = null
    try {
      const response = await UsersService.userDetailEndpoint(userId)
      user.value = response

      // 加载用户文章以获取数量统计
      await fetchUserArticles(userId, 1, 100)

      // 检查是否已关注
      if (userStore.isLoggedIn) {
        isFollowing.value = await checkRelationship('follow', userId, 'user')
      }
    } catch (err) {
      error.value = '加载用户信息失败，请稍后重试'
      console.error('加载用户信息失败:', err)
    } finally {
      loading.value = false
    }
  }
}

// 切换关注状态
const toggleFollow = async () => {
  if (!userId || !userStore.isLoggedIn) {
    navigateTo('/login')
    return
  }

  followLoading.value = true
  try {
    const success = await toggleRelationship('follow', userId, 'user')
    if (success) {
      isFollowing.value = !isFollowing.value
      // 更新关注数统计
      if (isFollowing.value) {
        stats.value.followingCount++
      } else {
        stats.value.followingCount--
      }
    }
  } catch (error) {
    console.error('关注操作失败:', error)
  } finally {
    followLoading.value = false
  }
}

// 处理文章点击
const handleArticleClick = (article: ArticleDetailResponse) => {
  console.log('点击文章:', article.title)
  // 导航到文章详情页面
  navigateTo(`/articles/${article.id}`)
}

// 格式化日期
const formatDate = (dateString?: string) => {
  if (!dateString) return '未知时间'
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

// 加载收藏数据（仅自己可见）
const loadMyFavorites = async () => {
  if (!isOwnProfile.value) {
    stats.value.favoritesCount = 0
    return
  }
  favoritesLoading.value = true
  try {
    // 获取用户的收藏关系
    const favoritesData = await getUserRelationships('like', 1, 50, 'article')
    myFavorites.value = favoritesData?.items || []
    stats.value.favoritesCount = myFavorites.value.length
  } catch (error) {
    console.error('加载收藏失败:', error)
  } finally {
    favoritesLoading.value = false
  }
}

// 加载订单数据（仅自己可见）
const loadMyOrders = async () => {
  if (!isOwnProfile.value) {
    stats.value.ordersCount = 0
    return
  }

  const targetUserId = userStore.userId
  if (!targetUserId) {
    // 模拟订单数据
    myOrders.value = [
      {
        id: 'order-001',
        totalAmount: 298.00,
        createdAt: '2024-01-15T10:30:00Z',
        transactionStatus: 1
      },
      {
        id: 'order-002',
        totalAmount: 168.00,
        createdAt: '2024-01-12T14:20:00Z',
        transactionStatus: 2
      }
    ] as TransactionDetailResponse[]
    stats.value.ordersCount = myOrders.value.length
    return
  }

  ordersLoading.value = true
  try {
    // 获取用户的交易记录
    const { transactions } = useTransactions()
    await fetchTransactions(1, 50)
    myOrders.value = [...(transactions.value || [])]
    stats.value.ordersCount = myOrders.value.length
  } catch (error) {
    console.error('加载订单失败:', error)
  } finally {
    ordersLoading.value = false
  }
}

// 加载关注数据
const loadMyFollowing = async () => {
  const targetUserId = userId || userStore.userId

  if (!targetUserId) {
    // 模拟关注数据
    myFollowing.value = [
      {
        id: 'user-001',
        username: 'designer_zhang',
        nickname: '张设计师',
        avatar: '/image/default-avatar.jpg',
        signature: '专业扎染设计师，10年经验'
      },
      {
        id: 'user-002',
        username: 'artist_li',
        nickname: '李艺术家',
        avatar: '/image/default-avatar.jpg',
        signature: '传统工艺传承者'
      }
    ] as UserDetailResponse[]
    stats.value.followingCount = myFollowing.value.length
    return
  }

  followingLoading.value = true
  try {
    // 获取指定用户的关注关系
    const followingData = await getUserRelationships('follow', 1, 50, 'user')
    myFollowing.value = followingData?.items || []
    stats.value.followingCount = myFollowing.value.length
  } catch (error) {
    console.error('加载关注列表失败:', error)
  } finally {
    followingLoading.value = false
  }
}

// 工具方法
const navigateToCreate = () => {
  navigateTo('/workshop')
}

// 收藏相关方法
const getFavoriteImage = (item: FavoriteItem) => {
  return item.coverImage || item.image || '/image/default-work.jpg'
}

const getFavoriteTitle = (item: FavoriteItem) => {
  return item.title || '未知标题'
}

const getFavoriteType = (item: FavoriteItem) => {
  // 根据类型返回中文类型名
  if (item.articleType !== undefined) {
    switch (item.articleType) {
      case 0: return 'IP故事'
      case 1: return '角色介绍'
      case 2: return '基因库'
      case 3: return '百科'
      case 4: return '社区帖子'
      default: return '文章'
    }
  }
  return '内容'
}

// 订单相关方法
const getOrderImage = (_order: TransactionDetailResponse) => {
  // 从订单中获取商品图片
  return '/image/default-product.jpg'
}

const getOrderTitle = (order: TransactionDetailResponse) => {
  return `订单 ${order.id?.substring(0, 8)}...`
}

const getOrderStatusClass = (order: TransactionDetailResponse) => {
  switch (order.transactionStatus) {
    case 0: return 'status-pending'
    case 1: return 'status-completed'
    case 2: return 'status-shipped'
    default: return 'status-pending'
  }
}

const getOrderStatusText = (order: TransactionDetailResponse) => {
  switch (order.transactionStatus) {
    case 0: return '待处理'
    case 1: return '已完成'
    case 2: return '已发货'
    default: return '未知状态'
  }
}

// 导航方法
const navigateToFavorite = (item: FavoriteItem) => {
  console.log('点击收藏:', item.title, '类型:', item.articleType)

  // 根据收藏项类型跳转到对应页面
  if (item.articleType !== undefined) {
    switch (item.articleType) {
      case 0: // Story
        navigateTo(`/ip-story/${item.id}`)
        break
      case 1: // Character
        navigateTo(`/ip-story/character/${item.id}`)
        break
      case 2: // Gene
        navigateTo(`/pattern-library/${item.id}`)
        break
      case 3: // Wiki
        navigateTo(`/knowledge/${item.id}`)
        break
      case 4: // Post
        navigateTo(`/community/${item.id}`)
        break
      default:
        navigateTo(`/articles/${item.id}`)
    }
  } else {
    // 如果没有类型信息，默认跳转到文章页面
    navigateTo(`/articles/${item.id}`)
  }
}

const navigateToOrder = (order: TransactionDetailResponse) => {
  console.log('点击订单:', order.id)
  // 跳转到订单详情页面
  navigateTo(`/orders/${order.id}`)
}

const navigateToUser = (user: UserDetailResponse) => {
  console.log('点击用户:', user.nickname || user.username)
  // 跳转到用户主页
  navigateTo(`/users/${user.id}`)
}

// 初始化数据
onMounted(async () => {
  await loadUserInfo()
  if (isOwnProfile.value) {
    loadMyFavorites()
    loadMyOrders()
  }
  loadMyFollowing()
})

// 监听用户登录状态变化
watch(() => userStore.isLoggedIn, (isLoggedIn) => {
  if (isLoggedIn) {
    loadUserInfo()
    if (isOwnProfile.value) {
      loadMyFavorites()
      loadMyOrders()
    }
    loadMyFollowing()
  }
})

// 监听路由参数变化
watch(() => route.params.id, async () => {
  await loadUserInfo()
  if (isOwnProfile.value) {
    loadMyFavorites()
    loadMyOrders()
  }
  loadMyFollowing()
  // 重置到作品标签页
  activeTab.value = 'works'
})
</script>

<style scoped>
.user-profile-page {
  min-height: 100vh;
  background-color: #f8fafc;
}

.page-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 3rem 0;
  text-align: center;
}

.page-header h1 {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.page-header p {
  font-size: 1.125rem;
  opacity: 0.9;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1rem;
}

.loading-container,
.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  text-align: center;
}

.loading-spinner {
  width: 2rem;
  height: 2rem;
  border: 2px solid #f3f4f6;
  border-top: 2px solid #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.error-message {
  color: #ef4444;
  margin-bottom: 1rem;
}

.retry-btn {
  padding: 0.5rem 1rem;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.retry-btn:hover {
  background-color: #2563eb;
}

.user-profile {
  background: white;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
  overflow: hidden;
}

.user-header {
  display: flex;
  align-items: center;
  gap: 2rem;
  padding: 2rem;
  border-bottom: 1px solid #e5e7eb;
  flex-wrap: wrap;
}

.user-avatar {
  flex-shrink: 0;
}

.user-avatar img {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  object-fit: cover;
  border: 4px solid #f3f4f6;
}

.user-info {
  flex: 1;
  min-width: 250px;
}

.user-name {
  font-size: 1.875rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.user-bio {
  color: #6b7280;
  font-size: 1rem;
  line-height: 1.5;
  margin-bottom: 1rem;
}

.user-meta {
  color: #9ca3af;
  font-size: 0.875rem;
  margin-bottom: 1rem;
}

/* 统计数据样式 */
.profile-stats {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 1rem;
}

.stat-item {
  text-align: center;
  cursor: pointer;
  transition: transform 0.2s;
}

.stat-item:hover {
  transform: translateY(-2px);
}

.stat-number {
  font-size: 1.25rem;
  font-weight: 600;
  color: #3b82f6;
  display: block;
}

.stat-label {
  font-size: 0.85rem;
  color: #6b7280;
}

.user-actions {
  flex-shrink: 0;
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
  flex-wrap: wrap;
}

/* 按钮样式 */
.btn {
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  border: none;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.3s;
  display: inline-flex;
  align-items: center;
  text-decoration: none;
}

.btn i {
  margin-right: 0.5rem;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
}

.btn-primary:hover {
  background-color: #2563eb;
}

.btn-outline {
  background-color: transparent;
  border: 1px solid #3b82f6;
  color: #3b82f6;
}

.btn-outline:hover {
  background-color: rgba(59, 130, 246, 0.1);
}

.follow-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.follow-btn:hover {
  background-color: #2563eb;
}

.follow-btn.following {
  background-color: #10b981;
}

.follow-btn.following:hover {
  background-color: #059669;
}

.follow-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.tabs-nav {
  display: flex;
  border-bottom: 1px solid #e5e7eb;
}

.tab-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem 1.5rem;
  background: none;
  border: none;
  color: #6b7280;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border-bottom: 2px solid transparent;
}

.tab-btn:hover {
  color: #374151;
  background-color: #f9fafb;
}

.tab-btn.active {
  color: #3b82f6;
  border-bottom-color: #3b82f6;
}

.tab-content {
  padding: 2rem;
}

/* 卡片样式 */
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.card-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
}

.card-actions {
  display: flex;
  gap: 0.5rem;
}

.card-action {
  background: none;
  border: none;
  color: #6b7280;
  cursor: pointer;
  font-size: 1rem;
  padding: 0.5rem;
  border-radius: 0.375rem;
  transition: all 0.3s;
}

.card-action:hover {
  color: #3b82f6;
  background-color: rgba(59, 130, 246, 0.1);
}

.tab-panel h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 1.5rem;
}

/* 网格样式 */
.favorites-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: 1.5rem;
}

.work-card {
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  overflow: hidden;
  transition: transform 0.3s, box-shadow 0.3s, border-color 0.3s;
  background: white;
  cursor: pointer;
}

.work-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.15);
  border-color: #3b82f6;
}

.work-image {
  height: 150px;
  background-color: #f3f4f6;
  background-size: cover;
  background-position: center;
  position: relative;
}

.work-info {
  padding: 1rem;
}

.work-title {
  font-weight: 600;
  margin-bottom: 0.5rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  color: #1f2937;
}

.work-meta {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  color: #6b7280;
}

/* 订单列表样式 */
.orders-list {
  display: grid;
  gap: 1rem;
}

.order-item {
  display: grid;
  grid-template-columns: 100px 1fr auto;
  gap: 1.5rem;
  padding: 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  align-items: center;
  background: white;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s, border-color 0.2s;
}

.order-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-color: #3b82f6;
}

.order-image {
  width: 100px;
  height: 100px;
  background-color: #f3f4f6;
  background-size: cover;
  background-position: center;
  border-radius: 0.5rem;
}

.order-detail {
  flex: 1;
}

.order-title {
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #1f2937;
}

.order-meta {
  font-size: 0.9rem;
  color: #6b7280;
  margin-bottom: 0.5rem;
}

.order-meta div {
  margin-bottom: 0.25rem;
}

.order-status {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.8rem;
  font-weight: 500;
}

.status-pending {
  background-color: #fff3cd;
  color: #856404;
}

.status-completed {
  background-color: #d4edda;
  color: #155724;
}

.status-shipped {
  background-color: #cce5ff;
  color: #004085;
}

.order-action {
  text-align: right;
}

/* 设置表单样式 */
.settings-form {
  max-width: 600px;
}

.setting-group {
  margin-bottom: 2rem;
}

.setting-label {
  font-size: 1.1rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 1rem;
  display: block;
}

.setting-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 0;
  border-bottom: 1px solid #e5e7eb;
}

.setting-item:last-child {
  border-bottom: none;
}

.setting-name {
  font-weight: 500;
  color: #1f2937;
}

.setting-value {
  color: #6b7280;
  margin-right: auto;
  margin-left: 1rem;
}

.setting-edit-btn {
  background: none;
  border: 1px solid #3b82f6;
  color: #3b82f6;
  padding: 0.25rem 0.75rem;
  border-radius: 0.375rem;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.3s;
}

.setting-edit-btn:hover {
  background-color: #3b82f6;
  color: white;
}

/* 空状态样式 */
.empty-state {
  text-align: center;
  padding: 3rem 0;
  color: #6b7280;
}

.empty-icon {
  font-size: 3rem;
  color: #e5e7eb;
  margin-bottom: 1rem;
}

.empty-state p {
  margin-bottom: 1rem;
  font-size: 1.1rem;
}

/* 创建作品按钮样式 */
.create-work-section {
  display: flex;
  justify-content: center;
  margin-top: 2rem;
  padding: 1rem;
}

.create-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.create-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.create-btn:active {
  transform: translateY(0);
}

@media (max-width: 768px) {
  .user-header {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }

  .user-avatar img {
    width: 100px;
    height: 100px;
  }

  .user-name {
    font-size: 1.5rem;
  }

  .profile-stats {
    justify-content: center;
  }

  .user-actions {
    justify-content: center;
  }

  .tabs-nav {
    flex-wrap: wrap;
  }

  .tab-btn {
    flex: 1;
    justify-content: center;
    min-width: 0;
  }

  .order-item {
    grid-template-columns: 1fr;
    text-align: center;
  }

  .order-image {
    width: 100%;
    height: 150px;
    margin: 0 auto;
  }

  .order-action {
    text-align: center;
    margin-top: 1rem;
  }

  .favorites-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  }
}

@media (max-width: 480px) {
  .page-header {
    padding: 2rem 0;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .profile-stats {
    flex-wrap: wrap;
    gap: 1rem;
  }

  .user-actions {
    flex-direction: column;
    width: 100%;
  }

  .btn {
    justify-content: center;
  }
}
</style>
