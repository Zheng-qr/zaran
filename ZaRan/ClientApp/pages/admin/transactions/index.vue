<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ isAdmin() ? '交易管理' : '我的交易' }}</h1>
        <p class="mt-2 text-sm text-gray-700">
          {{ isAdmin() ? '管理所有交易记录' : '查看您的交易记录' }}
        </p>
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
              placeholder="搜索商品名称、描述、用户..."
            >
          </div>

          <!-- Status -->
          <div>
            <label for="status" class="block text-sm font-medium text-gray-700">交易状态</label>
            <select
              id="status"
              v-model="filters.status"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">全部状态</option>
              <option v-for="status in transactionStatuses" :key="status.value" :value="status.value">
                {{ status.name }}
              </option>
            </select>
          </div>

          <!-- Type -->
          <div>
            <label for="type" class="block text-sm font-medium text-gray-700">交易类型</label>
            <select
              id="type"
              v-model="filters.type"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">全部类型</option>
              <option v-for="type in transactionTypes" :key="type.value" :value="type.value">
                {{ type.name }}
              </option>
            </select>
          </div>

          <!-- Sort -->
          <div>
            <label for="sort" class="block text-sm font-medium text-gray-700">排序</label>
            <select
              id="sort"
              v-model="filters.desc"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option :value="true">最新交易</option>
              <option :value="false">最早交易</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Transactions list -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <!-- Loading state -->
        <div v-if="loading" class="flex justify-center py-8">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>

        <!-- Empty state -->
        <div v-else-if="transactions.length === 0" class="text-center py-8">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">暂无交易记录</h3>
          <p class="mt-1 text-sm text-gray-500">还没有任何交易记录</p>
        </div>

        <!-- Transactions table -->
        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  交易信息
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  商品
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  买家/卖家
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  金额
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  状态
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  时间
                </th>
                <th v-if="isAdmin()" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  操作
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="transaction in transactions" :key="transaction.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div>
                      <div class="text-sm font-medium text-gray-900">
                        {{ getTransactionTypeName(transaction.type) }}
                      </div>
                      <div class="text-sm text-gray-500">
                        #{{ transaction.id?.slice(-8) }}
                      </div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">
                    {{ transaction.goodName || '无关联商品' }}
                  </div>
                  <div v-if="transaction.description" class="text-sm text-gray-500">
                    {{ transaction.description }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">
                    买家: {{ transaction.userName || '未知' }}
                  </div>
                  <div class="text-sm text-gray-500">
                    卖家: {{ transaction.targetUserName || '未知' }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900">
                    ¥{{ transaction.amount?.toFixed(2) || '0.00' }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusColorClass(transaction.status)">
                    {{ getTransactionStatusName(transaction.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(transaction.createdAt) }}
                </td>
                <td v-if="isAdmin()" class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <NuxtLink
                      :to="`/admin/transactions/${transaction.id}`"
                      class="text-blue-600 hover:text-blue-900"
                    >
                      查看
                    </NuxtLink>
                    <button
                      v-if="canUpdateTransaction(transaction)"
                      @click="updateTransactionStatus(transaction)"
                      class="text-green-600 hover:text-green-900"
                    >
                      更新状态
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <div v-if="totalTransactions > pageSize" class="mt-6 flex items-center justify-between">
          <div class="text-sm text-gray-700">
            显示第 {{ (currentPage - 1) * pageSize + 1 }} - {{ Math.min(currentPage * pageSize, totalTransactions) }} 条，
            共 {{ totalTransactions }} 条记录
          </div>
          <div class="flex space-x-2">
            <button
              :disabled="currentPage === 1"
              @click="currentPage--; loadTransactions()"
              class="px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              上一页
            </button>
            <button
              :disabled="currentPage * pageSize >= totalTransactions"
              @click="currentPage++; loadTransactions()"
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
import { TransactionsService } from '~/api'
import type { TransactionDetailResponse, TransactionPatchEndpointRequest } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { createPaginationParams } = useApi()
const { isAdmin } = usePermissions()

// Transaction constants
const TRANSACTION_STATUS = {
  PENDING: 0,
  COMPLETED: 1,
  CANCELLED: 2,
  REFUNDED: 3
}

const TRANSACTION_TYPE = {
  PURCHASE: 0,
  REFUND: 1,
  TRANSFER: 2
}

// Available options
const transactionStatuses = [
  { value: TRANSACTION_STATUS.PENDING, name: '待处理' },
  { value: TRANSACTION_STATUS.COMPLETED, name: '已完成' },
  { value: TRANSACTION_STATUS.CANCELLED, name: '已取消' },
  { value: TRANSACTION_STATUS.REFUNDED, name: '已退款' }
]

const transactionTypes = [
  { value: TRANSACTION_TYPE.PURCHASE, name: '购买' },
  { value: TRANSACTION_TYPE.REFUND, name: '退款' },
  { value: TRANSACTION_TYPE.TRANSFER, name: '转账' }
]

// Data
const transactions = ref<TransactionDetailResponse[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(20)
const totalTransactions = ref(0)

// Filters
const filters = ref({
  keyword: '',
  status: '',
  type: '',
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

// Load transactions
const loadTransactions = async () => {
  loading.value = true
  
  try {
    const params = createPaginationParams(
      currentPage.value,
      pageSize.value,
      filters.value.desc,
      filters.value.keyword || undefined
    )
    
    const response = await TransactionsService.transactionListEndpoint(
      params.offset,
      params.limit,
      params.desc,
      params.keyword
    )
    
    transactions.value = response.items || []
    totalTransactions.value = response.total || 0
  } catch (error) {
    console.error('Failed to load transactions:', error)
  } finally {
    loading.value = false
  }
}

// Debounced search
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1
  loadTransactions()
}, 500)

// Watch filters
watch(() => filters.value.keyword, debouncedSearch)
watch(() => filters.value.status, () => {
  currentPage.value = 1
  loadTransactions()
})
watch(() => filters.value.type, () => {
  currentPage.value = 1
  loadTransactions()
})
watch(() => filters.value.desc, () => {
  currentPage.value = 1
  loadTransactions()
})

// Transaction actions
const canUpdateTransaction = (transaction: TransactionDetailResponse) => {
  return isAdmin() && transaction.status === TRANSACTION_STATUS.PENDING
}

const updateTransactionStatus = async (transaction: TransactionDetailResponse) => {
  if (!transaction.id) return
  
  const newStatus = prompt('请输入新的状态 (0=待处理, 1=已完成, 2=已取消, 3=已退款):')
  if (newStatus === null) return
  
  const statusValue = parseInt(newStatus)
  if (isNaN(statusValue) || statusValue < 0 || statusValue > 3) {
    alert('无效的状态值')
    return
  }
  
  try {
    const updateData: TransactionPatchEndpointRequest = { status: statusValue }
    await TransactionsService.transactionPatchEndpoint(transaction.id, updateData)
    
    const { success } = useGlobalNotifications()
    success('更新成功', '交易状态已更新')
    loadTransactions()
  } catch (error) {
    console.error('Failed to update transaction:', error)
    const { error: showError } = useGlobalNotifications()
    showError('更新失败', '交易状态更新失败，请重试')
  }
}

// Utility functions
const getTransactionTypeName = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const typeObj = transactionTypes.find(t => t.value === typeValue)
  return typeObj?.name || '未知类型'
}

const getTransactionStatusName = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const statusObj = transactionStatuses.find(s => s.value === statusValue)
  return statusObj?.name || '未知状态'
}

const getStatusColorClass = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const colorMap = {
    0: 'bg-yellow-100 text-yellow-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-red-100 text-red-800',
    3: 'bg-blue-100 text-blue-800'
  }
  return colorMap[statusValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '未知时间'
  return new Date(dateString).toLocaleString('zh-CN')
}

// Load transactions on mount
onMounted(() => {
  loadTransactions()
})
</script>
