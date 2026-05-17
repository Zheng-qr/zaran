<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">评论管理</h1>
        <p class="mt-2 text-sm text-gray-700">管理所有评论内容，进行内容审核</p>
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
              placeholder="搜索评论内容、用户..."
            >
          </div>

          <!-- Target Type -->
          <div>
            <label for="targetType" class="block text-sm font-medium text-gray-700">目标类型</label>
            <select
              id="targetType"
              v-model="filters.targetType"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">全部类型</option>
              <option value="article">文章评论</option>
              <option value="good">商品评论</option>
              <option value="user">用户评论</option>
            </select>
          </div>

          <!-- Status -->
          <div>
            <label for="status" class="block text-sm font-medium text-gray-700">审核状态</label>
            <select
              id="status"
              v-model="filters.status"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">全部状态</option>
              <option value="pending">待审核</option>
              <option value="approved">已通过</option>
              <option value="rejected">已拒绝</option>
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
              <option :value="true">最新评论</option>
              <option :value="false">最早评论</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Comments list -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <!-- Loading state -->
        <div v-if="loading" class="flex justify-center py-8">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        </div>

        <!-- Empty state -->
        <div v-else-if="comments.length === 0" class="text-center py-8">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">暂无评论</h3>
          <p class="mt-1 text-sm text-gray-500">还没有任何评论记录</p>
        </div>

        <!-- Comments list -->
        <div v-else class="space-y-4">
          <div
            v-for="comment in comments"
            :key="comment.id"
            class="border border-gray-200 rounded-lg p-4 hover:shadow-sm transition-shadow"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1 min-w-0">
                <!-- Comment header -->
                <div class="flex items-center space-x-3 mb-2">
                  <div class="text-sm font-medium text-gray-900">
                    {{ comment.senderName || '匿名用户' }}
                  </div>
                  <div class="text-sm text-gray-500">
                    评论于 {{ formatDate(comment.createdAt) }}
                  </div>
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusColorClass(comment.status)">
                    {{ getCommentStatusName(comment.status) }}
                  </span>
                </div>

                <!-- Comment content -->
                <div class="text-sm text-gray-700 mb-3">
                  {{ comment.content || '无内容' }}
                </div>

                <!-- Target info -->
                <div class="text-xs text-gray-500">
                  目标ID: {{ comment.targetId }}
                </div>
              </div>

              <!-- Actions -->
              <div class="ml-4 flex-shrink-0">
                <div class="flex space-x-2">
                  <button
                    v-if="canApproveComment(comment)"
                    @click="approveComment(comment)"
                    class="text-green-600 hover:text-green-900 text-sm"
                  >
                    通过
                  </button>
                  <button
                    v-if="canRejectComment(comment)"
                    @click="rejectComment(comment)"
                    class="text-yellow-600 hover:text-yellow-900 text-sm"
                  >
                    拒绝
                  </button>
                  <button
                    @click="deleteComment(comment)"
                    class="text-red-600 hover:text-red-900 text-sm"
                  >
                    删除
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <div v-if="totalComments > pageSize" class="mt-6 flex items-center justify-between">
          <div class="text-sm text-gray-700">
            显示第 {{ (currentPage - 1) * pageSize + 1 }} - {{ Math.min(currentPage * pageSize, totalComments) }} 条，
            共 {{ totalComments }} 条记录
          </div>
          <div class="flex space-x-2">
            <button
              :disabled="currentPage === 1"
              @click="currentPage--; loadComments()"
              class="px-3 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              上一页
            </button>
            <button
              :disabled="currentPage * pageSize >= totalComments"
              @click="currentPage++; loadComments()"
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
import { CommentsService } from '~/api'
import type { CommentDetailResponse, CommentPatchEndpointRequest } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const { createPaginationParams } = useApi()
const { isAdmin } = usePermissions()

// Comment constants
const COMMENT_STATUS = {
  PENDING: 0,
  APPROVED: 1,
  REJECTED: 2
}

// Data
const comments = ref<CommentDetailResponse[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(20)
const totalComments = ref(0)

// Filters
const filters = ref({
  keyword: '',
  targetType: '',
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

// Load comments - Note: This is a simplified implementation
// In reality, we would need an admin endpoint to get all comments
const loadComments = async () => {
  loading.value = true
  
  try {
    // This is a placeholder - we would need a proper admin comments endpoint
    // For now, we'll show an empty list
    comments.value = []
    totalComments.value = 0
    
    // TODO: Implement proper admin comments API
    console.log('Loading comments with filters:', filters.value)
  } catch (error) {
    console.error('Failed to load comments:', error)
  } finally {
    loading.value = false
  }
}

// Debounced search
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1
  loadComments()
}, 500)

// Watch filters
watch(() => filters.value.keyword, debouncedSearch)
watch(() => filters.value.targetType, () => {
  currentPage.value = 1
  loadComments()
})
watch(() => filters.value.status, () => {
  currentPage.value = 1
  loadComments()
})
watch(() => filters.value.desc, () => {
  currentPage.value = 1
  loadComments()
})

// Comment actions
const canApproveComment = (comment: CommentDetailResponse) => {
  return isAdmin() && comment.status === COMMENT_STATUS.PENDING
}

const canRejectComment = (comment: CommentDetailResponse) => {
  return isAdmin() && comment.status === COMMENT_STATUS.PENDING
}

const approveComment = async (comment: CommentDetailResponse) => {
  if (!comment.id) return
  
  try {
    const updateData: CommentPatchEndpointRequest = { status: COMMENT_STATUS.APPROVED }
    await CommentsService.commentPatchEndpoint(comment.id, updateData)
    
    const { success } = useGlobalNotifications()
    success('操作成功', '评论已通过审核')
    loadComments()
  } catch (error) {
    console.error('Failed to approve comment:', error)
    const { error: showError } = useGlobalNotifications()
    showError('操作失败', '评论审核失败，请重试')
  }
}

const rejectComment = async (comment: CommentDetailResponse) => {
  if (!comment.id) return
  
  try {
    const updateData: CommentPatchEndpointRequest = { status: COMMENT_STATUS.REJECTED }
    await CommentsService.commentPatchEndpoint(comment.id, updateData)
    
    const { success } = useGlobalNotifications()
    success('操作成功', '评论已拒绝')
    loadComments()
  } catch (error) {
    console.error('Failed to reject comment:', error)
    const { error: showError } = useGlobalNotifications()
    showError('操作失败', '评论拒绝失败，请重试')
  }
}

const deleteComment = async (comment: CommentDetailResponse) => {
  if (!comment.id) return
  if (!confirm(`确定要删除这条评论吗？此操作不可恢复。`)) return
  
  try {
    await CommentsService.commentDeleteEndpoint(comment.id)
    
    const { success } = useGlobalNotifications()
    success('删除成功', '评论已删除')
    loadComments()
  } catch (error) {
    console.error('Failed to delete comment:', error)
    const { error: showError } = useGlobalNotifications()
    showError('删除失败', '评论删除失败，请重试')
  }
}

// Utility functions
const getCommentStatusName = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const statusMap = {
    0: '待审核',
    1: '已通过',
    2: '已拒绝'
  }
  return statusMap[statusValue as keyof typeof statusMap] || '未知状态'
}

const getStatusColorClass = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const colorMap = {
    0: 'bg-yellow-100 text-yellow-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-red-100 text-red-800'
  }
  return colorMap[statusValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '未知时间'
  return new Date(dateString).toLocaleString('zh-CN')
}

// Load comments on mount
onMounted(() => {
  loadComments()
})
</script>
