<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">用户管理</h1>
        <p class="mt-2 text-sm text-gray-700">管理系统中的所有用户账户</p>
      </div>
      <div class="mt-4 sm:mt-0">
        <button
          @click="refreshUsers"
          :disabled="loading"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          刷新
        </button>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white shadow rounded-lg mb-6">
      <div class="px-4 py-5 sm:p-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label for="search" class="block text-sm font-medium text-gray-700">搜索用户</label>
            <input
              id="search"
              v-model="filters.keyword"
              type="text"
              placeholder="用户名、昵称或邮箱"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @input="debouncedSearch"
            >
          </div>
          
          <div>
            <label for="role" class="block text-sm font-medium text-gray-700">角色筛选</label>
            <select
              id="role"
              v-model="filters.role"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadUsers"
            >
              <option value="">所有角色</option>
              <option v-for="role in availableRoles" :key="role.value" :value="role.value">
                {{ role.name }}
              </option>
            </select>
          </div>
          
          <div>
            <label for="status" class="block text-sm font-medium text-gray-700">状态筛选</label>
            <select
              id="status"
              v-model="filters.status"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadUsers"
            >
              <option value="">所有状态</option>
              <option v-for="status in availableStatuses" :key="status.value" :value="status.value">
                {{ status.name }}
              </option>
            </select>
          </div>
          
          <div>
            <label for="sort" class="block text-sm font-medium text-gray-700">排序方式</label>
            <select
              id="sort"
              v-model="filters.desc"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadUsers"
            >
              <option :value="true">最新注册</option>
              <option :value="false">最早注册</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Users table -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <LoadingOverlay v-if="loading" />
        
        <div v-else-if="users.length === 0" class="text-center py-12">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">没有找到用户</h3>
          <p class="mt-1 text-sm text-gray-500">尝试调整搜索条件或筛选器</p>
        </div>
        
        <div v-else class="overflow-hidden">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  用户信息
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  角色
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  状态
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  余额
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  注册时间
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  操作
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="user in users" :key="user.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10">
                      <img 
                        class="h-10 w-10 rounded-full" 
                        :src="user.avatar || '/default-avatar.png'" 
                        :alt="user.nickname"
                      >
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900">{{ user.nickname }}</div>
                      <div class="text-sm text-gray-500">@{{ user.username }}</div>
                      <div class="text-sm text-gray-500">{{ user.email }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getRoleColorClass(user.userRole)">
                    {{ getRoleName(user.userRole) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex flex-col">
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                          :class="getStatusColorClass(user.userStatus, user.unbanTime)">
                      {{ getStatusDisplayName(user.userStatus, user.unbanTime) }}
                    </span>
                    <span v-if="user.unbanTime && isUserBanned(user.unbanTime)" class="text-xs text-gray-500 mt-1">
                      解封时间：{{ formatDate(user.unbanTime) }}
                    </span>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  ¥{{ user.balance?.toFixed(2) || '0.00' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(user.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <NuxtLink
                      :to="`/admin/users/${user.id}`"
                      class="text-blue-600 hover:text-blue-900"
                    >
                      查看
                    </NuxtLink>
                    <button
                      @click="openEditModal(user)"
                      class="text-indigo-600 hover:text-indigo-900"
                    >
                      编辑
                    </button>
                    <button
                      v-if="!isUserBanned(user.unbanTime)"
                      @click="banUser(user)"
                      class="text-red-600 hover:text-red-900"
                    >
                      封禁
                    </button>
                    <button
                      v-else
                      @click="unbanUser(user)"
                      class="text-green-600 hover:text-green-900"
                    >
                      解封
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <AdminPagination
          v-if="users.length > 0"
          :current-page="currentPage"
          :total-items="totalUsers"
          :page-size="pageSize"
          @page-change="handlePageChange"
          class="mt-6"
        />
      </div>
    </div>

    <!-- Edit User Modal -->
    <AdminUserEditModal
      v-if="showEditModal"
      :user="selectedUser"
      @close="closeEditModal"
      @updated="handleUserUpdated"
    />
  </div>
</template>

<script setup lang="ts">
import { UsersService } from '~/api'
import type { UserDetailResponse, UserPatchRequest } from '~/api'
import { USER_STATUS } from '~/composables/usePermissions'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { getRoleName, getStatusName, getAllRoles, getAllStatuses, isBanned } = usePermissions()
const { executeApi, createPaginationParams } = useApi()

// Data
const users = ref<UserDetailResponse[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(20)
const totalUsers = ref(0)

// Filters
const filters = ref({
  keyword: '',
  role: '',
  status: '',
  desc: true
})

// Modal state
const showEditModal = ref(false)
const selectedUser = ref<UserDetailResponse | null>(null)

// Available options
const availableRoles = getAllRoles()
const availableStatuses = getAllStatuses()

// Load users
const loadUsers = async () => {
  loading.value = true

  try {
    const params = createPaginationParams(
      currentPage.value,
      pageSize.value,
      filters.value.desc,
      filters.value.keyword || undefined
    )

    const response = await UsersService.userListEndpoint(
      params.offset,
      params.limit,
      params.desc,
      params.keyword
    )

    users.value = response.items || []
    totalUsers.value = response.total || 0
  } catch (error) {
    console.error('Failed to load users:', error)
  } finally {
    loading.value = false
  }
}

// Debounced search
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1
  loadUsers()
}, 500)

// Refresh users
const refreshUsers = () => {
  loadUsers()
}

// Handle page change
const handlePageChange = (page: number) => {
  currentPage.value = page
  loadUsers()
}

// Modal functions
const openEditModal = (user: UserDetailResponse) => {
  selectedUser.value = user
  showEditModal.value = true
}

const closeEditModal = () => {
  showEditModal.value = false
  selectedUser.value = null
}

const handleUserUpdated = () => {
  closeEditModal()
  loadUsers()
}

// Helper functions
const isUserBanned = (unbanTime?: string | null): boolean => {
  if (!unbanTime) return false
  const unban = new Date(unbanTime)
  const now = new Date()
  return unban > now
}

const getStatusDisplayName = (status: any, unbanTime?: string | null): string => {
  const statusName = getStatusName(status)
  if (isUserBanned(unbanTime)) {
    return '已封禁'
  }
  return statusName
}

// User actions
const banUser = async (user: UserDetailResponse) => {
  // Ask for ban duration
  const duration = prompt('请输入封禁时长（小时），留空表示永久封禁：')
  if (duration === null) return // User cancelled

  const banHours = duration ? parseInt(duration) : null
  if (duration && (isNaN(banHours!) || banHours! <= 0)) {
    alert('请输入有效的小时数')
    return
  }

  if (!confirm(`确定要封禁用户 ${user.nickname} ${banHours ? `${banHours}小时` : '永久'} 吗？`)) return

  try {
    if (!user.id) return

    let unbanTime: string | null = null
    if (banHours) {
      const unbanDate = new Date()
      unbanDate.setHours(unbanDate.getHours() + banHours)
      unbanTime = unbanDate.toISOString()
    } else {
      // Permanent ban - set to far future
      const unbanDate = new Date()
      unbanDate.setFullYear(unbanDate.getFullYear() + 100)
      unbanTime = unbanDate.toISOString()
    }

    const updateData: UserPatchRequest = { unbanTime }
    await UsersService.userPatchEndpoint(user.id, updateData)
    const { success } = useGlobalNotifications()
    success('操作成功', `用户已被封禁${banHours ? `${banHours}小时` : '永久'}`)
    await loadUsers()
  } catch (error) {
    console.error('Failed to ban user:', error)
    const { error: showError } = useGlobalNotifications()
    showError('操作失败', '封禁用户失败，请重试')
  }
}

const unbanUser = async (user: UserDetailResponse) => {
  if (!confirm(`确定要解封用户 ${user.nickname} 吗？`)) return

  try {
    if (!user.id) return
    const updateData: UserPatchRequest = { unbanTime: null }
    await UsersService.userPatchEndpoint(user.id, updateData)
    const { success } = useGlobalNotifications()
    success('操作成功', '用户已被解封')
    await loadUsers()
  } catch (error) {
    console.error('Failed to unban user:', error)
    const { error: showError } = useGlobalNotifications()
    showError('操作失败', '解封用户失败，请重试')
  }
}

// Utility functions
const getRoleColorClass = (role: any) => {
  const roleValue = typeof role === 'number' ? role : 0
  const colorMap = {
    0: 'bg-gray-100 text-gray-800',
    1: 'bg-blue-100 text-blue-800',
    2: 'bg-green-100 text-green-800',
    3: 'bg-purple-100 text-purple-800',
    4: 'bg-red-100 text-red-800'
  }
  return colorMap[roleValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const getStatusColorClass = (status: any, unbanTime?: string | null) => {
  if (isUserBanned(unbanTime)) {
    return 'bg-red-100 text-red-800'
  }

  const statusValue = typeof status === 'number' ? status : 0
  const colorMap = {
    0: 'bg-yellow-100 text-yellow-800',
    1: 'bg-green-100 text-green-800'
  }
  return colorMap[statusValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '-'
  return new Intl.DateTimeFormat('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(dateString))
}

// Load data on mount
onMounted(() => {
  loadUsers()
})
</script>
