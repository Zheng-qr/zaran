<template>
  <div>
    <LoadingOverlay v-if="loading" />
    
    <div v-else-if="user" class="space-y-6">
      <!-- Header -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <div class="flex items-center justify-between">
            <div class="flex items-center space-x-4">
              <img 
                class="h-16 w-16 rounded-full" 
                :src="user.avatar || '/default-avatar.png'" 
                :alt="user.nickname"
              >
              <div>
                <h1 class="text-2xl font-bold text-gray-900">{{ user.nickname }}</h1>
                <p class="text-sm text-gray-500">@{{ user.username }}</p>
                <p class="text-sm text-gray-500">{{ user.email }}</p>
                <div class="flex items-center space-x-2 mt-2">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getRoleColorClass(user.userRole)">
                    {{ getRoleName(user.userRole) }}
                  </span>
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusColorClass(user.userStatus)">
                    {{ getStatusName(user.userStatus) }}
                  </span>
                </div>
              </div>
            </div>
            <div class="flex space-x-3">
              <button
                @click="openEditModal"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
              >
                编辑用户
              </button>
              <NuxtLink
                to="/admin/users"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
              >
                返回列表
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>

      <!-- User Details -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Basic Information -->
        <div class="lg:col-span-2">
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">基本信息</h3>
              <dl class="grid grid-cols-1 gap-x-4 gap-y-6 sm:grid-cols-2">
                <div>
                  <dt class="text-sm font-medium text-gray-500">用户名</dt>
                  <dd class="mt-1 text-sm text-gray-900">{{ user.username }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">昵称</dt>
                  <dd class="mt-1 text-sm text-gray-900">{{ user.nickname }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">邮箱</dt>
                  <dd class="mt-1 text-sm text-gray-900">{{ user.email }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">账户余额</dt>
                  <dd class="mt-1 text-sm text-gray-900">¥{{ user.balance?.toFixed(2) || '0.00' }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">注册时间</dt>
                  <dd class="mt-1 text-sm text-gray-900">{{ formatDate(user.createdAt) }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">最后登录</dt>
                  <dd class="mt-1 text-sm text-gray-900">{{ formatDate(user.lastLoginAt) }}</dd>
                </div>
                <div class="sm:col-span-2">
                  <dt class="text-sm font-medium text-gray-500">个性签名</dt>
                  <dd class="mt-1 text-sm text-gray-900">{{ user.signature || '暂无签名' }}</dd>
                </div>
              </dl>
            </div>
          </div>
        </div>

        <!-- Statistics -->
        <div class="space-y-6">
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">统计信息</h3>
              <div class="space-y-4">
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500">发布文章</span>
                  <span class="text-sm font-medium text-gray-900">{{ userStats.articleCount }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500">发布商品</span>
                  <span class="text-sm font-medium text-gray-900">{{ userStats.goodCount }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500">创建集合</span>
                  <span class="text-sm font-medium text-gray-900">{{ userStats.collectionCount }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-sm text-gray-500">交易次数</span>
                  <span class="text-sm font-medium text-gray-900">{{ userStats.transactionCount }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">快速操作</h3>
              <div class="space-y-3">
                <button
                  @click="resetPassword"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  重置密码
                </button>
                <button
                  v-if="user.userStatus !== USER_STATUS.BANNED"
                  @click="banUser"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-red-300 rounded-md shadow-sm text-sm font-medium text-red-700 bg-white hover:bg-red-50"
                >
                  封禁用户
                </button>
                <button
                  v-else
                  @click="unbanUser"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-green-300 rounded-md shadow-sm text-sm font-medium text-green-700 bg-white hover:bg-green-50"
                >
                  解封用户
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">最近活动</h3>
          <div class="flow-root">
            <ul class="-mb-8">
              <li v-for="(activity, index) in recentActivities" :key="index">
                <div class="relative pb-8" :class="{ 'pb-0': index === recentActivities.length - 1 }">
                  <span 
                    v-if="index !== recentActivities.length - 1"
                    class="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200" 
                    aria-hidden="true"
                  />
                  <div class="relative flex space-x-3">
                    <div>
                      <span class="h-8 w-8 rounded-full flex items-center justify-center ring-8 ring-white"
                            :class="activity.iconBg">
                        <svg class="h-5 w-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="activity.iconPath" />
                        </svg>
                      </span>
                    </div>
                    <div class="min-w-0 flex-1 pt-1.5 flex justify-between space-x-4">
                      <div>
                        <p class="text-sm text-gray-500">{{ activity.content }}</p>
                      </div>
                      <div class="text-right text-sm whitespace-nowrap text-gray-500">
                        {{ activity.time }}
                      </div>
                    </div>
                  </div>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-12">
      <h3 class="mt-2 text-sm font-medium text-gray-900">用户不存在</h3>
      <p class="mt-1 text-sm text-gray-500">请检查用户ID是否正确</p>
      <div class="mt-6">
        <NuxtLink
          to="/admin/users"
          class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
        >
          返回用户列表
        </NuxtLink>
      </div>
    </div>

    <!-- Edit User Modal -->
    <AdminUserEditModal
      v-if="showEditModal"
      :user="user"
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

const route = useRoute()
const { getRoleName, getStatusName } = usePermissions()

// Data
const user = ref<UserDetailResponse | null>(null)
const loading = ref(false)
const showEditModal = ref(false)

// Mock user statistics (replace with real API calls)
const userStats = ref({
  articleCount: 0,
  goodCount: 0,
  collectionCount: 0,
  transactionCount: 0
})

// Mock recent activities (replace with real API calls)
const recentActivities = ref([
  {
    content: '发布了新文章《扎染工艺介绍》',
    time: '2小时前',
    iconPath: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z',
    iconBg: 'bg-blue-500'
  },
  {
    content: '创建了新集合《传统工艺》',
    time: '1天前',
    iconPath: 'M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10',
    iconBg: 'bg-green-500'
  }
])

// Load user data
const loadUser = async () => {
  const userId = route.params.id as string
  if (!userId) return

  loading.value = true
  
  try {
    user.value = await UsersService.userDetailEndpoint(userId)
  } catch (error) {
    console.error('Failed to load user:', error)
    user.value = null
  } finally {
    loading.value = false
  }
}

// Modal functions
const openEditModal = () => {
  showEditModal.value = true
}

const closeEditModal = () => {
  showEditModal.value = false
}

const handleUserUpdated = () => {
  closeEditModal()
  loadUser()
}

// User actions
const resetPassword = async () => {
  if (!user.value?.id) return
  if (!confirm(`确定要重置用户 ${user.value.nickname} 的密码吗？`)) return

  try {
    // Generate a random password
    const newPassword = Math.random().toString(36).slice(-8)
    const updateData: UserPatchRequest = { password: newPassword }

    await UsersService.userPatchEndpoint(user.value.id, updateData)
    alert(`密码重置成功！新密码：${newPassword}\n请将此密码安全地传达给用户。`)

    const { success } = useGlobalNotifications()
    success('密码重置成功', '用户密码已重置')
  } catch (error) {
    console.error('Failed to reset password:', error)
    const { error: showError } = useGlobalNotifications()
    showError('重置失败', '密码重置失败，请重试')
  }
}

const banUser = async () => {
  if (!user.value?.id) return
  if (!confirm(`确定要封禁用户 ${user.value.nickname} 吗？`)) return

  try {
    const updateData: UserPatchRequest = { userStatus: USER_STATUS.BANNED }
    await UsersService.userPatchEndpoint(user.value.id, updateData)
    await loadUser()

    const { success } = useGlobalNotifications()
    success('操作成功', '用户已被封禁')
  } catch (error) {
    console.error('Failed to ban user:', error)
    const { error: showError } = useGlobalNotifications()
    showError('操作失败', '封禁用户失败，请重试')
  }
}

const unbanUser = async () => {
  if (!user.value?.id) return
  if (!confirm(`确定要解封用户 ${user.value.nickname} 吗？`)) return

  try {
    const updateData: UserPatchRequest = { userStatus: USER_STATUS.NORMAL }
    await UsersService.userPatchEndpoint(user.value.id, updateData)
    await loadUser()

    const { success } = useGlobalNotifications()
    success('操作成功', '用户已被解封')
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

const getStatusColorClass = (status: any) => {
  const statusValue = typeof status === 'number' ? status : 0
  const colorMap = {
    0: 'bg-yellow-100 text-yellow-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-red-100 text-red-800'
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
  loadUser()
})
</script>
