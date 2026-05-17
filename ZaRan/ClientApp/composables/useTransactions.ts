import { TransactionsService, type TransactionDetailResponse, type TransactionPostEndpointRequest, type TransactionPatchEndpointRequest, TransactionStatus } from '~/api'

export function useTransactions() {
  const transactions = ref<TransactionDetailResponse[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const total = ref(0)

  /**
   * 获取交易列表
   */
  const fetchTransactions = async (page: number = 1, pageSize: number = 12) => {
    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize, true)

      const response = await executeApi(
        () => TransactionsService.transactionListEndpoint(
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        transactions.value = []
        total.value = 0
      } else if (response.data) {
        transactions.value = response.data.items
        total.value = response.data.total
      }

    } catch (err) {
      error.value = '加载交易记录失败，请稍后重试'
      console.error('获取交易记录失败:', err)
      transactions.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  /**
   * 获取单个交易详情
   */
  const getTransactionById = async (id: string): Promise<TransactionDetailResponse | null> => {
    loading.value = true
    error.value = null

    try {
      const { executeApi } = useApi()

      const response = await executeApi(
        () => TransactionsService.transactionGetEndpoint(id),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        return null
      }

      return response.data
    } catch (err) {
      error.value = '加载交易详情失败'
      console.error('获取交易详情失败:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  /**
   * 创建交易
   */
  const createTransaction = async (transactionData: TransactionPostEndpointRequest): Promise<TransactionDetailResponse | null> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return null

    const response = await executeApi(
      () => TransactionsService.transactionPostEndpoint(transactionData),
      { errorMessage: '创建交易失败' }
    )

    if (response.data) {
      // 添加到交易列表
      transactions.value.unshift(response.data)
      total.value++
    }

    return response.data
  }

  /**
   * 更新交易状态
   */
  const updateTransaction = async (id: string, transactionData: TransactionPatchEndpointRequest): Promise<TransactionDetailResponse | null> => {
    const { executeApi, requireAuth } = useApi()
    
    if (!requireAuth()) return null

    const response = await executeApi(
      () => TransactionsService.transactionPatchEndpoint(id, transactionData),
      { errorMessage: '更新交易失败' }
    )

    if (response.data) {
      // 更新交易列表中的交易
      const index = transactions.value.findIndex(transaction => transaction.id === id)
      if (index !== -1) {
        transactions.value[index] = response.data
      }
    }

    return response.data
  }

  /**
   * 获取交易状态的中文名称
   */
  const getTransactionStatusName = (status: TransactionStatus): string => {
    const statusNames: Record<TransactionStatus, string> = {
      [TransactionStatus._0]: '待付款',
      [TransactionStatus._1]: '已付款',
      [TransactionStatus._2]: '已发货',
      [TransactionStatus._3]: '已完成',
      [TransactionStatus._4]: '已取消',
      [TransactionStatus._5]: '已退款'
    }
    return statusNames[status] || '未知状态'
  }

  /**
   * 获取交易状态的颜色类
   */
  const getTransactionStatusColor = (status: TransactionStatus): string => {
    const statusColors: Record<TransactionStatus, string> = {
      [TransactionStatus._0]: 'text-orange-500',
      [TransactionStatus._1]: 'text-blue-500',
      [TransactionStatus._2]: 'text-purple-500',
      [TransactionStatus._3]: 'text-green-500',
      [TransactionStatus._4]: 'text-red-500',
      [TransactionStatus._5]: 'text-gray-500'
    }
    return statusColors[status] || 'text-gray-500'
  }

  /**
   * 格式化交易金额
   */
  const formatTransactionAmount = (amount: number): string => {
    return `¥${Math.abs(amount).toFixed(2)}`
  }

  /**
   * 格式化交易时间
   */
  const formatTransactionTime = (dateString: string): string => {
    const date = new Date(dateString)
    return date.toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    })
  }

  /**
   * 检查交易是否可以取消
   */
  const canCancelTransaction = (transaction: TransactionDetailResponse): boolean => {
    return transaction.status === TransactionStatus._0 // 只有待付款状态可以取消
  }

  /**
   * 检查交易是否可以退款
   */
  const canRefundTransaction = (transaction: TransactionDetailResponse): boolean => {
    return transaction.status === TransactionStatus._1 || transaction.status === TransactionStatus._2 // 已付款或已发货可以退款
  }

  /**
   * 取消交易
   */
  const cancelTransaction = async (id: string): Promise<boolean> => {
    const success = await updateTransaction(id, { status: TransactionStatus._4 })
    return !!success
  }

  /**
   * 申请退款
   */
  const requestRefund = async (id: string): Promise<boolean> => {
    const success = await updateTransaction(id, { status: TransactionStatus._5 })
    return !!success
  }

  // 计算总页数
  const totalPages = computed(() => {
    return Math.ceil(total.value / 12) // 默认每页12条
  })

  return {
    transactions: readonly(transactions),
    loading: readonly(loading),
    error: readonly(error),
    total: readonly(total),
    totalPages: readonly(totalPages),
    fetchTransactions,
    getTransactionById,
    createTransaction,
    updateTransaction,
    getTransactionStatusName,
    getTransactionStatusColor,
    formatTransactionAmount,
    formatTransactionTime,
    canCancelTransaction,
    canRefundTransaction,
    cancelTransaction,
    requestRefund
  }
}
