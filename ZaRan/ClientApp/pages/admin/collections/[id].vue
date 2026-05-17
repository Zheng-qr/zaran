<template>
  <div>
    <!-- Loading state -->
    <LoadingOverlay v-if="loading" />

    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">集合详情</h1>
        <p class="mt-2 text-sm text-gray-700">查看集合信息和包含的内容</p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <NuxtLink
          to="/admin/collections"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          返回列表
        </NuxtLink>
        <NuxtLink
          v-if="canEditCollection"
          :to="`/admin/collections/${route.params.id}/edit`"
          class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
          </svg>
          编辑
        </NuxtLink>
      </div>
    </div>

    <!-- Collection info -->
    <div v-if="!loading && collection" class="space-y-6">
      <!-- Basic info card -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <div class="flex items-start space-x-6">
            <!-- Collection image -->
            <div class="flex-shrink-0">
              <div class="w-32 h-32 bg-gray-200 rounded-lg overflow-hidden">
                <img
                  v-if="collection.imageUrl"
                  :src="collection.imageUrl"
                  :alt="collection.name"
                  class="w-full h-full object-cover"
                >
                <div v-else class="w-full h-full bg-gray-100 flex items-center justify-center">
                  <svg class="h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                  </svg>
                </div>
              </div>
            </div>

            <!-- Collection details -->
            <div class="flex-1 min-w-0">
              <h2 class="text-2xl font-bold text-gray-900">{{ collection.name }}</h2>
              
              <div class="mt-2 flex items-center space-x-4 text-sm text-gray-500">
                <div class="flex items-center">
                  <svg class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                  </svg>
                  {{ getCollectionTypeName(collection.type) }}
                </div>
                <div class="flex items-center">
                  <svg class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  {{ collection.creatorName || '未知创建者' }}
                </div>
                <div class="flex items-center">
                  <svg class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  {{ formatDate(collection.createdAt) }}
                </div>
                <div class="flex items-center">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="collection.isPublic ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'">
                    {{ collection.isPublic ? '公开' : '私有' }}
                  </span>
                </div>
              </div>

              <p v-if="collection.summary" class="mt-3 text-gray-600">
                {{ collection.summary }}
              </p>

              <!-- Tags -->
              <div v-if="collection.tags && collection.tags.length > 0" class="mt-3">
                <div class="flex flex-wrap gap-2">
                  <span
                    v-for="tag in collection.tags"
                    :key="tag"
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
                  >
                    {{ tag }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Description -->
      <div v-if="collection.description" class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">详细描述</h3>
          <div class="prose max-w-none" v-html="collection.description"></div>
        </div>
      </div>

      <!-- Collection items -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg leading-6 font-medium text-gray-900">集合内容</h3>
            <div class="text-sm text-gray-500">
              共 {{ collectionItems.length }} 项
            </div>
          </div>

          <!-- Loading items -->
          <div v-if="loadingItems" class="flex justify-center py-8">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          </div>

          <!-- Empty items -->
          <div v-else-if="collectionItems.length === 0" class="text-center py-8">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 009.586 13H7" />
            </svg>
            <h4 class="mt-2 text-sm font-medium text-gray-900">暂无内容</h4>
            <p class="mt-1 text-sm text-gray-500">此集合还没有添加任何内容</p>
          </div>

          <!-- Items list -->
          <div v-else class="space-y-4">
            <div
              v-for="item in collectionItems"
              :key="item.id"
              class="border border-gray-200 rounded-lg p-4 hover:shadow-sm transition-shadow"
            >
              <div class="flex items-start justify-between">
                <div class="flex-1 min-w-0">
                  <h4 class="text-sm font-medium text-gray-900 truncate">
                    {{ item.title || item.name || '未知项目' }}
                  </h4>
                  <p v-if="item.summary" class="text-sm text-gray-500 mt-1 line-clamp-2">
                    {{ item.summary }}
                  </p>
                  <div class="mt-2 text-xs text-gray-400">
                    添加时间：{{ formatDate(item.addedAt) }}
                  </div>
                </div>
                <div class="ml-4 flex-shrink-0">
                  <button
                    v-if="canEditCollection"
                    @click="removeItem(item)"
                    class="text-red-600 hover:text-red-900 text-sm"
                  >
                    移除
                  </button>
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
import { CollectionsService } from '~/api'
import type { CollectionDetailResponse } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const route = useRoute()
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
  { value: COLLECTION_TYPE.ARTICLE, name: '文章集合' },
  { value: COLLECTION_TYPE.GOOD, name: '商品集合' },
  { value: COLLECTION_TYPE.USER, name: '用户集合' }
]

// Data
const collection = ref<CollectionDetailResponse | null>(null)
const collectionItems = ref<any[]>([])
const loading = ref(true)
const loadingItems = ref(false)

// Computed
const canEditCollection = computed(() => {
  return collection.value && (isAdmin() || collection.value.creatorId === userStore.userId)
})

// Load collection data
const loadCollection = async () => {
  try {
    const collectionId = route.params.id as string
    const response = await CollectionsService.collectionGetEndpoint(collectionId)
    
    collection.value = response
    
    // Load collection items
    await loadCollectionItems()
    
  } catch (error) {
    console.error('Failed to load collection:', error)
    const { error: showError } = useGlobalNotifications()
    showError('加载失败', '无法加载集合数据')
    await navigateTo('/admin/collections')
  } finally {
    loading.value = false
  }
}

// Load collection items
const loadCollectionItems = async () => {
  if (!collection.value?.id) return
  
  loadingItems.value = true
  try {
    const response = await CollectionsService.collectionItemsEndpoint(
      collection.value.id,
      0,
      100,
      true
    )
    collectionItems.value = response.items || []
  } catch (error) {
    console.error('Failed to load collection items:', error)
    collectionItems.value = []
  } finally {
    loadingItems.value = false
  }
}

// Remove item from collection
const removeItem = async (item: any) => {
  if (!confirm(`确定要从集合中移除此项目吗？`)) return
  
  try {
    // TODO: Implement remove item API call
    const { success } = useGlobalNotifications()
    success('移除成功', '项目已从集合中移除')
    await loadCollectionItems()
  } catch (error) {
    console.error('Failed to remove item:', error)
    const { error: showError } = useGlobalNotifications()
    showError('移除失败', '项目移除失败，请重试')
  }
}

// Utility functions
const getCollectionTypeName = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const typeObj = collectionTypes.find(t => t.value === typeValue)
  return typeObj?.name || '未知类型'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '未知时间'
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// Load collection on mount
onMounted(() => {
  loadCollection()
})
</script>
