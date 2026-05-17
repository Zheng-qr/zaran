<template>
  <div>
    <LoadingOverlay v-if="loading" />
    
    <div v-else-if="good" class="space-y-6">
      <!-- Header -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <div class="flex items-center justify-between">
            <div class="flex-1">
              <h1 class="text-2xl font-bold text-gray-900">{{ good.name }}</h1>
              <div class="flex items-center space-x-4 mt-2">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="getStatusColorClass(good.status)">
                  {{ getStatusName(good.status) }}
                </span>
                <span class="text-sm text-gray-500">
                  发布于 {{ formatDate(good.createdAt) }}
                </span>
              </div>
            </div>
            <div class="flex space-x-3">
              <NuxtLink
                :to="`/admin/goods/${good.id}/edit`"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
              >
                编辑商品
              </NuxtLink>
              <NuxtLink
                to="/admin/goods"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
              >
                返回列表
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>

      <!-- Good Content -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Main Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Good Image -->
          <div v-if="good.imageUrl" class="bg-white shadow rounded-lg overflow-hidden">
            <img 
              :src="good.imageUrl" 
              :alt="good.name"
              class="w-full h-64 object-cover"
            >
          </div>

          <!-- Good Description -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">商品描述</h3>
              <div v-if="good.relatedArticle" class="prose max-w-none">
                <div v-if="good.relatedArticle.summary" class="text-gray-600 italic mb-4">
                  {{ good.relatedArticle.summary }}
                </div>
                <div v-html="good.relatedArticle.body || '暂无描述'" class="text-gray-900"></div>
              </div>
              <div v-else class="text-gray-500">
                暂无商品描述
              </div>
            </div>
          </div>

          <!-- Related Article -->
          <div v-if="good.relatedArticle" class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">关联文章</h3>
              <div class="flex items-center space-x-4">
                <img 
                  class="h-16 w-16 rounded-lg object-cover" 
                  :src="good.relatedArticle.imageSmallUrl || good.relatedArticle.imageUrl || '/default-article.png'" 
                  :alt="good.relatedArticle.title"
                >
                <div class="flex-1">
                  <h4 class="text-sm font-medium text-gray-900">{{ good.relatedArticle.title }}</h4>
                  <p class="text-sm text-gray-500 mt-1">{{ good.relatedArticle.summary || '暂无摘要' }}</p>
                  <div class="flex items-center space-x-2 mt-2">
                    <NuxtLink
                      :to="`/admin/articles/${good.relatedArticle.id}`"
                      class="text-blue-600 hover:text-blue-900 text-sm"
                    >
                      查看文章
                    </NuxtLink>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="space-y-6">
          <!-- Publisher Info -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">发布者信息</h3>
              <div class="flex items-center space-x-3">
                <img 
                  class="h-12 w-12 rounded-full" 
                  :src="good.publisher?.avatar || '/default-avatar.png'" 
                  :alt="good.publisher?.nickname"
                >
                <div>
                  <p class="text-sm font-medium text-gray-900">{{ good.publisher?.nickname }}</p>
                  <p class="text-sm text-gray-500">@{{ good.publisher?.username }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Good Details -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">商品详情</h3>
              <dl class="space-y-3">
                <div>
                  <dt class="text-sm font-medium text-gray-500">价格</dt>
                  <dd class="text-sm text-gray-900">¥{{ good.price?.toFixed(2) || '0.00' }}</dd>
                </div>
                <div v-if="good.discountedPrice && good.discountedPrice !== good.price">
                  <dt class="text-sm font-medium text-gray-500">折扣价</dt>
                  <dd class="text-sm text-red-600">¥{{ good.discountedPrice.toFixed(2) }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">库存</dt>
                  <dd class="text-sm text-gray-900">
                    <span v-if="good.stoke === -1" class="text-green-600">无限库存</span>
                    <span v-else-if="good.stoke === 0" class="text-red-600">缺货</span>
                    <span v-else-if="good.stoke && good.stoke < 10" class="text-yellow-600">{{ good.stoke }} 件</span>
                    <span v-else class="text-green-600">{{ good.stoke }} 件</span>
                  </dd>
                </div>
                <div v-if="good.copyrightInfo">
                  <dt class="text-sm font-medium text-gray-500">版权信息</dt>
                  <dd class="text-sm text-gray-900">
                    <div class="flex items-start space-x-2">
                      <i class="fas fa-copyright text-blue-600 mt-0.5"></i>
                      <span>{{ good.copyrightInfo }}</span>
                    </div>
                  </dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">创建时间</dt>
                  <dd class="text-sm text-gray-900">{{ formatDate(good.createdAt) }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">更新时间</dt>
                  <dd class="text-sm text-gray-900">{{ formatDate(good.updatedAt) }}</dd>
                </div>
              </dl>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">快速操作</h3>
              <div class="space-y-3">
                <button
                  v-if="good.status === GOOD_STATUS.DRAFT"
                  @click="publishGood"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-green-300 rounded-md shadow-sm text-sm font-medium text-green-700 bg-white hover:bg-green-50"
                >
                  发布商品
                </button>
                <button
                  v-else-if="good.status === GOOD_STATUS.PUBLISHED"
                  @click="unpublishGood"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-yellow-300 rounded-md shadow-sm text-sm font-medium text-yellow-700 bg-white hover:bg-yellow-50"
                >
                  下架商品
                </button>
                <button
                  @click="deleteGood"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-red-300 rounded-md shadow-sm text-sm font-medium text-red-700 bg-white hover:bg-red-50"
                >
                  删除商品
                </button>
                <NuxtLink
                  :to="`/shop/${good.id}`"
                  target="_blank"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  前台预览
                </NuxtLink>
              </div>
            </div>
          </div>

          <!-- Sales Statistics -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">销售统计</h3>
              <div class="space-y-3">
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500">总销量</span>
                  <span class="text-sm font-medium text-gray-900">{{ salesStats.totalSales }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500">总收入</span>
                  <span class="text-sm font-medium text-gray-900">¥{{ salesStats.totalRevenue.toFixed(2) }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500">本月销量</span>
                  <span class="text-sm font-medium text-gray-900">{{ salesStats.monthlySales }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500">收藏数</span>
                  <span class="text-sm font-medium text-gray-900">{{ salesStats.favorites }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-12">
      <h3 class="mt-2 text-sm font-medium text-gray-900">商品不存在</h3>
      <p class="mt-1 text-sm text-gray-500">请检查商品ID是否正确</p>
      <div class="mt-6">
        <NuxtLink
          to="/admin/goods"
          class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
        >
          返回商品列表
        </NuxtLink>
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

const route = useRoute()
const router = useRouter()

// Good constants
const GOOD_STATUS = {
  DRAFT: 0,
  PUBLISHED: 1,
  BANNED: 2
}

// Data
const good = ref<GoodDetailResponse | null>(null)
const loading = ref(false)

// Mock sales statistics (replace with real API calls)
const salesStats = ref({
  totalSales: 0,
  totalRevenue: 0,
  monthlySales: 0,
  favorites: 0
})

// Available options
const goodStatuses = [
  { value: GOOD_STATUS.DRAFT, name: '草稿' },
  { value: GOOD_STATUS.PUBLISHED, name: '已发布' },
  { value: GOOD_STATUS.BANNED, name: '已封禁' }
]

// Load good data
const loadGood = async () => {
  const goodId = route.params.id as string
  if (!goodId) return

  loading.value = true
  
  try {
    good.value = await GoodsService.goodGetEndpoint(goodId)
  } catch (error) {
    console.error('Failed to load good:', error)
    good.value = null
  } finally {
    loading.value = false
  }
}

// Good actions
const publishGood = async () => {
  if (!good.value?.id) return
  if (!confirm(`确定要发布商品《${good.value.name}》吗？`)) return
  
  try {
    await AdminService.adminUpdateGoodStatusEndpoint(good.value.id, { status: GOOD_STATUS.PUBLISHED })
    await loadGood()
  } catch (error) {
    console.error('Failed to publish good:', error)
  }
}

const unpublishGood = async () => {
  if (!good.value?.id) return
  if (!confirm(`确定要下架商品《${good.value.name}》吗？`)) return
  
  try {
    await AdminService.adminUpdateGoodStatusEndpoint(good.value.id, { status: GOOD_STATUS.DRAFT })
    await loadGood()
  } catch (error) {
    console.error('Failed to unpublish good:', error)
  }
}

const deleteGood = async () => {
  if (!good.value?.id) return
  if (!confirm(`确定要删除商品《${good.value.name}》吗？此操作不可恢复！`)) return
  
  try {
    await AdminService.adminDeleteGoodEndpoint(good.value.id)
    await router.push('/admin/goods')
  } catch (error) {
    console.error('Failed to delete good:', error)
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
    2: 'bg-red-100 text-red-800'
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
  loadGood()
})
</script>

<style scoped>
.prose {
  max-width: none;
}

.prose h1, .prose h2, .prose h3, .prose h4, .prose h5, .prose h6 {
  margin-top: 1.5em;
  margin-bottom: 0.5em;
  font-weight: 600;
}

.prose p {
  margin-bottom: 1em;
  line-height: 1.6;
}

.prose ul, .prose ol {
  margin-bottom: 1em;
  padding-left: 1.5em;
}

.prose blockquote {
  border-left: 4px solid #e5e7eb;
  padding-left: 1em;
  margin: 1em 0;
  font-style: italic;
  color: #6b7280;
}
</style>
