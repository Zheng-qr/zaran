<template>
  <div class="min-h-screen bg-gray-50 flex">
    <!-- Mobile menu overlay -->
    <div
      v-if="sidebarOpen"
      class="fixed inset-0 z-40 lg:hidden"
      @click="sidebarOpen = false"
    >
      <div class="fixed inset-0 bg-gray-600 bg-opacity-75"></div>
    </div>

    <!-- Sidebar -->
    <div
      class="fixed inset-y-0 left-0 z-50 w-64 bg-white shadow-lg transform transition-transform duration-300 ease-in-out lg:translate-x-0 lg:static lg:inset-0"
      :class="sidebarOpen ? 'translate-x-0' : '-translate-x-full'"
    >
      <AdminSidebar @close="sidebarOpen = false" />
    </div>

    <!-- Main content -->
    <div class="flex-1 flex flex-col min-h-screen lg:ml-0">
      <!-- Top navigation -->
      <AdminHeader @toggle-sidebar="sidebarOpen = !sidebarOpen" />

      <!-- Page content -->
      <main class="flex-1 py-6">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <!-- Breadcrumbs -->
          <AdminBreadcrumbs />

          <!-- Page content -->
          <div class="mt-6">
            <slot />
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
// Check publisher permissions (minimum required for admin panel)
const { isPublisher, isActiveUser } = usePermissions()

// Redirect if not publisher or not active
if (!isPublisher() || !isActiveUser()) {
  throw createError({
    statusCode: 403,
    statusMessage: '您需要发布者权限才能访问管理后台'
  })
}

// Sidebar state
const sidebarOpen = ref(false)

// Close sidebar on route change (mobile)
const route = useRoute()
watch(() => route.path, () => {
  sidebarOpen.value = false
})
</script>

<style scoped>
/* 确保布局正确对齐 */
.min-h-screen {
  min-height: 100vh;
}

/* 确保侧边栏在大屏幕上正确定位 */
@media (min-width: 1024px) {
  .lg\:static {
    position: static !important;
  }

  .lg\:translate-x-0 {
    transform: translateX(0) !important;
  }
}

/* 确保主内容区域正确布局 */
.flex-1 {
  flex: 1;
}
</style>
