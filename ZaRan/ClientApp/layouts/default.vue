<template>
  <div>
    <ErrorBoundary>
      <Header />
      <main>
        <slot />
      </main>
      <Footer />
    </ErrorBoundary>

    <!-- 全局组件 -->
    <NotificationContainer />
    <LoadingOverlay />
  </div>
</template>

<script setup lang="ts">
// 在 Nuxt 中，组件会自动导入，无需手动导入 Header 和 Footer
</script>
