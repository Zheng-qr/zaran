<template>
  <div class="comment-section">
    <div class="comment-header">
      <h3 class="comment-title">
        <i class="fas fa-comments"></i>
        评论 ({{ total }})
      </h3>
    </div>

    <!-- 发表评论 -->
    <div v-if="userStore.isLoggedIn" class="comment-form">
      <div class="comment-input-wrapper">
        <textarea
          v-model="newComment"
          placeholder="写下你的评论..."
          class="comment-input"
          rows="3"
          maxlength="1000"
        ></textarea>
        <div class="comment-actions">
          <span class="char-count">{{ newComment.length }}/1000</span>
          <button 
            @click="submitComment" 
            :disabled="!newComment.trim() || submitting"
            class="submit-btn"
          >
            <span v-if="submitting" class="loading-spinner"></span>
            {{ submitting ? '发表中...' : '发表评论' }}
          </button>
        </div>
      </div>
    </div>

    <!-- 未登录提示 -->
    <div v-else class="login-prompt">
      <p>
        <i class="fas fa-user"></i>
        请 <NuxtLink to="/login" class="login-link">登录</NuxtLink> 后发表评论
      </p>
    </div>

    <!-- 评论列表 -->
    <div class="comment-list">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-state">
        <div class="loading-spinner"></div>
        <p>加载评论中...</p>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="error-state">
        <i class="fas fa-exclamation-triangle"></i>
        <p>{{ error }}</p>
        <button @click="loadComments" class="retry-btn">重试</button>
      </div>

      <!-- 评论项 -->
      <div v-else-if="comments.length > 0" class="comments">
        <div v-for="comment in comments" :key="comment.id" class="comment-item">
          <div class="comment-avatar">
            <img 
              :src="comment.sender?.avatar || '/image/default-avatar.jpg'" 
              :alt="comment.sender?.nickname"
              class="avatar-img"
            >
          </div>
          <div class="comment-content">
            <div class="comment-header">
              <span class="author-name">{{ comment.sender?.nickname }}</span>
              <span class="comment-time">{{ formatCommentTime(comment.createdAt) }}</span>
            </div>
            <div class="comment-text">{{ comment.content }}</div>
            <div class="comment-actions" v-if="userStore.user?.id === comment.sender?.id">
              <button @click="editComment(comment)" class="edit-btn">
                <i class="fas fa-edit"></i>
                编辑
              </button>
              <button @click="deleteCommentConfirm(comment.id)" class="delete-btn">
                <i class="fas fa-trash"></i>
                删除
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-else class="empty-state">
        <i class="fas fa-comment-slash"></i>
        <p>暂无评论，快来发表第一条评论吧！</p>
      </div>
    </div>

    <!-- 分页 -->
    <div v-if="totalPages > 1" class="pagination">
      <button 
        @click="loadPage(currentPage - 1)"
        :disabled="currentPage <= 1"
        class="page-btn"
      >
        <i class="fas fa-chevron-left"></i>
        上一页
      </button>
      <span class="page-info">{{ currentPage }} / {{ totalPages }}</span>
      <button 
        @click="loadPage(currentPage + 1)"
        :disabled="currentPage >= totalPages"
        class="page-btn"
      >
        下一页
        <i class="fas fa-chevron-right"></i>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { CommentDetailResponse } from '~/api'

interface Props {
  targetId: string
}

const props = defineProps<Props>()
const userStore = useUserStore()

// 使用评论 composable
const { 
  comments, 
  loading, 
  error, 
  total, 
  totalPages, 
  fetchComments, 
  createComment, 
  updateComment, 
  deleteComment, 
  formatCommentTime 
} = useComments(props.targetId)

// 本地状态
const newComment = ref('')
const submitting = ref(false)
const currentPage = ref(1)
const editingComment = ref<CommentDetailResponse | null>(null)

// 加载评论
const loadComments = () => {
  fetchComments(currentPage.value)
}

// 加载指定页
const loadPage = (page: number) => {
  currentPage.value = page
  loadComments()
}

