<template>
  <div>
    <!-- Loading state -->
    <LoadingOverlay v-if="loading" />

    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">交易详情</h1>
        <p class="mt-2 text-sm text-gray-700">查看交易的详细信息</p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <NuxtLink
          to="/admin/transactions"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          返回列表
        </NuxtLink>
      </div>
    </div>

    <!-- Transaction details -->
    <div v-if="!loading && transaction" class="space-y-6">
      <!-- Basic info card -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">基本信息</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <dt class="text-sm font-medium text-gray-500">交易ID</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ transaction.id }}</dd>
            </div>
            
            <div>
              <dt class="text-sm font-medium text-gray-500">交易类型</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ getTransactionTypeName(transaction.type) }}</dd>
            </div>
            
            <div>
              <dt class="text-sm font-medium text-gray-500">交易状态</dt>
              <dd class="mt-1">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="getStatusColorClass(transaction.status)">
                  {{ getTransactionStatusName(transaction.status) }}
                </span>
              </dd>
            </div>
            
            <div>
              <dt class="text-sm font-medium text-gray-500">交易金额</dt>
              <dd class="mt-1 text-lg font-semibold text-gray-900">
                ¥{{ transaction.amount?.toFixed(2) || '0.00' }}
              </dd>
            </div>
            
            <div>
              <dt class="text-sm font-medium text-gray-500">创建时间</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ formatDate(transaction.createdAt) }}</dd>
            </div>
            
            <div>
              <dt class="text-sm font-medium text-gray-500">更新时间</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ formatDate(transaction.updatedAt) }}</dd>
            </div>
          </div>
          
          <div v-if="transaction.description" class="mt-6">
            <dt class="text-sm font-medium text-gray-500">交易描述</dt>
            <dd class="mt-1 text-sm text-gray-900">{{ transaction.description }}</dd>
          </div>
        </div>
      </div>

      <!-- Participants card -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">交易参与者</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Buyer -->
            <div class="border border-gray-200 rounded-lg p-4">
              <h4 class="text-sm font-medium text-gray-900 mb-3">买家信息</h4>
              <div class="space-y-2">
                <div>
                  <dt class="text-xs font-medium text-gray-500">用户名</dt>
                  <dd class="text-sm text-gray-900">{{ transaction.userName || '未知用户' }}</dd>
                </div>
                <div>
                  <dt class="text-xs font-medium text-gray-500">用户ID</dt>
                  <dd class="text-sm text-gray-900">{{ transaction.userId || '未知' }}</dd>
                </div>
              </div>
            </div>
            
            <!-- Seller -->
            <div class="border border-gray-200 rounded-lg p-4">
              <h4 class="text-sm font-medium text-gray-900 mb-3">卖家信息</h4>
              <div class="space-y-2">
                <div>
                  <dt class="text-xs font-medium text-gray-500">用户名</dt>
                  <dd class="text-sm text-gray-900">{{ transaction.targetUserName || '未知用户' }}</dd>
                </div>
                <div>
                  <dt class="text-xs font-medium text-gray-500">用户ID</dt>
                  <dd class="text-sm text-gray-900">{{ transaction.targetUserId || '未知' }}</dd>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Good info card -->
      <div v-if="transaction.goodId" class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">商品信息</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <dt class="text-sm font-medium text-gray-500">商品名称</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ transaction.goodName || '未知商品' }}</dd>
            </div>
            
            <div>
              <dt class="text-sm font-medium text-gray-500">商品ID</dt>
              <dd class="mt-1 text-sm text-gray-900">{{ transaction.goodId }}</dd>
            </div>
          </div>
          
          <div class="mt-4">
            <NuxtLink
              :to="`/admin/goods/${transaction.goodId}`"
              class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-blue-700 bg-blue-100 hover:bg-blue-200"
            >
              查看商品详情
            </NuxtLink>
          </div>
        </div>
      </div>

      <!-- Admin actions -->
      <div v-if="isAdmin() && canUpdateTransaction" class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">管理操作</h3>
          
          <div class="space-y-4">
            <!-- Status update -->
            <div>
              <label for="status" class="block text-sm font-medium text-gray-700">更新交易状态</label>
              <div class="mt-1 flex space-x-3">
                <select
                  id="status"
                  v-model="newStatus"
                  class="block w-48 border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                >
                  <option v-for="status in transactionStatuses" :key="status.value" :value="status.value">
                    {{ status.name }}
                  </option>
                </select>
                <button
                  @click="updateStatus"
                  :disabled="updating || newStatus === transaction.status"
                  class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <svg v-if="updating" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  {{ updating ? '更新中...' : '更新状态' }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Not found state -->
    <div v-else-if="!loading" class="text-center py-12">
      <h3 class="mt-2 text-sm font-medium text-gray-900">交易不存在</h3>
      <p class="mt-1 text-sm text-gray-500">请检查交易ID是否正确</p>
      <div class="mt-6">
        <NuxtLink
          to="/admin/transactions"
          class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
        >
          返回交易列表
        </NuxtLink>
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

const route = useRoute()
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
const transaction = ref<TransactionDetailResponse | null>(null)
const loading = ref(true)
const updating = ref(false)
const newStatus = ref(TRANSACTION_STATUS.PENDING)

// Computed
const canUpdateTransaction = computed(() => {
  return transaction.value && transaction.value.status === TRANSACTION_STATUS.PENDING
})

// Load transaction data
const loadTransaction = async () => {
  try {
    const transactionId = route.params.id as string
    const response = await TransactionsService.transactionGetEndpoint(transactionId)
    
    transaction.value = response
    newStatus.value = response.status || TRANSACTION_STATUS.PENDING
    
  } catch (error) {
    console.error('Failed to load transaction:', error)
    const { error: showError } = useGlobalNotifications()
    showError('加载失败', '无法加载交易数据')
    transaction.value = null
  } finally {
    loading.value = false
  }
}

// Update transaction status
const updateStatus = async () => {
  if (!transaction.value?.id) return
  
  updating.value = true
  
  try {
    const updateData: TransactionPatchEndpointRequest = { status: newStatus.value }
    const response = await TransactionsService.transactionPatchEndpoint(transaction.value.id, updateData)
    
    transaction.value = response
    
    const { success } = useGlobalNotifications()
    success('更新成功', '交易状态已更新')
  } catch (error) {
    console.error('Failed to update transaction:', error)
    const { error: showError } = useGlobalNotifications()
    showError('更新失败', '交易状态更新失败，请重试')
  } finally {
    updating.value = false
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

// Load transaction on mount
onMounted(() => {
  loadTransaction()
})
</script>
