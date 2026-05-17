<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">收藏品管理</h1>
        <p class="mt-2 text-sm text-gray-700">查看和管理用户的收藏记录</p>
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
              placeholder="搜索商品名称、用户..."
            >
          </div>

          <!-- Good ID -->
          <div>
            <label for="goodId" class="block text-sm font-medium text-gray-700">商品ID</label>
            <input
              id="goodId"
              v-model="filters.goodId"
              type="text"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="输入商品ID"
            >
          </div>

          <!-- User -->
          <div>
            <label for="user" class="block text-sm font-medium text-gray-700">用户</label>
            <input
              id="user"
              v-model="filters.user"
              type="text"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="用户名或ID"
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
              <option :value="true">最新收藏</option>
              <option :value="false">最早收藏</option>
            </select>
          </div>
        </div>

        <!-- Search button -->
        <div class="mt-4">
          <button
            @click="searchCollects"
            :disabled="!filters.goodId"
            class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            搜索收藏记录
          </button>
          <p class="mt-1 text-sm text-gray-500">请输入商品ID来查看该商品的收藏记录</p>
        </div>
      </div>
    </div>

    <!-- Collects list -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <!-- Loading state -->
        <div v-if="loading" class="flex justify-center py-8">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>

        <!-- Empty state -->
        <div v-else-if="collects.length === 0 && !hasSearched" class="text-center py-8">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">请输入商品ID进行搜索</h3>
          <p class="mt-1 text-sm text-gray-500">输入商品ID来查看该商品的收藏记录</p>
        </div>

        <!-- No results -->
        <div v-else-if="collects.length === 0 && hasSearched" class="text-center py-8">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">暂无收藏记录</h3>
          <p class="mt-1 text-sm text-gray-500">该商品还没有被任何用户收藏</p>
        </div>

        <!-- Collects table -->
        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  用户信息
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  商品信息
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  收藏时间
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  收藏顺序
                </th>
                <th v-if="isAdmin()" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  操作
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="collect in collects" :key="collect.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10">
                      <img
                        v-if="collect.user?.avatar"
                        :src="collect.user.avatar"
                        :alt="collect.user.nickname"
                        class="h-10 w-10 rounded-full object-cover"
                      >
                      <div v-else class="h-10 w-10 rounded-full bg-gray-300 flex items-center justify-center">
                        <svg class="h-6 w-6 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                        </svg>
                      </div>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900">
                        {{ collect.user?.nickname || '未知用户' }}
                      </div>
                      <div class="text-sm text-gray-500">
                        ID: {{ collect.userId }}
                      </div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">
                    商品ID: {{ collect.goodId }}
                  </div>
                  <div v-if="currentGoodInfo" class="text-sm text-gray-500">
                    {{ currentGoodInfo.name }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(collect.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  #{{ collect.index }}
                </td>
                <td v-if="isAdmin()" class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <NuxtLink
                      :to="`/admin/users/${collect.userId}`"
                      class="text-blue-600 hover:text-blue-900"
                    >
                      查看用户
                    </NuxtLink>
                    <NuxtLink
                      :to="`/admin/goods/${collect.goodId}`"
                      class="text-green-600 hover:text-green-900"
                    >
                      查看商品
                    </NuxtLink>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <div v-if="totalCollects > pageSize" class="mt-6 flex items-center justify-between">
          <div class="text-sm text-gray-700">
            显示第 {{ (currentPage - 1) * pageSize + 1 }} - {{ Math.min(currentPage * pageSize, totalCollects) }} 条，
            共 {{ totalCollects }} 条记录
          </div>
          <div class="flex space-x-2">
            <button
              :disabled="currentPage === 1"
              @click="currentPage--; searchCollects()"
              class="px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              上一页
            </button>
            <button
              :disabled="currentPage * pageSize >= totalCollects"
              @click="currentPage++; searchCollects()"
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
import { GoodsService } from '~/api'
import type { GoodCollectsListEndpointResponse, GoodDetailResponse } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { createPaginationParams } = useApi()
const { isAdmin } = usePermissions()

// Data
const collects = ref<GoodCollectsListEndpointResponse[]>([])
const currentGoodInfo = ref<GoodDetailResponse | null>(null)
const loading = ref(false)
const hasSearched = ref(false)
const currentPage = ref(1)
const pageSize = ref(20)
const totalCollects = ref(0)

// Filters
const filters = ref({
  keyword: '',
  goodId: '',
  user: '',
  desc: true
})

// Search collects for a specific good
const searchCollects = async () => {
  if (!filters.value.goodId) {
    const { error } = useGlobalNotifications()
    error('请输入商品ID', '需要输入商品ID才能查看收藏记录')
    return
  }

  loading.value = true
  hasSearched.value = true
  
  try {
    const params = createPaginationParams(
      currentPage.value,
      pageSize.value,
      filters.value.desc
    )
    
    // Get collects for the specific good
    const response = await GoodsService.goodCollectsListEndpoint(
      filters.value.goodId,
      params.offset,
      params.limit,
      params.desc
    )
    
    collects.value = response.items || []
    totalCollects.value = response.total || 0
    
    // Get good info
    try {
      const goodResponse = await GoodsService.goodGetEndpoint(filters.value.goodId)
      currentGoodInfo.value = goodResponse
    } catch (error) {
      console.error('Failed to load good info:', error)
      currentGoodInfo.value = null
    }
    
    // Filter by user if specified
    if (filters.value.user) {
      collects.value = collects.value.filter(collect => 
        collect.user?.nickname?.includes(filters.value.user) ||
        collect.userId?.includes(filters.value.user)
      )
    }
    
  } catch (error) {
    console.error('Failed to load collects:', error)
    const { error: showError } = useGlobalNotifications()
    showError('加载失败', '无法加载收藏记录，请检查商品ID是否正确')
    collects.value = []
    totalCollects.value = 0
    currentGoodInfo.value = null
  } finally {
    loading.value = false
  }
}

// Reset search when good ID changes
watch(() => filters.value.goodId, () => {
  if (!filters.value.goodId) {
    collects.value = []
    totalCollects.value = 0
    currentGoodInfo.value = null
    hasSearched.value = false
    currentPage.value = 1
  }
})

// Utility functions
const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '未知时间'
  return new Date(dateString).toLocaleString('zh-CN')
}
</script>