// 提交评论
const submitComment = async () => {
  if (!newComment.value.trim()) return

  submitting.value = true
  try {
    const success = await createComment(newComment.value, props.targetId)
    if (success) {
      newComment.value = ''
      // 重新加载评论列表
      loadComments()
    }
  } finally {
    submitting.value = false
  }
}

// 编辑评论
const editComment = (comment: CommentDetailResponse) => {
  editingComment.value = comment
  newComment.value = comment.content
}

// 删除评论确认
const deleteCommentConfirm = async (commentId: string) => {
  if (confirm('确定要删除这条评论吗？')) {
    const success = await deleteComment(commentId)
    if (success) {
      // 如果当前页没有评论了，回到上一页
      if (comments.value.length === 1 && currentPage.value > 1) {
        currentPage.value--
      }
      loadComments()
    }
  }
}

// 页面加载时获取评论
onMounted(() => {
  loadComments()
})
</script>

<style scoped>
.comment-section {
  margin-top: 32px;
  padding: 24px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.comment-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 24px;
  color: #2d3748;
  display: flex;
  align-items: center;
  gap: 8px;
}

.comment-form {
  margin-bottom: 32px;
}

.comment-input-wrapper {
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  overflow: hidden;
}

.comment-input {
  width: 100%;
  padding: 16px;
  border: none;
  resize: vertical;
  font-family: inherit;
  font-size: 0.9rem;
  line-height: 1.5;
}

.comment-input:focus {
  outline: none;
  border-color: #667eea;
}

.comment-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: #f7fafc;
  border-top: 1px solid #e2e8f0;
}

.char-count {
  font-size: 0.8rem;
  color: #718096;
}

.submit-btn {
  background: #667eea;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  display: flex;
  align-items: center;
  gap: 8px;
}

.submit-btn:hover:not(:disabled) {
  background: #5a6fd8;
}

.submit-btn:disabled {
  background: #cbd5e0;
  cursor: not-allowed;
}

.login-prompt {
  text-align: center;
  padding: 24px;
  background: #f7fafc;
  border-radius: 8px;
  margin-bottom: 32px;
}

.login-link {
  color: #667eea;
  text-decoration: none;
}

.login-link:hover {
  text-decoration: underline;
}

.loading-state, .error-state, .empty-state {
  text-align: center;
  padding: 40px;
  color: #718096;
}

.loading-spinner {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 2px solid #f3f3f3;
  border-radius: 50%;
  border-top-color: #667eea;
  animation: spin 1s ease-in-out infinite;
  margin-right: 8px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.comment-item {
  display: flex;
  gap: 12px;
  padding: 16px 0;
  border-bottom: 1px solid #e2e8f0;
}

.comment-item:last-child {
  border-bottom: none;
}

.comment-avatar {
  flex-shrink: 0;
}

.avatar-img {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
}

.comment-content {
  flex: 1;
}

.comment-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 8px;
}

.author-name {
  font-weight: 600;
  color: #2d3748;
}

.comment-time {
  font-size: 0.8rem;
  color: #718096;
}

.comment-text {
  color: #4a5568;
  line-height: 1.6;
  margin-bottom: 8px;
}

.comment-actions {
  display: flex;
  gap: 12px;
}

.edit-btn, .delete-btn {
  background: none;
  border: none;
  color: #718096;
  cursor: pointer;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  gap: 4px;
}

.edit-btn:hover {
  color: #667eea;
}

.delete-btn:hover {
  color: #e53e3e;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 16px;
  margin-top: 24px;
}

.page-btn {
  background: #667eea;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
}

.page-btn:hover:not(:disabled) {
  background: #5a6fd8;
}

.page-btn:disabled {
  background: #cbd5e0;
  cursor: not-allowed;
}

.page-info {
  color: #718096;
  font-size: 0.9rem;
}

.retry-btn {
  background: #667eea;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  margin-top: 16px;
}

.retry-btn:hover {
  background: #5a6fd8;
}
</style>
