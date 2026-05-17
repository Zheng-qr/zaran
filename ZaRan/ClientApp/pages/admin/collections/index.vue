<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ isAdmin() ? '集合管理' : '我的集合' }}</h1>
        <p class="mt-2 text-sm text-gray-700">
          {{ isAdmin() ? '管理所有集合内容' : '管理您创建的集合' }}
        </p>
      </div>
      <div class="mt-4 sm:mt-0">
        <NuxtLink
          to="/admin/collections/create"
          class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
          </svg>
          创建集合
        </NuxtLink>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white shadow rounded-lg mb-6">
      <div class="px-4 py-5 sm:p-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <!-- Search -->
          <div>
            <label for="search" class="block text-sm font-medium text-gray-700">搜索</label>
            <input
              id="search"
              v-model="filters.keyword"
              type="text"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="搜索集合名称、描述..."
            >
          </div>

          <!-- Type -->
          <div>
            <label for="type" class="block text-sm font-medium text-gray-700">集合类型</label>
            <select
              id="type"
              v-model="filters.type"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">全部类型</option>
              <option v-for="type in collectionTypes" :key="type.value" :value="type.value">
                {{ type.name }}
              </option>
            </select>
          </div>

          <!-- Creator (Admin only) -->
          <div v-if="isAdmin()">
            <label for="creator" class="block text-sm font-medium text-gray-700">创建者</label>
            <input
              id="creator"
              v-model="filters.creator"
              type="text"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="创建者用户名"
            >
          </div>

          <!-- Sort -->
          <div>
            <label for="sort" class="block text-sm font-medium text-gray-700">排序</label>
            <select
              id="sort"
              v-model="filters.desc"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option :value="true">最新创建</option>
              <option :value="false">最早创建</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Collections list -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <!-- Loading state -->
        <div v-if="loading" class="flex justify-center py-8">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>

        <!-- Empty state -->
        <div v-else-if="collections.length === 0" class="text-center py-8">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">暂无集合</h3>
          <p class="mt-1 text-sm text-gray-500">开始创建您的第一个集合吧</p>
          <div class="mt-6">
            <NuxtLink
              to="/admin/collections/create"
              class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
            >
              <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
              </svg>
              创建集合
            </NuxtLink>
          </div>
        </div>

        <!-- Collections grid -->
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div
            v-for="collection in collections"
            :key="collection.id"
            class="border border-gray-200 rounded-lg overflow-hidden hover:shadow-md transition-shadow"
          >
            <!-- Collection image -->
            <div class="aspect-w-16 aspect-h-9 bg-gray-200">
              <img
                v-if="collection.imageUrl"
                :src="collection.imageUrl"
                :alt="collection.name"
                class="w-full h-48 object-cover"
              >
              <div v-else class="w-full h-48 bg-gray-100 flex items-center justify-center">
                <svg class="h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                </svg>
              </div>
            </div>

            <!-- Collection info -->
            <div class="p-4">
              <div class="flex items-start justify-between">
                <div class="flex-1 min-w-0">
                  <h3 class="text-lg font-medium text-gray-900 truncate">
                    {{ collection.name }}
                  </h3>
                  <p class="text-sm text-gray-500 mt-1">
                    {{ getCollectionTypeName(collection.type) }}
                  </p>
                  <p v-if="collection.summary" class="text-sm text-gray-600 mt-2 line-clamp-2">
                    {{ collection.summary }}
                  </p>
                </div>
              </div>

              <!-- Collection meta -->
              <div class="mt-4 flex items-center justify-between text-sm text-gray-500">
                <div class="flex items-center">
                  <svg class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  {{ collection.creatorName || '未知' }}
                </div>
                <div class="flex items-center">
                  <svg class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  {{ formatDate(collection.createdAt) }}
                </div>
              </div>

              <!-- Actions -->
              <div class="mt-4 flex space-x-2">
                <NuxtLink
                  :to="`/admin/collections/${collection.id}`"
                  class="flex-1 text-center px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  查看
                </NuxtLink>
                <NuxtLink
                  v-if="canEditCollection(collection)"
                  :to="`/admin/collections/${collection.id}/edit`"
                  class="flex-1 text-center px-3 py-2 border border-transparent rounded-md text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
                >
                  编辑
                </NuxtLink>
                <button
                  v-if="canDeleteCollection(collection)"
                  @click="deleteCollection(collection)"
                  class="px-3 py-2 border border-transparent rounded-md text-sm font-medium text-white bg-red-600 hover:bg-red-700"
                >
                  删除
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <div v-if="totalCollections > pageSize" class="mt-6 flex items-center justify-between">
          <div class="text-sm text-gray-700">
            显示第 {{ (currentPage - 1) * pageSize + 1 }} - {{ Math.min(currentPage * pageSize, totalCollections) }} 条，
            共 {{ totalCollections }} 条记录
          </div>
          <div class="flex space-x-2">
            <button
              :disabled="currentPage === 1"
              @click="currentPage--; loadCollections()"
              class="px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              上一页
            </button>
            <button
              :disabled="currentPage * pageSize >= totalCollections"
              @click="currentPage++; loadCollections()"
              class="px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              下一页
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { CollectionsService } from '~/api'
import type { CollectionDetailResponse } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { createPaginationParams } = useApi()
const { isAdmin } = usePermissions()
const userStore = useUserStore()

