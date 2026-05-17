<template>
  <div>
    <!-- Stats Overview -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <!-- User Stats - Admin Only -->
      <div v-if="isAdmin()" class="bg-white overflow-hidden shadow rounded-lg">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">总用户数</dt>
                <dd class="text-lg font-medium text-gray-900">{{ stats.userCount || 0 }}</dd>
              </dl>
            </div>
          </div>
        </div>
        <div class="bg-gray-50 px-5 py-3">
          <div class="text-sm">
            <NuxtLink to="/admin/users" class="font-medium text-blue-700 hover:text-blue-900">
              查看详情
            </NuxtLink>
          </div>
        </div>
      </div>

      <div class="bg-white overflow-hidden shadow rounded-lg">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">{{ isAdmin() ? '总文章数' : '我的文章数' }}</dt>
                <dd class="text-lg font-medium text-gray-900">{{ stats.articleCount || 0 }}</dd>
              </dl>
            </div>
          </div>
        </div>
        <div class="bg-gray-50 px-5 py-3">
          <div class="text-sm">
            <NuxtLink to="/admin/articles" class="font-medium text-blue-700 hover:text-blue-900">
              查看详情
            </NuxtLink>
          </div>
        </div>
      </div>

      <div class="bg-white overflow-hidden shadow rounded-lg">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">{{ isAdmin() ? '总商品数' : '我的商品数' }}</dt>
                <dd class="text-lg font-medium text-gray-900">{{ stats.goodCount || 0 }}</dd>
              </dl>
            </div>
          </div>
        </div>
        <div class="bg-gray-50 px-5 py-3">
          <div class="text-sm">
            <NuxtLink to="/admin/goods" class="font-medium text-blue-700 hover:text-blue-900">
              查看详情
            </NuxtLink>
          </div>
        </div>
      </div>

      <!-- Transaction Stats - Admin Only -->
      <div v-if="isAdmin()" class="bg-white overflow-hidden shadow rounded-lg">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">总交易数</dt>
                <dd class="text-lg font-medium text-gray-900">{{ stats.transactionCount || 0 }}</dd>
              </dl>
            </div>
          </div>
        </div>
        <div class="bg-gray-50 px-5 py-3">
          <div class="text-sm">
            <NuxtLink to="/admin/transactions" class="font-medium text-blue-700 hover:text-blue-900">
              查看详情
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">快速操作</h3>
          <div class="grid grid-cols-2 gap-4">
            <NuxtLink
              to="/admin/articles/create"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
            >
              创建文章
            </NuxtLink>
            <NuxtLink
              to="/admin/goods/create"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700"
            >
              创建商品
            </NuxtLink>
            <NuxtLink
              to="/admin/collections/create"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-purple-600 hover:bg-purple-700"
            >
              创建集合
            </NuxtLink>
            <NuxtLink
              to="/admin/users"
              class="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
            >
              管理用户
            </NuxtLink>
          </div>
        </div>
      </div>

      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">系统状态</h3>
          <div class="space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-sm text-gray-500">日活跃用户</span>
              <span class="text-sm font-medium text-gray-900">{{ stats.dailyActiveUsers || 0 }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-sm text-gray-500">系统状态</span>
              <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                正常运行
              </span>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-sm text-gray-500">最后更新</span>
              <span class="text-sm text-gray-500">{{ formatDate(new Date()) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Analytics Charts -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
      <!-- User Growth Chart -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">用户增长趋势</h3>
          <div class="h-64 flex items-center justify-center bg-gray-50 rounded">
            <div class="text-center">
              <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
              <p class="mt-2 text-sm text-gray-500">图表组件待集成</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Content Performance -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">内容表现</h3>
          <div class="space-y-4">
            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="h-8 w-8 bg-blue-100 rounded-full flex items-center justify-center">
                    <svg class="h-4 w-4 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                    </svg>
                  </div>
                </div>
                <div class="ml-3">
                  <p class="text-sm font-medium text-gray-900">今日发布文章</p>
                  <p class="text-sm text-gray-500">{{ contentStats.todayArticles }} 篇</p>
                </div>
              </div>
              <div class="text-sm text-green-600">+12%</div>
            </div>

            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="h-8 w-8 bg-green-100 rounded-full flex items-center justify-center">
                    <svg class="h-4 w-4 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                    </svg>
                  </div>
                </div>
                <div class="ml-3">
                  <p class="text-sm font-medium text-gray-900">今日上架商品</p>
                  <p class="text-sm text-gray-500">{{ contentStats.todayGoods }} 件</p>
                </div>
              </div>
              <div class="text-sm text-green-600">+8%</div>
            </div>

            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="h-8 w-8 bg-purple-100 rounded-full flex items-center justify-center">
                    <svg class="h-4 w-4 text-purple-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                  </div>
                </div>
                <div class="ml-3">
                  <p class="text-sm font-medium text-gray-900">今日交易</p>
                  <p class="text-sm text-gray-500">{{ contentStats.todayTransactions }} 笔</p>
                </div>
              </div>
              <div class="text-sm text-green-600">+15%</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">最近活动</h3>
        <div class="flow-root">
          <ul class="-mb-8">
            <li v-for="(activity, index) in recentActivities" :key="index">
              <div class="relative pb-8" :class="{ 'pb-0': index === recentActivities.length - 1 }">
                <span
                  v-if="index !== recentActivities.length - 1"
                  class="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200"
                  aria-hidden="true"
                />
                <div class="relative flex space-x-3">
                  <div>
                    <span class="h-8 w-8 rounded-full flex items-center justify-center ring-8 ring-white"
                          :class="activity.iconBg">
                      <svg class="h-5 w-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="activity.iconPath" />
                      </svg>
                    </span>
                  </div>
                  <div class="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                    <div>
                      <p class="text-sm text-gray-500">{{ activity.content }}</p>
                    </div>
                    <div class="text-right text-sm whitespace-nowrap text-gray-500">
                      {{ activity.time }}
                    </div>
                  </div>
                </div>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { AdminService } from '~/api'

// Set layout
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { isAdmin } = usePermissions()

// Stats data
const stats = ref({
  userCount: 0,
  articleCount: 0,
  goodCount: 0,
  transactionCount: 0,
  dailyActiveUsers: 0
})

// Content statistics
const contentStats = ref({
  todayArticles: 12,
  todayGoods: 8,
  todayTransactions: 25
})

// Recent activities (mock data for now)
const recentActivities = ref([
  {
    content: '新用户注册',
    time: '2分钟前',
    iconPath: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z',
    iconBg: 'bg-blue-500'
  },
  {
    content: '新文章发布',
    time: '5分钟前',
    iconPath: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z',
    iconBg: 'bg-green-500'
  },
  {
    content: '商品状态更新',
    time: '10分钟前',
    iconPath: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z',
    iconBg: 'bg-purple-500'
  }
])

// Load statistics
const loadStats = async () => {
  try {
    const response = await AdminService.adminGetStatisticsEndpoint()
    stats.value = response
  } catch (error) {
    console.error('Failed to load statistics:', error)
  }
}

// Format date
const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

// Load data on mount
onMounted(() => {
  loadStats()
})
</script>
