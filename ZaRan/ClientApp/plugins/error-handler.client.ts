export default defineNuxtPlugin(() => {
  // 获取全局通知实例
  const { error: showError } = useGlobalNotifications()

  // 处理未捕获的 JavaScript 错误
  window.addEventListener('error', (event) => {
    console.error('Global error:', event.error)
    showError(
      '页面错误',
      '页面运行时发生错误，请刷新页面重试。',
      true // 持久化显示
    )
  })

  // 处理未处理的 Promise 拒绝
  window.addEventListener('unhandledrejection', (event) => {
    console.error('Unhandled promise rejection:', event.reason)
    showError(
      '网络错误',
      '网络请求失败，请检查网络连接后重试。',
      true // 持久化显示
    )
    
    // 阻止默认的控制台错误输出
    event.preventDefault()
  })

  // 处理资源加载错误
  window.addEventListener('error', (event) => {
    if (event.target && event.target !== window) {
      const target = event.target as HTMLElement
      if (target.tagName === 'IMG') {
        console.warn('Image load failed:', (target as HTMLImageElement).src)
        // 可以设置默认图片
        ;(target as HTMLImageElement).src = '/image/default.png'
      } else if (target.tagName === 'SCRIPT') {
        console.error('Script load failed:', (target as HTMLScriptElement).src)
        showError(
          '资源加载失败',
          '页面资源加载失败，请刷新页面重试。'
        )
      }
    }
  }, true) // 使用捕获阶段

  // Vue 错误处理
  const vueApp = useNuxtApp().vueApp
  vueApp.config.errorHandler = (error, instance, info) => {
    console.error('Vue error:', error, info)
    showError(
      '应用错误',
      '应用运行时发生错误，请刷新页面重试。',
      true
    )
  }
})
