export interface LoadingState {
  id: string
  message?: string
  progress?: number
}

export function useLoading() {
  const loadingStates = ref<LoadingState[]>([])

  /**
   * 开始加载
   */
  const startLoading = (message?: string, id?: string): string => {
    const loadingId = id || Date.now().toString() + Math.random().toString(36).substr(2, 9)
    
    const existingIndex = loadingStates.value.findIndex(state => state.id === loadingId)
    if (existingIndex > -1) {
      // 更新现有的加载状态
      loadingStates.value[existingIndex].message = message
    } else {
      // 添加新的加载状态
      loadingStates.value.push({
        id: loadingId,
        message,
        progress: 0
      })
    }

    return loadingId
  }

  /**
   * 更新加载进度
   */
  const updateProgress = (id: string, progress: number, message?: string) => {
    const state = loadingStates.value.find(state => state.id === id)
    if (state) {
      state.progress = Math.max(0, Math.min(100, progress))
      if (message) {
        state.message = message
      }
    }
  }

  /**
   * 停止加载
   */
  const stopLoading = (id: string) => {
    const index = loadingStates.value.findIndex(state => state.id === id)
    if (index > -1) {
      loadingStates.value.splice(index, 1)
    }
  }

  /**
   * 清空所有加载状态
   */
  const clearLoading = () => {
    loadingStates.value = []
  }

  /**
   * 检查是否有加载状态
   */
  const isLoading = computed(() => loadingStates.value.length > 0)

  /**
   * 获取当前加载消息
   */
  const currentLoadingMessage = computed(() => {
    const latest = loadingStates.value[loadingStates.value.length - 1]
    return latest?.message || '加载中...'
  })

  /**
   * 获取当前加载进度
   */
  const currentProgress = computed(() => {
    const latest = loadingStates.value[loadingStates.value.length - 1]
    return latest?.progress || 0
  })

  /**
   * 包装异步函数以自动管理加载状态
   */
  const withLoading = async <T>(
    asyncFn: () => Promise<T>,
    message?: string,
    id?: string
  ): Promise<T> => {
    const loadingId = startLoading(message, id)
    try {
      const result = await asyncFn()
      return result
    } finally {
      stopLoading(loadingId)
    }
  }

  return {
    loadingStates: readonly(loadingStates),
    isLoading,
    currentLoadingMessage,
    currentProgress,
    startLoading,
    updateProgress,
    stopLoading,
    clearLoading,
    withLoading
  }
}

// 全局加载状态实例
let globalLoading: ReturnType<typeof useLoading> | null = null

/**
 * 获取全局加载状态实例
 */
export function useGlobalLoading() {
  if (!globalLoading) {
    globalLoading = useLoading()
  }
  return globalLoading
}
