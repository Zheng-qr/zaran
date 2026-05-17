<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">{{ isAdmin() ? '消息管理' : '我的消息' }}</h1>
        <p class="mt-2 text-sm text-gray-700">
          {{ isAdmin() ? '管理所有用户消息' : '查看您的消息记录' }}
        </p>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white shadow rounded-lg mb-6">
      <div class="px-4 py-5 sm:p-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <!-- Search -->
          <div>
            <label for="search" class="block text-sm font-medium text-gray-700">搜索</label>
            <input
              id="search"
              v-model="filters.keyword"
              type="text"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="搜索消息标题、内容..."
            >
          </div>

          <!-- Type -->
          <div>
            <label for="type" class="block text-sm font-medium text-gray-700">消息类型</label>
            <select
              id="type"
              v-model="filters.type"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">全部类型</option>
              <option v-for="type in messageTypes" :key="type.value" :value="type.value">
                {{ type.name }}
              </option>
            </select>
          </div>

          <!-- Status -->
          <div>
            <label for="status" class="block text-sm font-medium text-gray-700">阅读状态</label>
            <select
              id="status"
              v-model="filters.status"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">全部状态</option>
              <option value="unread">未读</option>
              <option value="read">已读</option>
            </select>
          </div>

          <!-- Sort -->
          <div>
            <label for="sort" class="block text-sm font-medium text-gray-700">排序</label>
            <select
              id="sort"
              v-model="filters.desc"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option :value="true">最新消息</option>
              <option :value="false">最早消息</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Messages list -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <!-- Loading state -->
        <div v-if="loading" class="flex justify-center py-8">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>

        <!-- Empty state -->
        <div v-else-if="messages.length === 0" class="text-center py-8">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">暂无消息</h3>
          <p class="mt-1 text-sm text-gray-500">还没有任何消息记录</p>
        </div>

        <!-- Messages table -->
        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  消息信息
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  发送者
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  接收者
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  类型
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  状态
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  时间
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  操作
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="message in messages" :key="message.id" class="hover:bg-gray-50">
                <td class="px-6 py-4">
                  <div class="flex items-start">
                    <div class="flex-1 min-w-0">
                      <div class="text-sm font-medium text-gray-900 truncate">
                        {{ message.title || '无标题' }}
                      </div>
                      <div class="text-sm text-gray-500 mt-1 line-clamp-2">
                        {{ message.content || '无内容' }}
                      </div>
                    </div>
                    <div v-if="!message.isRead" class="ml-2 flex-shrink-0">
                      <span class="inline-block w-2 h-2 bg-blue-600 rounded-full"></span>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">
                    {{ message.senderName || '系统' }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">
                    {{ message.receiverName || '未知' }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getTypeColorClass(message.type)">
                    {{ getMessageTypeName(message.type) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="message.isRead ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'">
                    {{ message.isRead ? '已读' : '未读' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(message.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <NuxtLink
                      :to="`/admin/messages/${message.id}`"
                      class="text-blue-600 hover:text-blue-900"
                    >
                      查看
                    </NuxtLink>
                    <button
                      v-if="!message.isRead && canMarkAsRead(message)"
                      @click="markAsRead(message)"
                      class="text-green-600 hover:text-green-900"
                    >
                      标记已读
                    </button>
                    <button
                      v-if="canDeleteMessage(message)"
                      @click="deleteMessage(message)"
                      class="text-red-600 hover:text-red-900"
                    >
                      删除
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <div v-if="totalMessages > pageSize" class="mt-6 flex items-center justify-between">
          <div class="text-sm text-gray-700">
            显示第 {{ (currentPage - 1) * pageSize + 1 }} - {{ Math.min(currentPage * pageSize, totalMessages) }} 条，
            共 {{ totalMessages }} 条记录
          </div>
          <div class="flex space-x-2">
            <button
              :disabled="currentPage === 1"
              @click="currentPage--; loadMessages()"
              class="px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              上一页
            </button>
            <button
              :disabled="currentPage * pageSize >= totalMessages"
              @click="currentPage++; loadMessages()"
              class="px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              下一页
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { MessagesService } from '~/api'
import type { MessageDetailResponse } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { createPaginationParams } = useApi()
const { isAdmin } = usePermissions()
const userStore = useUserStore()

// Message constants
const MESSAGE_TYPE = {
  SYSTEM: 0,
  USER: 1,
  NOTIFICATION: 2
}

// Available options
const messageTypes = [
  { value: MESSAGE_TYPE.SYSTEM, name: '系统消息' },
  { value: MESSAGE_TYPE.USER, name: '用户消息' },
  { value: MESSAGE_TYPE.NOTIFICATION, name: '通知消息' }
]

// Data
const messages = ref<MessageDetailResponse[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(20)
const totalMessages = ref(0)

// Filters
const filters = ref({
  keyword: '',
  type: '',
  status: '',
  desc: true
})

// Utility function for debouncing
const useDebounceFn = (fn: (...args: unknown[]) => void, delay: number) => {
  let timeoutId: NodeJS.Timeout
  return (...args: unknown[]) => {
    clearTimeout(timeoutId)
    timeoutId = setTimeout(() => fn(...args), delay)
  }
}

// Load messages
const loadMessages = async () => {
  loading.value = true
  
  try {
    const params = createPaginationParams(
      currentPage.value,
      pageSize.value,
      filters.value.desc,
      filters.value.keyword || undefined
    )
    
    const typeValue = filters.value.type ? parseInt(filters.value.type) : undefined
    
    const response = await MessagesService.messageListEndpoint(
      params.offset,
      params.limit,
      params.desc,
      params.keyword,
      typeValue
    )
    
    messages.value = response.items || []
    totalMessages.value = response.total || 0
  } catch (error) {
    console.error('Failed to load messages:', error)
  } finally {
    loading.value = false
  }
}

// Debounced search
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1
  loadMessages()
}, 500)

// Watch filters
watch(() => filters.value.keyword, debouncedSearch)
watch(() => filters.value.type, () => {
  currentPage.value = 1
  loadMessages()
})
watch(() => filters.value.status, () => {
  currentPage.value = 1
  loadMessages()
})
watch(() => filters.value.desc, () => {
  currentPage.value = 1
  loadMessages()
})

// Message actions
const canMarkAsRead = (message: MessageDetailResponse) => {
  return message.receiverId === userStore.userId || isAdmin()
}

const canDeleteMessage = (message: MessageDetailResponse) => {
  return isAdmin() || message.senderId === userStore.userId || message.receiverId === userStore.userId
}

const markAsRead = async (message: MessageDetailResponse) => {
  if (!message.id) return
  
  try {
    await MessagesService.messageReadEndpoint(message.id)
    
    const { success } = useGlobalNotifications()
    success('操作成功', '消息已标记为已读')
    loadMessages()
  } catch (error) {
    console.error('Failed to mark message as read:', error)
    const { error: showError } = useGlobalNotifications()
    showError('操作失败', '标记已读失败，请重试')
  }
}

const deleteMessage = async (message: MessageDetailResponse) => {
  if (!message.id) return
  if (!confirm(`确定要删除这条消息吗？此操作不可恢复。`)) return
  
  try {
    await MessagesService.messageDeleteEndpoint(message.id)
    
    const { success } = useGlobalNotifications()
    success('删除成功', '消息已删除')
    loadMessages()
  } catch (error) {
    console.error('Failed to delete message:', error)
    const { error: showError } = useGlobalNotifications()
    showError('删除失败', '消息删除失败，请重试')
  }
}

// Utility functions
const getMessageTypeName = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const typeObj = messageTypes.find(t => t.value === typeValue)
  return typeObj?.name || '未知类型'
}

const getTypeColorClass = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const colorMap = {
    0: 'bg-blue-100 text-blue-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-yellow-100 text-yellow-800'
  }
  return colorMap[typeValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '未知时间'
  return new Date(dateString).toLocaleString('zh-CN')
}

// Load messages on mount
onMounted(() => {
  loadMessages()
})
</script>
