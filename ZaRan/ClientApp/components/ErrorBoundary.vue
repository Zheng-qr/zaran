<template>
  <div v-if="hasError" class="error-boundary">
    <div class="error-content">
      <div class="error-icon">
        <i class="fas fa-exclamation-triangle"></i>
      </div>
      <div class="error-text">
        <h2 class="error-title">{{ errorTitle }}</h2>
        <p class="error-message">{{ errorMessage }}</p>
        <div v-if="showDetails && errorDetails" class="error-details">
          <details>
            <summary>错误详情</summary>
            <pre class="error-stack">{{ errorDetails }}</pre>
          </details>
        </div>
      </div>
      <div class="error-actions">
        <button @click="retry" class="retry-btn">
          <i class="fas fa-redo"></i>
          重试
        </button>
        <button @click="goHome" class="home-btn">
          <i class="fas fa-home"></i>
          返回首页
        </button>
        <button v-if="showReport" @click="reportError" class="report-btn">
          <i class="fas fa-bug"></i>
          报告问题
        </button>
      </div>
    </div>
  </div>
  <slot v-else />
</template>

<script setup lang="ts">
interface Props {
  errorTitle?: string
  errorMessage?: string
  showDetails?: boolean
  showReport?: boolean
  onRetry?: () => void
  onReport?: (error: Error) => void
}

const props = withDefaults(defineProps<Props>(), {
  errorTitle: '出现了一些问题',
  errorMessage: '页面加载时发生错误，请稍后重试。',
  showDetails: false,
  showReport: true
})

const hasError = ref(false)
const errorDetails = ref<string>('')
const currentError = ref<Error | null>(null)

// 捕获错误
const captureError = (error: Error) => {
  hasError.value = true
  currentError.value = error
  errorDetails.value = error.stack || error.message
  
  // 记录错误到控制台
  console.error('ErrorBoundary caught an error:', error)
  
  // 可以在这里添加错误上报逻辑
  if (process.client) {
    // 发送错误到监控服务
    // reportErrorToService(error)
  }
}

// 重试
const retry = () => {
  hasError.value = false
  errorDetails.value = ''
  currentError.value = null
  
  if (props.onRetry) {
    props.onRetry()
  } else {
    // 默认重新加载页面
    window.location.reload()
  }
}

// 返回首页
const goHome = () => {
  navigateTo('/')
}

// 报告错误
const reportError = () => {
  if (props.onReport && currentError.value) {
    props.onReport(currentError.value)
  } else {
    // 默认的错误报告逻辑
    const errorInfo = {
      message: currentError.value?.message,
      stack: currentError.value?.stack,
      url: window.location.href,
      userAgent: navigator.userAgent,
      timestamp: new Date().toISOString()
    }
    
    // 这里可以发送到错误收集服务
    console.log('Error report:', errorInfo)
    
    const { success } = useGlobalNotifications()
    success('错误报告已发送', '感谢您的反馈，我们会尽快修复问题。')
  }
}

// 监听全局错误
onMounted(() => {
  if (process.client) {
    window.addEventListener('error', (event) => {
      captureError(new Error(event.message))
    })
    
    window.addEventListener('unhandledrejection', (event) => {
      captureError(new Error(event.reason))
    })
  }
})

// 暴露捕获错误的方法
defineExpose({
  captureError
})
</script>

<style scoped>
.error-boundary {
  min-height: 400px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
}

.error-content {
  text-align: center;
  max-width: 500px;
  width: 100%;
}

.error-icon {
  font-size: 4rem;
  color: #ef4444;
  margin-bottom: 24px;
}

.error-text {
  margin-bottom: 32px;
}

.error-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 12px;
}

.error-message {
  color: #6b7280;
  line-height: 1.6;
  margin-bottom: 16px;
}

.error-details {
  text-align: left;
  margin-top: 16px;
}

.error-details summary {
  cursor: pointer;
  color: #6b7280;
  font-size: 0.9rem;
  margin-bottom: 8px;
}

.error-stack {
  background: #f3f4f6;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  padding: 12px;
  font-size: 0.8rem;
  color: #374151;
  overflow-x: auto;
  white-space: pre-wrap;
  word-break: break-all;
}

.error-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
  flex-wrap: wrap;
}

.retry-btn,
.home-btn,
.report-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.2s;
}

.retry-btn {
  background: #667eea;
  color: white;
}

.retry-btn:hover {
  background: #5a6fd8;
}

.home-btn {
  background: #10b981;
  color: white;
}

.home-btn:hover {
  background: #059669;
}

.report-btn {
  background: #f3f4f6;
  color: #6b7280;
  border: 1px solid #e5e7eb;
}

.report-btn:hover {
  background: #e5e7eb;
  color: #374151;
}

/* 响应式设计 */
@media (max-width: 640px) {
  .error-boundary {
    padding: 20px;
    min-height: 300px;
  }

  .error-icon {
    font-size: 3rem;
    margin-bottom: 20px;
  }

  .error-title {
    font-size: 1.25rem;
  }

  .error-actions {
    flex-direction: column;
    align-items: center;
  }

  .retry-btn,
  .home-btn,
  .report-btn {
    width: 100%;
    max-width: 200px;
    justify-content: center;
  }
}
</style>
