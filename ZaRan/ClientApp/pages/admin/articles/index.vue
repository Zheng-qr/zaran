<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">文章管理</h1>
        <p class="mt-2 text-sm text-gray-700">管理系统中的所有文章内容</p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <button
          @click="refreshArticles"
          :disabled="loading"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          刷新
        </button>
        <NuxtLink
          to="/admin/articles/create"
          class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          创建文章
        </NuxtLink>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white shadow rounded-lg mb-6">
      <div class="px-4 py-5 sm:p-6">
        <div class="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div>
            <label for="search" class="block text-sm font-medium text-gray-700">搜索文章</label>
            <input
              id="search"
              v-model="filters.keyword"
              type="text"
              placeholder="标题或内容"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @input="debouncedSearch"
            >
          </div>
          
          <div>
            <label for="type" class="block text-sm font-medium text-gray-700">文章类型</label>
            <select
              id="type"
              v-model="filters.type"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadArticles"
            >
              <option value="">所有类型</option>
              <option v-for="type in articleTypes" :key="type.value" :value="type.value">
                {{ type.name }}
              </option>
            </select>
          </div>
          
          <div>
            <label for="status" class="block text-sm font-medium text-gray-700">文章状态</label>
            <select
              id="status"
              v-model="filters.status"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadArticles"
            >
              <option value="">所有状态</option>
              <option v-for="status in articleStatuses" :key="status.value" :value="status.value">
                {{ status.name }}
              </option>
            </select>
          </div>
          
          <div>
            <label for="author" class="block text-sm font-medium text-gray-700">作者筛选</label>
            <input
              id="author"
              v-model="filters.author"
              type="text"
              placeholder="作者昵称"
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
              @change="loadArticles"
            >
              <option :value="true">最新发布</option>
              <option :value="false">最早发布</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Articles table -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <LoadingOverlay v-if="loading" />
        
        <div v-else-if="articles.length === 0" class="text-center py-12">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">没有找到文章</h3>
          <p class="mt-1 text-sm text-gray-500">尝试调整搜索条件或筛选器</p>
        </div>
        
        <div v-else class="overflow-hidden">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  文章信息
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  类型
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  状态
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  作者
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
              <tr v-for="article in articles" :key="article.id" class="hover:bg-gray-50">
                <td class="px-6 py-4">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-16 w-16">
                      <img 
                        class="h-16 w-16 rounded-lg object-cover" 
                        :src="article.imageSmallUrl || article.imageUrl || '/default-article.png'" 
                        :alt="article.title"
                      >
                    </div>
                    <div class="ml-4 flex-1">
                      <div class="text-sm font-medium text-gray-900 line-clamp-2">{{ article.title }}</div>
                      <div class="text-sm text-gray-500 line-clamp-2 mt-1">{{ article.summary || '暂无摘要' }}</div>
                      <div class="flex flex-wrap gap-1 mt-2">
                        <span 
                          v-for="tag in (article.tags || []).slice(0, 3)" 
                          :key="tag"
                          class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-800"
                        >
                          {{ tag }}
                        </span>
                        <span 
                          v-if="(article.tags || []).length > 3"
                          class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-800"
                        >
                          +{{ (article.tags || []).length - 3 }}
                        </span>
                      </div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getTypeColorClass(article.type)">
                    {{ getTypeName(article.type) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusColorClass(article.status)">
                    {{ getStatusName(article.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-8 w-8">
                      <img 
                        class="h-8 w-8 rounded-full" 
                        :src="article.author?.avatar || '/default-avatar.png'" 
                        :alt="article.author?.nickname"
                      >
                    </div>
                    <div class="ml-3">
                      <div class="text-sm font-medium text-gray-900">{{ article.author?.nickname }}</div>
                      <div class="text-sm text-gray-500">@{{ article.author?.username }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(article.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <NuxtLink
                      :to="`/admin/articles/${article.id}`"
                      class="text-blue-600 hover:text-blue-900"
                    >
                      查看
                    </NuxtLink>
                    <NuxtLink
                      :to="`/admin/articles/${article.id}/edit`"
                      class="text-indigo-600 hover:text-indigo-900"
                    >
                      编辑
                    </NuxtLink>
                    <button
                      v-if="article.status === ARTICLE_STATUS.DRAFT"
                      @click="publishArticle(article)"
                      class="text-green-600 hover:text-green-900"
                    >
                      发布
                    </button>
                    <button
                      v-else-if="article.status === ARTICLE_STATUS.PUBLISHED"
                      @click="archiveArticle(article)"
                      class="text-yellow-600 hover:text-yellow-900"
                    >
                      归档
                    </button>
                    <button
                      v-if="article.status === ARTICLE_STATUS.DRAFT || article.status === ARTICLE_STATUS.PUBLISHED"
                      @click="openRejectModal(article)"
                      class="text-orange-600 hover:text-orange-900"
                    >
                      审核不通过
                    </button>
                    <button
                      @click="deleteArticle(article)"
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
          v-if="articles.length > 0"
          :current-page="currentPage"
          :total-items="totalArticles"
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
              文章：<span class="font-medium">{{ selectedArticle?.title }}</span>
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
              @click="confirmRejectArticle"
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
import { ArticlesService, AdminService } from '~/api'
import type { ArticleDetailResponse } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { executeApi, createPaginationParams } = useApi()
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

// Article constants
const ARTICLE_STATUS = {
  DRAFT: 0,
  PUBLISHED: 1,
  BANNED: 2,
  ARCHIVED: 3,
  REJECTED: 4
}

const ARTICLE_TYPE = {
  STORY: 0,
  CHARACTER: 1,
  GENE: 2,
  WIKI: 3,
  POST: 4,
  PATTERN: 5
}

// Data
const articles = ref<ArticleDetailResponse[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(20)
const totalArticles = ref(0)

// 审核不通过相关状态
const showRejectModal = ref(false)
const selectedArticle = ref<ArticleDetailResponse | null>(null)
const rejectMessage = ref('')
const rejecting = ref(false)

// Filters
const filters = ref({
  keyword: '',
  type: '',
  status: '',
  author: '',
  desc: true
})

// Available options
const articleTypes = [
  { value: ARTICLE_TYPE.STORY, name: '故事' },
  { value: ARTICLE_TYPE.CHARACTER, name: '角色介绍' },
  { value: ARTICLE_TYPE.GENE, name: '基因库' },
  { value: ARTICLE_TYPE.WIKI, name: '百科' },
  { value: ARTICLE_TYPE.POST, name: '社区帖子' },
  { value: ARTICLE_TYPE.PATTERN, name: '纹样' }
]

const articleStatuses = [
  { value: ARTICLE_STATUS.DRAFT, name: '草稿' },
  { value: ARTICLE_STATUS.PUBLISHED, name: '已发布' },
  { value: ARTICLE_STATUS.BANNED, name: '已封禁' },
  { value: ARTICLE_STATUS.ARCHIVED, name: '已归档' },
  { value: ARTICLE_STATUS.REJECTED, name: '审核不通过' }
]

// Load articles
const loadArticles = async () => {
  loading.value = true

  try {
    // For better performance, we'll load more articles and filter on frontend
    // This is a temporary solution until backend supports more filtering options
    const params = createPaginationParams(
      1, // Always start from page 1 to get all articles
      100, // Load more articles to allow frontend filtering
      filters.value.desc,
      filters.value.keyword || undefined
    )

    // If user is not admin, only show their own articles
    const userId = isAdmin() ? undefined : userStore.userId

    // Determine article type from filters, default to 'all' for admin panel
    const articleType = filters.value.type !== '' ? filters.value.type.toString() : 'all'

    // Debug logging
    console.log('筛选参数:', {
      type: filters.value.type,
      articleType,
      status: filters.value.status,
      author: filters.value.author,
      keyword: filters.value.keyword
    })

    // Use ArticlesService to get articles with proper type filtering
    const response = await ArticlesService.articleListEndpoint(
      articleType,
      params.offset,
      params.limit,
      params.desc,
      userId,
      params.keyword
    )

    console.log('API响应:', response)

    let filteredArticles = response.items || []

    // Apply frontend filters for status and author
    if (filters.value.status !== '') {
      const statusFilter = parseInt(filters.value.status)
      filteredArticles = filteredArticles.filter(article => article.status === statusFilter)
    }

    if (filters.value.author) {
      const authorFilter = filters.value.author.toLowerCase()
      filteredArticles = filteredArticles.filter(article =>
        article.author?.nickname?.toLowerCase().includes(authorFilter) ||
        article.author?.username?.toLowerCase().includes(authorFilter)
      )
    }

    // Apply pagination to filtered results
    const startIndex = (currentPage.value - 1) * pageSize.value
    const endIndex = startIndex + pageSize.value
    articles.value = filteredArticles.slice(startIndex, endIndex)
    totalArticles.value = filteredArticles.length
  } catch (error) {
    console.error('Failed to load articles:', error)
  } finally {
    loading.value = false
  }
}

// Debounced search
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1
  loadArticles()
}, 500)

// Refresh articles
const refreshArticles = () => {
  loadArticles()
}

// Handle page change
const handlePageChange = (page: number) => {
  currentPage.value = page
  loadArticles()
}

// Article actions
const publishArticle = async (article: ArticleDetailResponse) => {
  if (!article.id) return
  if (!confirm(`确定要发布文章《${article.title}》吗？`)) return

  try {
    await AdminService.adminUpdateArticleStatusEndpoint(article.id, { status: ARTICLE_STATUS.PUBLISHED })
    await loadArticles()
  } catch (error) {
    console.error('Failed to publish article:', error)
  }
}

const archiveArticle = async (article: ArticleDetailResponse) => {
  if (!article.id) return
  if (!confirm(`确定要归档文章《${article.title}》吗？`)) return

  try {
    await AdminService.adminUpdateArticleStatusEndpoint(article.id, { status: ARTICLE_STATUS.ARCHIVED })
    await loadArticles()
  } catch (error) {
    console.error('Failed to archive article:', error)
  }
}

const deleteArticle = async (article: ArticleDetailResponse) => {
  if (!article.id) return
  if (!confirm(`确定要删除文章《${article.title}》吗？此操作不可恢复！`)) return

  try {
    await AdminService.adminDeleteArticleEndpoint(article.id)
    await loadArticles()
  } catch (error) {
    console.error('Failed to delete article:', error)
  }
}

// 审核不通过相关函数
const openRejectModal = (article: ArticleDetailResponse) => {
  selectedArticle.value = article
  rejectMessage.value = ''
  showRejectModal.value = true
}

const closeRejectModal = () => {
  showRejectModal.value = false
  selectedArticle.value = null
  rejectMessage.value = ''
  rejecting.value = false
}

const confirmRejectArticle = async () => {
  if (!selectedArticle.value?.id || !rejectMessage.value.trim()) return

  rejecting.value = true
  try {
    await AdminService.adminRejectArticleEndpoint(selectedArticle.value.id, {
      message: rejectMessage.value.trim()
    })
    await loadArticles()
    closeRejectModal()
  } catch (error) {
    console.error('Failed to reject article:', error)
  } finally {
    rejecting.value = false
  }
}

// Utility functions
const getTypeName = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const typeObj = articleTypes.find(t => t.value === typeValue)
  return typeObj?.name || '未知'
}

const getStatusName = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const statusObj = articleStatuses.find(s => s.value === statusValue)
  return statusObj?.name || '未知'
}

const getTypeColorClass = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const colorMap = {
    0: 'bg-blue-100 text-blue-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-purple-100 text-purple-800',
    3: 'bg-yellow-100 text-yellow-800',
    4: 'bg-pink-100 text-pink-800'
  }
  return colorMap[typeValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const getStatusColorClass = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const colorMap = {
    0: 'bg-gray-100 text-gray-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-red-100 text-red-800',
    3: 'bg-yellow-100 text-yellow-800',
    4: 'bg-orange-100 text-orange-800'
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
  loadArticles()
})
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
