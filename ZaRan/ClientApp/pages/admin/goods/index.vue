<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">商品管理</h1>
        <p class="mt-2 text-sm text-gray-700">管理系统中的所有商品</p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <button
          @click="refreshGoods"
          :disabled="loading"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          刷新
        </button>
        <NuxtLink
          to="/admin/goods/create"
          class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          创建商品
        </NuxtLink>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white shadow rounded-lg mb-6">
      <div class="px-4 py-5 sm:p-6">
        <div class="grid grid-cols-1 md:grid-cols-6 gap-4">
          <div>
            <label for="search" class="block text-sm font-medium text-gray-700">搜索商品</label>
            <input
              id="search"
              v-model="filters.keyword"
              type="text"
              placeholder="商品名称"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @input="debouncedSearch"
            >
          </div>

          <div>
            <label for="type" class="block text-sm font-medium text-gray-700">商品类型</label>
            <select
              id="type"
              v-model="filters.type"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadGoods"
            >
              <option value="">所有类型</option>
              <option value="copyright">版权商品</option>
              <option value="physical">实体商品</option>
            </select>
          </div>

          <div>
            <label for="status" class="block text-sm font-medium text-gray-700">商品状态</label>
            <select
              id="status"
              v-model="filters.status"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadGoods"
            >
              <option value="">所有状态</option>
              <option v-for="status in goodStatuses" :key="status.value" :value="status.value">
                {{ status.name }}
              </option>
            </select>
          </div>
          
          <div>
            <label for="priceRange" class="block text-sm font-medium text-gray-700">价格范围</label>
            <select
              id="priceRange"
              v-model="filters.priceRange"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadGoods"
            >
              <option value="">所有价格</option>
              <option value="0-100">¥0 - ¥100</option>
              <option value="100-500">¥100 - ¥500</option>
              <option value="500-1000">¥500 - ¥1000</option>
              <option value="1000+">¥1000+</option>
            </select>
          </div>
          
          <div>
            <label for="publisher" class="block text-sm font-medium text-gray-700">发布者</label>
            <input
              id="publisher"
              v-model="filters.publisher"
              type="text"
              placeholder="发布者昵称"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @input="debouncedSearch"
            >
          </div>
          
          <div>
            <label for="sort" class="block text-sm font-medium text-gray-700">排序方式</label>
            <select
              id="sort"
              v-model="filters.desc"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadGoods"
            >
              <option :value="true">最新发布</option>
              <option :value="false">最早发布</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Goods table -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <LoadingOverlay v-if="loading" />
        
        <div v-else-if="goods.length === 0" class="text-center py-12">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">没有找到商品</h3>
          <p class="mt-1 text-sm text-gray-500">尝试调整搜索条件或筛选器</p>
        </div>
        
        <div v-else class="overflow-hidden">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  商品信息
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  价格
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  库存
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  状态
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  发布者
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  发布时间
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  操作
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="good in goods" :key="good.id" class="hover:bg-gray-50">
                <td class="px-6 py-4">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-16 w-16">
                      <img 
                        class="h-16 w-16 rounded-lg object-cover" 
                        :src="good.imageUrl || '/default-product.png'" 
                        :alt="good.name"
                      >
                    </div>
                    <div class="ml-4 flex-1">
                      <div class="text-sm font-medium text-gray-900">{{ good.name }}</div>
                      <div class="text-sm text-gray-500">{{ good.publisherName }}</div>
                      <div v-if="good.copyrightInfo" class="text-xs text-blue-600 mt-1 flex items-center">
                        <i class="fas fa-copyright mr-1"></i>
                        <span class="truncate">{{ good.copyrightInfo }}</span>
                      </div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">
                    <div class="font-medium">¥{{ good.price?.toFixed(2) || '0.00' }}</div>
                    <div v-if="good.discountedPrice && good.discountedPrice !== good.price" class="text-xs text-red-600">
                      折扣价: ¥{{ good.discountedPrice.toFixed(2) }}
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">
                    <span v-if="good.stoke === -1" class="text-green-600">无限</span>
                    <span v-else-if="good.stoke === 0" class="text-red-600">缺货</span>
                    <span v-else-if="good.stoke && good.stoke < 10" class="text-yellow-600">{{ good.stoke }}</span>
                    <span v-else class="text-green-600">{{ good.stoke }}</span>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusColorClass(good.status)">
                    {{ getStatusName(good.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-8 w-8">
                      <img 
                        class="h-8 w-8 rounded-full" 
                        :src="good.publisher?.avatar || '/default-avatar.png'" 
                        :alt="good.publisher?.nickname"
                      >
                    </div>
                    <div class="ml-3">
                      <div class="text-sm font-medium text-gray-900">{{ good.publisher?.nickname }}</div>
                      <div class="text-sm text-gray-500">@{{ good.publisher?.username }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(good.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <NuxtLink
                      :to="`/admin/goods/${good.id}`"
                      class="text-blue-600 hover:text-blue-900"
                    >
                      查看
                    </NuxtLink>
                    <NuxtLink
                      :to="`/admin/goods/${good.id}/edit`"
                      class="text-indigo-600 hover:text-indigo-900"
                    >
                      编辑
                    </NuxtLink>
                    <button
                      v-if="good.status === GOOD_STATUS.DRAFT"
                      @click="publishGood(good)"
                      class="text-green-600 hover:text-green-900"
                    >
                      发布
                    </button>
                    <button
                      v-else-if="good.status === GOOD_STATUS.AVAILABLE"
                      @click="unpublishGood(good)"
                      class="text-yellow-600 hover:text-yellow-900"
                    >
                      下架
                    </button>
                    <button
                      v-if="good.status === GOOD_STATUS.DRAFT || good.status === GOOD_STATUS.AVAILABLE"
                      @click="openRejectModal(good)"
                      class="text-orange-600 hover:text-orange-900"
                    >
                      审核不通过
                    </button>
                    <button
                      @click="deleteGood(good)"
                      class="text-red-600 hover:text-red-900"
                    >
                      删除
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <AdminPagination
          v-if="goods.length > 0"
          :current-page="currentPage"
          :total-items="totalGoods"
          :page-size="pageSize"
          @page-change="handlePageChange"
          class="mt-6"
        />
      </div>
    </div>

    <!-- 审核不通过模态框 -->
    <div v-if="showRejectModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900">审核不通过</h3>
            <button @click="closeRejectModal" class="text-gray-400 hover:text-gray-600">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
          </div>

          <div class="mb-4">
            <p class="text-sm text-gray-600 mb-2">
              商品：<span class="font-medium">{{ selectedGood?.name }}</span>
            </p>
            <label for="rejectMessage" class="block text-sm font-medium text-gray-700 mb-2">
              审核意见 <span class="text-red-500">*</span>
            </label>
            <textarea
              id="rejectMessage"
              v-model="rejectMessage"
              rows="4"
              class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              placeholder="请输入审核不通过的原因..."
            ></textarea>
            <p class="text-xs text-gray-500 mt-1">{{ rejectMessage.length }}/1000</p>
          </div>

          <div class="flex justify-end space-x-3">
            <button
              @click="closeRejectModal"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 border border-gray-300 rounded-md hover:bg-gray-200"
            >
              取消
            </button>
            <button
              @click="confirmRejectGood"
              :disabled="!rejectMessage.trim() || rejecting"
              class="px-4 py-2 text-sm font-medium text-white bg-red-600 border border-transparent rounded-md hover:bg-red-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ rejecting ? '处理中...' : '确认审核不通过' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { GoodsService, AdminService } from '~/api'
import type { GoodDetailResponse } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { createPaginationParams } = useApi()
const { isAdmin } = usePermissions()
const userStore = useUserStore()

// Utility function for debouncing
const useDebounceFn = (fn: (...args: unknown[]) => void, delay: number) => {
  let timeoutId: NodeJS.Timeout
  return (...args: unknown[]) => {
    clearTimeout(timeoutId)
    timeoutId = setTimeout(() => fn(...args), delay)
  }
}

// Good constants
const GOOD_STATUS = {
  DRAFT: 0,
  AVAILABLE: 1,
  UNAVAILABLE: 2,
  REJECTED: 3
}

// Data
const goods = ref<GoodDetailResponse[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(20)
const totalGoods = ref(0)

// 审核不通过相关状态
const showRejectModal = ref(false)
const selectedGood = ref<GoodDetailResponse | null>(null)
const rejectMessage = ref('')
const rejecting = ref(false)

// Filters
const filters = ref({
  keyword: '',
  type: '', // 新增商品类型过滤
  status: '',
  priceRange: '',
  publisher: '',
  desc: true
})

// Available options
const goodStatuses = [
  { value: GOOD_STATUS.DRAFT, name: '草稿' },
  { value: GOOD_STATUS.AVAILABLE, name: '可购买' },
  { value: GOOD_STATUS.UNAVAILABLE, name: '不可购买' },
  { value: GOOD_STATUS.REJECTED, name: '审核不通过' }
]

// Load goods
const loadGoods = async () => {
  loading.value = true

  try {
    const params = createPaginationParams(
      currentPage.value,
      pageSize.value,
      filters.value.desc,
      filters.value.keyword || undefined
    )

    // If user is not admin, only show their own goods
    const userId = isAdmin() ? undefined : userStore.userId

    // Use GoodsService with manual userId parameter
    const response = await GoodsService.goodListEndpoint(
      params.offset,
      params.limit,
      params.desc,
      params.keyword
    )

    // TODO: Filter by userId on frontend until API is updated
    let filteredGoods = response.items || []
    if (userId && !isAdmin()) {
      filteredGoods = filteredGoods.filter(good => good.publisherId === userId)
    }

    // 根据商品类型过滤
    if (filters.value.type) {
      if (filters.value.type === 'copyright') {
        filteredGoods = filteredGoods.filter(good => good.copyrightInfo && good.copyrightInfo.trim() !== '')
      } else if (filters.value.type === 'physical') {
        filteredGoods = filteredGoods.filter(good => !good.copyrightInfo || good.copyrightInfo.trim() === '')
      }
    }

    // 根据状态过滤
    if (filters.value.status !== '') {
      filteredGoods = filteredGoods.filter(good => good.status === parseInt(filters.value.status))
    }

    // 根据发布者过滤
    if (filters.value.publisher) {
      filteredGoods = filteredGoods.filter(good =>
        good.publisherName?.toLowerCase().includes(filters.value.publisher.toLowerCase()) ||
        good.publisher?.nickname?.toLowerCase().includes(filters.value.publisher.toLowerCase())
      )
    }

    // 根据价格范围过滤
    if (filters.value.priceRange) {
      const [min, max] = filters.value.priceRange.split('-').map(p => p.replace('+', ''))
      filteredGoods = filteredGoods.filter(good => {
        const price = good.discountedPrice || good.price || 0
        if (max) {
          return price >= parseInt(min) && price <= parseInt(max)
        } else {
          return price >= parseInt(min)
        }
      })
    }

    goods.value = filteredGoods
    totalGoods.value = filteredGoods.length
  } catch (error) {
    console.error('Failed to load goods:', error)
  } finally {
    loading.value = false
  }
}

// Debounced search
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1
  loadGoods()
}, 500)

// Refresh goods
const refreshGoods = () => {
  loadGoods()
}

// Handle page change
const handlePageChange = (page: number) => {
  currentPage.value = page
  loadGoods()
}

// Good actions
const publishGood = async (good: GoodDetailResponse) => {
  if (!good.id) return
  if (!confirm(`确定要发布商品《${good.name}》吗？`)) return
  
  try {
    await AdminService.adminUpdateGoodStatusEndpoint(good.id, { status: GOOD_STATUS.PUBLISHED })
    await loadGoods()
  } catch (error) {
    console.error('Failed to publish good:', error)
  }
}

const unpublishGood = async (good: GoodDetailResponse) => {
  if (!good.id) return
  if (!confirm(`确定要下架商品《${good.name}》吗？`)) return
  
  try {
    await AdminService.adminUpdateGoodStatusEndpoint(good.id, { status: GOOD_STATUS.DRAFT })
    await loadGoods()
  } catch (error) {
    console.error('Failed to unpublish good:', error)
  }
}

const deleteGood = async (good: GoodDetailResponse) => {
  if (!good.id) return
  if (!confirm(`确定要删除商品《${good.name}》吗？此操作不可恢复！`)) return

  try {
    await AdminService.adminDeleteGoodEndpoint(good.id)
    await loadGoods()
  } catch (error) {
    console.error('Failed to delete good:', error)
  }
}

// 审核不通过相关函数
const openRejectModal = (good: GoodDetailResponse) => {
  selectedGood.value = good
  rejectMessage.value = ''
  showRejectModal.value = true
}

const closeRejectModal = () => {
  showRejectModal.value = false
  selectedGood.value = null
  rejectMessage.value = ''
  rejecting.value = false
}

const confirmRejectGood = async () => {
  if (!selectedGood.value?.id || !rejectMessage.value.trim()) return

  rejecting.value = true
  try {
    await AdminService.adminRejectGoodEndpoint(selectedGood.value.id, {
      message: rejectMessage.value.trim()
    })
    await loadGoods()
    closeRejectModal()
  } catch (error) {
    console.error('Failed to reject good:', error)
  } finally {
    rejecting.value = false
  }
}

// Utility functions
const getStatusName = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const statusObj = goodStatuses.find(s => s.value === statusValue)
  return statusObj?.name || '未知'
}

const getStatusColorClass = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const colorMap = {
    0: 'bg-gray-100 text-gray-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-red-100 text-red-800',
    3: 'bg-orange-100 text-orange-800'
  }
  return colorMap[statusValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '-'
  return new Intl.DateTimeFormat('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(dateString))
}

// Load data on mount
onMounted(() => {
  loadGoods()
})
</script>
