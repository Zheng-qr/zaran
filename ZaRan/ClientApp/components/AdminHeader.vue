<template>
  <header class="bg-white shadow-sm border-b border-gray-200">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <!-- Left side -->
        <div class="flex items-center">
          <!-- Mobile menu button -->
          <button
            @click="$emit('toggleSidebar')"
            class="lg:hidden p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100"
          >
            <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>

          <!-- Page title -->
          <h1 class="ml-4 lg:ml-0 text-lg font-semibold text-gray-900">
            {{ pageTitle }}
          </h1>
        </div>

        <!-- Right side -->
        <div class="flex items-center space-x-4">
          <!-- Notifications -->
          <button class="p-2 text-gray-400 hover:text-gray-500 hover:bg-gray-100 rounded-md relative">
            <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-5 5v-5zM10.5 3.5a6 6 0 0 1 6 6v2l1.5 3h-15l1.5-3v-2a6 6 0 0 1 6-6z" />
            </svg>
            <!-- Notification badge -->
            <span v-if="notificationCount > 0" class="absolute -top-1 -right-1 h-4 w-4 bg-red-500 text-white text-xs rounded-full flex items-center justify-center">
              {{ notificationCount > 9 ? '9+' : notificationCount }}
            </span>
          </button>

          <!-- Quick actions dropdown -->
          <div class="relative" ref="quickActionsRef">
            <button
              @click="showQuickActions = !showQuickActions"
              class="p-2 text-gray-400 hover:text-gray-500 hover:bg-gray-100 rounded-md"
            >
              <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
            </button>

            <!-- Quick actions menu -->
            <div
              v-if="showQuickActions"
              class="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-50 border border-gray-200"
            >
              <NuxtLink
                to="/admin/articles/create"
                class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                @click="showQuickActions = false"
              >
                创建文章
              </NuxtLink>
              <NuxtLink
                to="/admin/goods/create"
                class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                @click="showQuickActions = false"
              >
                创建商品
              </NuxtLink>
              <NuxtLink
                to="/admin/collections/create"
                class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                @click="showQuickActions = false"
              >
                创建集合
              </NuxtLink>
            </div>
          </div>

          <!-- User menu -->
          <div class="relative" ref="userMenuRef">
            <button
              @click="showUserMenu = !showUserMenu"
              class="flex items-center space-x-3 p-2 rounded-md hover:bg-gray-100"
            >
              <img 
                class="h-8 w-8 rounded-full" 
                :src="userStore.user?.avatar || '/default-avatar.png'" 
                :alt="userStore.user?.nickname"
              >
              <div class="hidden md:block text-left">
                <p class="text-sm font-medium text-gray-700">{{ userStore.user?.nickname }}</p>
                <p class="text-xs text-gray-500">{{ getRoleName() }}</p>
              </div>
              <svg class="h-4 w-4 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
              </svg>
            </button>

            <!-- User menu dropdown -->
            <div
              v-if="showUserMenu"
              class="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-50 border border-gray-200"
            >
              <NuxtLink
                to="/my"
                class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                @click="showUserMenu = false"
              >
                个人中心
              </NuxtLink>
              <NuxtLink
                to="/"
                class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                @click="showUserMenu = false"
              >
                返回前台
              </NuxtLink>
              <hr class="my-1">
              <button
                @click="handleLogout"
                class="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
              >
                退出登录
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
defineEmits<{
  toggleSidebar: []
}>()

const route = useRoute()
const userStore = useUserStore()
const { getRoleName } = usePermissions()

// Dropdown states
const showQuickActions = ref(false)
const showUserMenu = ref(false)
const quickActionsRef = ref<HTMLElement>()
const userMenuRef = ref<HTMLElement>()

// Mock notification count (replace with real data)
const notificationCount = ref(3)

// Page title based on route
const pageTitle = computed(() => {
  const pathSegments = route.path.split('/').filter(Boolean)
  
  if (pathSegments.length === 1 && pathSegments[0] === 'admin') {
    return '仪表板'
  }
  
  const titleMap: Record<string, string> = {
    'users': '用户管理',
    'articles': '文章管理',
    'goods': '商品管理',
    'collections': '集合管理',
    'transactions': '交易管理',
    'collects': '收藏品管理',
    'messages': '消息管理',
    'comments': '评论管理',
    'files': '文件管理',
    'settings': '系统设置'
  }
  
  const section = pathSegments[1]
  return titleMap[section] || '管理后台'
})

// Handle logout
const handleLogout = async () => {
  showUserMenu.value = false
  await userStore.logout()
  await navigateTo('/login')
}

// Close dropdowns when clicking outside
onClickOutside(quickActionsRef, () => {
  showQuickActions.value = false
})

onClickOutside(userMenuRef, () => {
  showUserMenu.value = false
})

// Close dropdowns on route change
watch(() => route.path, () => {
  showQuickActions.value = false
  showUserMenu.value = false
})
</script>
