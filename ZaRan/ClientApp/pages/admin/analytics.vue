<template>
  <div>
    <!-- Page header -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-900">数据分析</h1>
      <p class="mt-2 text-sm text-gray-700">系统运营数据和业务分析报告</p>
    </div>

    <!-- Time Range Selector -->
    <div class="bg-white shadow rounded-lg mb-6">
      <div class="px-4 py-5 sm:p-6">
        <div class="flex items-center justify-between">
          <h3 class="text-lg leading-6 font-medium text-gray-900">时间范围</h3>
          <div class="flex space-x-3">
            <button
              v-for="range in timeRanges"
              :key="range.value"
              @click="selectedTimeRange = range.value"
              :class="[
                'px-3 py-2 text-sm font-medium rounded-md',
                selectedTimeRange === range.value
                  ? 'bg-blue-100 text-blue-700'
                  : 'text-gray-500 hover:text-gray-700'
              ]"
            >
              {{ range.label }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Key Metrics -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-white overflow-hidden shadow rounded-lg">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">活跃用户</dt>
                <dd class="flex items-baseline">
                  <div class="text-2xl font-semibold text-gray-900">{{ metrics.activeUsers }}</div>
                  <div class="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                    <svg class="self-center flex-shrink-0 h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                    <span class="sr-only">Increased by</span>
                    {{ metrics.activeUsersGrowth }}%
                  </div>
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white overflow-hidden shadow rounded-lg">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">页面浏览量</dt>
                <dd class="flex items-baseline">
                  <div class="text-2xl font-semibold text-gray-900">{{ formatNumber(metrics.pageViews) }}</div>
                  <div class="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                    <svg class="self-center flex-shrink-0 h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                    <span class="sr-only">Increased by</span>
                    {{ metrics.pageViewsGrowth }}%
                  </div>
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white overflow-hidden shadow rounded-lg">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">交易金额</dt>
                <dd class="flex items-baseline">
                  <div class="text-2xl font-semibold text-gray-900">¥{{ formatNumber(metrics.revenue) }}</div>
                  <div class="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                    <svg class="self-center flex-shrink-0 h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                    <span class="sr-only">Increased by</span>
                    {{ metrics.revenueGrowth }}%
                  </div>
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white overflow-hidden shadow rounded-lg">
        <div class="p-5">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <svg class="h-6 w-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">转化率</dt>
                <dd class="flex items-baseline">
                  <div class="text-2xl font-semibold text-gray-900">{{ metrics.conversionRate }}%</div>
                  <div class="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                    <svg class="self-center flex-shrink-0 h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                    </svg>
                    <span class="sr-only">Increased by</span>
                    {{ metrics.conversionRateGrowth }}%
                  </div>
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts Section -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
      <!-- User Activity Chart -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">用户活动趋势</h3>
          <div class="h-64 flex items-center justify-center bg-gray-50 rounded">
            <div class="text-center">
              <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
              <p class="mt-2 text-sm text-gray-500">图表组件待集成</p>
              <p class="text-xs text-gray-400">可集成 Chart.js 或 ECharts</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Revenue Chart -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">收入趋势</h3>
          <div class="h-64 flex items-center justify-center bg-gray-50 rounded">
            <div class="text-center">
              <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
              </svg>
              <p class="mt-2 text-sm text-gray-500">图表组件待集成</p>
              <p class="text-xs text-gray-400">可集成 Chart.js 或 ECharts</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Detailed Analytics -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Top Content -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">热门内容</h3>
          <div class="space-y-3">
            <div v-for="(item, index) in topContent" :key="index" class="flex items-center justify-between">
              <div class="flex items-center space-x-3">
                <div class="flex-shrink-0">
                  <span class="inline-flex items-center justify-center h-6 w-6 rounded-full bg-blue-100 text-blue-800 text-xs font-medium">
                    {{ index + 1 }}
                  </span>
                </div>
                <div class="min-w-0 flex-1">
                  <p class="text-sm font-medium text-gray-900 truncate">{{ item.title }}</p>
                  <p class="text-sm text-gray-500">{{ item.type }}</p>
                </div>
              </div>
              <div class="text-sm text-gray-500">{{ item.views }} 次浏览</div>
            </div>
          </div>
        </div>
      </div>

      <!-- User Demographics -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">用户分布</h3>
          <div class="space-y-4">
            <div v-for="demo in userDemographics" :key="demo.label" class="flex items-center justify-between">
              <div class="flex items-center space-x-3">
                <div class="w-3 h-3 rounded-full" :style="{ backgroundColor: demo.color }"></div>
                <span class="text-sm text-gray-900">{{ demo.label }}</span>
              </div>
              <div class="text-sm text-gray-500">{{ demo.percentage }}%</div>
            </div>
          </div>
        </div>
      </div>

      <!-- System Health -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">系统状态</h3>
          <div class="space-y-4">
            <div v-for="health in systemHealth" :key="health.metric" class="flex items-center justify-between">
              <span class="text-sm text-gray-900">{{ health.metric }}</span>
              <div class="flex items-center space-x-2">
                <div class="w-16 bg-gray-200 rounded-full h-2">
                  <div 
                    class="h-2 rounded-full transition-all duration-300"
                    :class="health.status === 'good' ? 'bg-green-500' : health.status === 'warning' ? 'bg-yellow-500' : 'bg-red-500'"
                    :style="{ width: `${health.value}%` }"
                  ></div>
                </div>
                <span class="text-sm text-gray-500">{{ health.value }}%</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

// Time range options
const timeRanges = [
  { label: '今天', value: 'today' },
  { label: '7天', value: '7days' },
  { label: '30天', value: '30days' },
  { label: '90天', value: '90days' }
]

const selectedTimeRange = ref('7days')

// Mock analytics data
const metrics = ref({
  activeUsers: 1234,
  activeUsersGrowth: 12.5,
  pageViews: 45678,
  pageViewsGrowth: 8.3,
  revenue: 23456,
  revenueGrowth: 15.2,
  conversionRate: 3.2,
  conversionRateGrowth: 5.1
})

const topContent = ref([
  { title: '扎染工艺详解', type: '文章', views: 2345 },
  { title: '传统蓝染布料', type: '商品', views: 1876 },
  { title: '扎染历史文化', type: '文章', views: 1654 },
  { title: '手工扎染围巾', type: '商品', views: 1432 },
  { title: '扎染技法大全', type: '文章', views: 1298 }
])

const userDemographics = ref([
  { label: '18-25岁', percentage: 35, color: '#3B82F6' },
  { label: '26-35岁', percentage: 28, color: '#10B981' },
  { label: '36-45岁', percentage: 22, color: '#F59E0B' },
  { label: '46岁以上', percentage: 15, color: '#EF4444' }
])

const systemHealth = ref([
  { metric: 'CPU使用率', value: 65, status: 'good' },
  { metric: '内存使用率', value: 78, status: 'warning' },
  { metric: '磁盘使用率', value: 45, status: 'good' },
  { metric: '网络延迟', value: 23, status: 'good' },
  { metric: '错误率', value: 2, status: 'good' }
])

// Utility functions
const formatNumber = (num: number) => {
  if (num >= 1000000) {
    return (num / 1000000).toFixed(1) + 'M'
  }
  if (num >= 1000) {
    return (num / 1000).toFixed(1) + 'K'
  }
  return num.toString()
}

// Watch time range changes
watch(selectedTimeRange, (newRange) => {
  console.log('Time range changed to:', newRange)
  // Here you would typically reload data based on the new time range
})
</script>