// Collection constants
const COLLECTION_TYPE = {
  UNSPECIFIED: 0,
  ARTICLE: 1,
  GOOD: 2,
  USER: 3
}

// Available options
const collectionTypes = [
  { value: 'article', name: '文章集合' },
  { value: 'good', name: '商品集合' },
  { value: 'user', name: '用户集合' }
]

// Data
const collections = ref<CollectionDetailResponse[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(12)
const totalCollections = ref(0)

// Filters
const filters = ref({
  keyword: '',
  type: '',
  creator: '',
  desc: true
})

// Utility function for debouncing
const useDebounceFn = (fn: (...args: unknown[]) => void, delay: number) => {
  let timeoutId: NodeJS.Timeout
  return (...args: unknown[]) => {
    clearTimeout(timeoutId)
    timeoutId = setTimeout(() => fn(...args), delay)
  }
}

// Load collections
const loadCollections = async () => {
  loading.value = true
  
  try {
    const params = createPaginationParams(
      currentPage.value,
      pageSize.value,
      filters.value.desc,
      filters.value.keyword || undefined
    )
    
    // If user is not admin, only show their own collections
    const creatorId = isAdmin() ? undefined : userStore.userId
    
    const response = await CollectionsService.collectionListEndpoint(
      params.offset,
      params.limit,
      params.desc,
      filters.value.type || undefined,
      creatorId,
      params.keyword
    )
    
    collections.value = response.items || []
    totalCollections.value = response.total || 0
  } catch (error) {
    console.error('Failed to load collections:', error)
  } finally {
    loading.value = false
  }
}

// Debounced search
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1
  loadCollections()
}, 500)

// Watch filters
watch(() => filters.value.keyword, debouncedSearch)
watch(() => filters.value.type, () => {
  currentPage.value = 1
  loadCollections()
})
watch(() => filters.value.creator, debouncedSearch)
watch(() => filters.value.desc, () => {
  currentPage.value = 1
  loadCollections()
})

// Permission checks
const canEditCollection = (collection: CollectionDetailResponse) => {
  return isAdmin() || collection.creatorId === userStore.userId
}

const canDeleteCollection = (collection: CollectionDetailResponse) => {
  return isAdmin() || collection.creatorId === userStore.userId
}

// Collection actions
const deleteCollection = async (collection: CollectionDetailResponse) => {
  if (!collection.id) return
  if (!confirm(`确定要删除集合《${collection.name}》吗？此操作不可恢复。`)) return
  
  try {
    await CollectionsService.collectionDeleteEndpoint(collection.id)
    const { success } = useGlobalNotifications()
    success('删除成功', '集合已成功删除')
    loadCollections()
  } catch (error) {
    console.error('Failed to delete collection:', error)
    const { error: showError } = useGlobalNotifications()
    showError('删除失败', '集合删除失败，请重试')
  }
}

// Utility functions
const getCollectionTypeName = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const typeObj = collectionTypes.find(t => t.value === typeValue.toString())
  return typeObj?.name || '未知类型'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '未知时间'
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// Load collections on mount
onMounted(() => {
  loadCollections()
})
</script>
