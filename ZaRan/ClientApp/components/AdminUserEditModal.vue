<template>
  <div class="fixed inset-0 z-50 overflow-y-auto">
    <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
      <!-- Background overlay -->
      <div 
        class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"
        @click="$emit('close')"
      ></div>

      <!-- Modal panel -->
      <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
        <form @submit.prevent="handleSubmit">
          <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
            <div class="sm:flex sm:items-start">
              <div class="mt-3 text-center sm:mt-0 sm:text-left w-full">
                <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">
                  编辑用户信息
                </h3>
                
                <!-- User basic info (read-only) -->
                <div class="mb-4 p-4 bg-gray-50 rounded-md">
                  <div class="flex items-center space-x-3">
                    <img 
                      class="h-12 w-12 rounded-full" 
                      :src="user?.avatar || '/default-avatar.png'" 
                      :alt="user?.nickname"
                    >
                    <div>
                      <p class="text-sm font-medium text-gray-900">{{ user?.nickname }}</p>
                      <p class="text-sm text-gray-500">@{{ user?.username }}</p>
                      <p class="text-sm text-gray-500">{{ user?.email }}</p>
                    </div>
                  </div>
                </div>

                <!-- Role selection -->
                <div class="mb-4">
                  <label for="userRole" class="block text-sm font-medium text-gray-700 mb-2">
                    用户角色
                  </label>
                  <select
                    id="userRole"
                    v-model="formData.userRole"
                    class="block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    required
                  >
                    <option v-for="role in availableRoles" :key="role.value" :value="role.value">
                      {{ role.name }}
                    </option>
                  </select>
                  <p class="mt-1 text-xs text-gray-500">
                    角色决定用户在系统中的权限级别
                  </p>
                </div>

                <!-- Status selection -->
                <div class="mb-4">
                  <label for="userStatus" class="block text-sm font-medium text-gray-700 mb-2">
                    账户状态
                  </label>
                  <select
                    id="userStatus"
                    v-model="formData.userStatus"
                    class="block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    required
                  >
                    <option v-for="status in availableStatuses" :key="status.value" :value="status.value">
                      {{ status.name }}
                    </option>
                  </select>
                  <p class="mt-1 text-xs text-gray-500">
                    控制用户是否可以正常使用系统功能
                  </p>
                </div>

                <!-- Unban time -->
                <div class="mb-4">
                  <label for="unbanTime" class="block text-sm font-medium text-gray-700 mb-2">
                    解封时间
                  </label>
                  <input
                    id="unbanTime"
                    v-model="formData.unbanTime"
                    type="datetime-local"
                    class="block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  >
                  <p class="mt-1 text-xs text-gray-500">
                    设置用户解封时间。留空表示用户未被封禁。当前时间之前的时间表示用户已解封。
                  </p>
                  <div v-if="user?.unbanTime" class="mt-1 text-xs">
                    <span :class="isCurrentlyBanned ? 'text-red-600' : 'text-green-600'">
                      当前状态：{{ isCurrentlyBanned ? '已封禁' : '未封禁' }}
                    </span>
                    <span v-if="user.unbanTime" class="text-gray-500 ml-2">
                      (解封时间：{{ formatDateTime(user.unbanTime) }})
                    </span>
                  </div>
                </div>

                <!-- Balance adjustment -->
                <div class="mb-4">
                  <label for="balance" class="block text-sm font-medium text-gray-700 mb-2">
                    账户余额
                  </label>
                  <div class="relative">
                    <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                      <span class="text-gray-500 sm:text-sm">¥</span>
                    </div>
                    <input
                      id="balance"
                      v-model.number="formData.balance"
                      type="number"
                      step="0.01"
                      min="0"
                      class="block w-full pl-7 border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                      placeholder="0.00"
                    >
                  </div>
                  <p class="mt-1 text-xs text-gray-500">
                    当前余额：¥{{ user?.balance?.toFixed(2) || '0.00' }}
                  </p>
                </div>

                <!-- Password reset option -->
                <div class="mb-4">
                  <div class="flex items-center">
                    <input
                      id="resetPassword"
                      v-model="formData.resetPassword"
                      type="checkbox"
                      class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                    >
                    <label for="resetPassword" class="ml-2 block text-sm text-gray-900">
                      重置用户密码
                    </label>
                  </div>
                  <p class="mt-1 text-xs text-gray-500">
                    勾选此项将为用户生成新的随机密码
                  </p>
                </div>

                <!-- Error message -->
                <div v-if="error" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-md">
                  <p class="text-sm text-red-600">{{ error }}</p>
                </div>
              </div>
            </div>
          </div>
          
          <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
            <button
              type="submit"
              :disabled="loading"
              class="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-blue-600 text-base font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50"
            >
              <svg v-if="loading" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              {{ loading ? '保存中...' : '保存更改' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              :disabled="loading"
              class="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50"
            >
              取消
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { UsersService } from '~/api'
import type { UserDetailResponse, UserPatchRequest } from '~/api'

interface Props {
  user: UserDetailResponse | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  updated: []
}>()

const { getAllRoles, getAllStatuses } = usePermissions()

// Helper functions
const formatDateTime = (dateString: string): string => {
  return new Date(dateString).toLocaleString('zh-CN')
}

const formatDateTimeForInput = (dateString: string): string => {
  const date = new Date(dateString)
  return date.toISOString().slice(0, 16) // Format for datetime-local input
}

// Form data
const formData = ref({
  userRole: props.user?.userRole || 0,
  userStatus: props.user?.userStatus || 0,
  balance: props.user?.balance || 0,
  unbanTime: props.user?.unbanTime ? formatDateTimeForInput(props.user.unbanTime) : '',
  resetPassword: false
})

// State
const loading = ref(false)
const error = ref('')

// Available options
const availableRoles = getAllRoles()
const availableStatuses = getAllStatuses()

// Computed properties
const isCurrentlyBanned = computed(() => {
  if (!props.user?.unbanTime) return false
  const unbanTime = new Date(props.user.unbanTime)
  const now = new Date()
  return unbanTime > now
})

// Initialize form data when user prop changes
watch(() => props.user, (newUser) => {
  if (newUser) {
    formData.value = {
      userRole: newUser.userRole || 0,
      userStatus: newUser.userStatus || 0,
      balance: newUser.balance || 0,
      unbanTime: newUser.unbanTime ? formatDateTimeForInput(newUser.unbanTime) : '',
      resetPassword: false
    }
  }
}, { immediate: true })

// Handle form submission
const handleSubmit = async () => {
  if (!props.user?.id) return

  loading.value = true
  error.value = ''

  try {
    // Prepare update data
    const updateData: UserPatchRequest = {}

    // Update role if changed
    if (formData.value.userRole !== props.user.userRole) {
      updateData.userRole = formData.value.userRole
    }

    // Update status if changed
    if (formData.value.userStatus !== props.user.userStatus) {
      updateData.userStatus = formData.value.userStatus
    }

    // Update balance if changed
    if (formData.value.balance !== props.user.balance) {
      updateData.balance = formData.value.balance
    }

    // Update unban time if changed
    const currentUnbanTime = props.user.unbanTime ? formatDateTimeForInput(props.user.unbanTime) : ''
    if (formData.value.unbanTime !== currentUnbanTime) {
      updateData.unbanTime = formData.value.unbanTime ? new Date(formData.value.unbanTime).toISOString() : null
    }

    // Reset password if requested
    if (formData.value.resetPassword) {
      // Generate a random password
      const newPassword = Math.random().toString(36).slice(-8)
      updateData.password = newPassword

      // Show the new password to admin
      alert(`用户密码已重置。新密码：${newPassword}\n请将此密码安全地传达给用户。`)
    }

    // Apply updates if there are any changes
    if (Object.keys(updateData).length > 0) {
      await UsersService.userPatchEndpoint(props.user.id, updateData)
    }

    const { success } = useGlobalNotifications()
    success('更新成功', '用户信息已成功更新')
    emit('updated')
  } catch (err) {
    console.error('Failed to update user:', err)
    error.value = '更新用户信息失败，请重试'
    const { error: showError } = useGlobalNotifications()
    showError('更新失败', '用户信息更新失败，请重试')
  } finally {
    loading.value = false
  }
}

// Close modal on escape key
onMounted(() => {
  const handleEscape = (e: KeyboardEvent) => {
    if (e.key === 'Escape') {
      emit('close')
    }
  }
  
  document.addEventListener('keydown', handleEscape)
  
  onUnmounted(() => {
    document.removeEventListener('keydown', handleEscape)
  })
})
</script>
