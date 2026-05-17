<template>
  <div class="flex flex-col h-full">
    <!-- Logo -->
    <div class="flex items-center justify-between h-16 px-6 border-b border-gray-200">
      <div class="flex items-center">
        <h1 class="text-xl font-semibold text-gray-900">管理后台</h1>
      </div>
      <button
        @click="$emit('close')"
        class="lg:hidden p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100"
      >
        <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 px-4 py-6 space-y-2 overflow-y-auto">
      <!-- Dashboard -->
      <NuxtLink
        to="/admin"
        class="nav-item"
        :class="{ 'nav-item-active': $route.path === '/admin' }"
      >
        <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z" />
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v6H8V5z" />
        </svg>
        仪表板
      </NuxtLink>

      <!-- Analytics -->
      <NuxtLink
        to="/admin/analytics"
        class="nav-item"
        :class="{ 'nav-item-active': $route.path === '/admin/analytics' }"
      >
        <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
        </svg>
        数据分析
      </NuxtLink>

      <!-- User Management - Admin Only -->
      <div v-if="isAdmin()" class="nav-section">
        <div class="nav-section-title">用户管理</div>
        <NuxtLink
          to="/admin/users"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/users') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
          </svg>
          用户列表
        </NuxtLink>
      </div>

      <!-- Content Management -->
      <div class="nav-section">
        <div class="nav-section-title">内容管理</div>
        <NuxtLink
          to="/admin/articles"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/articles') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
          {{ isAdmin() ? '文章管理' : '我的文章' }}
        </NuxtLink>
        <NuxtLink
          to="/admin/collections"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/collections') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
          </svg>
          {{ isAdmin() ? '集合管理' : '我的集合' }}
        </NuxtLink>
      </div>

      <!-- Commerce Management -->
      <div class="nav-section">
        <div class="nav-section-title">商务管理</div>
        <NuxtLink
          to="/admin/goods"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/goods') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
          </svg>
          {{ isAdmin() ? '商品管理' : '我的商品' }}
        </NuxtLink>
        <NuxtLink
          v-if="isAdmin()"
          to="/admin/transactions"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/transactions') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>
          交易管理
        </NuxtLink>
        <NuxtLink
          v-if="isAdmin()"
          to="/admin/collects"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/collects') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
          </svg>
          收藏品管理
        </NuxtLink>
      </div>

      <!-- Communication Management - Admin Only -->
      <div v-if="isAdmin()" class="nav-section">
        <div class="nav-section-title">沟通管理</div>
        <NuxtLink
          to="/admin/messages"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/messages') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
          </svg>
          消息管理
        </NuxtLink>
        <NuxtLink
          to="/admin/comments"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/comments') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z" />
          </svg>
          评论管理
        </NuxtLink>
      </div>

      <!-- System Management - Admin Only -->
      <div v-if="isAdmin()" class="nav-section">
        <div class="nav-section-title">系统管理</div>
        <NuxtLink
          to="/admin/files"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/files') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
          </svg>
          文件管理
        </NuxtLink>
        <NuxtLink
          to="/admin/settings"
          class="nav-item nav-item-sub"
          :class="{ 'nav-item-active': $route.path.startsWith('/admin/settings') }"
        >
          <svg class="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          系统设置
        </NuxtLink>
      </div>
    </nav>

    <!-- User info -->
    <div class="border-t border-gray-200 p-4">
      <div class="flex items-center">
        <div class="flex-shrink-0">
          <img 
            class="h-8 w-8 rounded-full" 
            :src="userStore.user?.avatar || '/default-avatar.png'" 
            :alt="userStore.user?.nickname"
          >
        </div>
        <div class="ml-3">
          <p class="text-sm font-medium text-gray-700">{{ userStore.user?.nickname }}</p>
          <p class="text-xs text-gray-500">{{ getRoleName() }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
defineEmits<{
  close: []
}>()

const userStore = useUserStore()
const { getRoleName, isAdmin } = usePermissions()
</script>

<style scoped>
.nav-item {
  @apply flex items-center px-3 py-2 text-sm font-medium rounded-md text-gray-600 hover:text-gray-900 hover:bg-gray-50 transition-colors duration-200;
}

.nav-item-active {
  @apply text-blue-600 bg-blue-50 hover:bg-blue-100;
}

.nav-item-sub {
  @apply ml-4;
}

.nav-icon {
  @apply mr-3 h-5 w-5 flex-shrink-0;
}

.nav-section {
  @apply mt-6;
}

.nav-section-title {
  @apply px-3 text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2;
}

/* 确保导航区域可以滚动 */
nav.flex-1 {
  overflow-y: auto;
  scrollbar-width: thin;
  scrollbar-color: #cbd5e0 transparent;
}

/* 自定义滚动条样式 */
nav.flex-1::-webkit-scrollbar {
  width: 6px;
}

nav.flex-1::-webkit-scrollbar-track {
  background: transparent;
}

nav.flex-1::-webkit-scrollbar-thumb {
  background-color: #cbd5e0;
  border-radius: 3px;
}

nav.flex-1::-webkit-scrollbar-thumb:hover {
  background-color: #a0aec0;
}
</style>
