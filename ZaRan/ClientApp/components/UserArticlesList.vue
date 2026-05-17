<template>
  <div class="user-articles-list">
    <!-- 加载状态 -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>加载文章中...</p>
    </div>

    <!-- 错误状态 -->
    <div v-else-if="error" class="error-container">
      <p class="error-message">{{ error }}</p>
      <button @click="loadArticles" class="retry-btn">重试</button>
    </div>

    <!-- 空状态 -->
    <div v-else-if="articles.length === 0" class="empty-container">
      <div class="empty-icon">📝</div>
      <p class="empty-message">{{ emptyMessage || '暂无文章' }}</p>
    </div>

    <!-- 文章列表 -->
    <div v-else class="articles-grid">
      <div 
        v-for="article in articles" 
        :key="article.id" 
        class="article-card"
        @click="goToArticle(article)"
      >
        <div class="article-image" v-if="article.imageSmallUrl || article.imageUrl">
          <img 
            :src="article.imageSmallUrl || article.imageUrl" 
            :alt="article.title" 
            loading="lazy"
          >
        </div>
        <div class="article-content">
          <h3 class="article-title">{{ article.title }}</h3>
          <p class="article-summary">{{ article.summary || '暂无摘要' }}</p>
          <div class="article-meta">
            <span class="article-date">{{ formatDate(article.createdAt!) }}</span>
            <span class="article-author" v-if="article.author && showAuthor">
              {{ article.author.nickname || article.author.username }}
            </span>
            <span class="article-type" v-if="showType">
              {{ getArticleTypeName(article.type) }}
            </span>
          </div>
          <div class="article-tags" v-if="article.tags && article.tags.length > 0">
            <span 
              v-for="tag in article.tags.slice(0, 3)" 
              :key="tag" 
              class="tag"
            >
              {{ tag }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- 分页 -->
    <div v-if="showPagination && totalPages > 1" class="pagination">
      <button 
        @click="goToPage(currentPage - 1)" 
        :disabled="currentPage <= 1"
        class="pagination-btn"
      >
        上一页
      </button>
      <span class="pagination-info">
        第 {{ currentPage }} 页，共 {{ totalPages }} 页
      </span>
      <button 
        @click="goToPage(currentPage + 1)" 
        :disabled="currentPage >= totalPages"
        class="pagination-btn"
      >
        下一页
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ArticleDetailResponse } from '~/api'

interface Props {
  userId: string
  articleType?: string
  pageSize?: number
  showAuthor?: boolean
  showType?: boolean
  showPagination?: boolean
  emptyMessage?: string
}

const props = withDefaults(defineProps<Props>(), {
  articleType: 'story',
  pageSize: 12,
  showAuthor: false,
  showType: false,
  showPagination: true,
  emptyMessage: '暂无文章'
})

const emit = defineEmits<{
  articleClick: [article: ArticleDetailResponse]
}>()

// 使用 composables
const { articles, loading, error, total, fetchUserArticles } = useArticles(props.articleType)

// 分页状态
const currentPage = ref(1)
const totalPages = computed(() => Math.ceil(total.value / props.pageSize))

// 加载文章
const loadArticles = async () => {
  await fetchUserArticles(props.userId, currentPage.value, props.pageSize)
}

// 分页处理
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    loadArticles()
  }
}

// 点击文章
const goToArticle = (article: ArticleDetailResponse) => {
  emit('articleClick', article)
  navigateTo(`/articles/${article.id}`)
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN')
}

// 获取文章类型名称
const getArticleTypeName = (type?: number) => {
  const typeMap: Record<number, string> = {
    0: '故事',
    1: '角色',
    2: '基因',
    3: '百科',
    4: '帖子',
    5: '作品',
  }
  return typeMap[type || 0] || '未知'
}

// 监听 userId 变化
watch(() => props.userId, () => {
  currentPage.value = 1
  loadArticles()
}, { immediate: true })

// 监听 articleType 变化
watch(() => props.articleType, () => {
  currentPage.value = 1
  loadArticles()
})
</script>

<style scoped>
.user-articles-list {
  width: 100%;
}

.loading-container,
.error-container,
.empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  text-align: center;
}

.loading-spinner {
  width: 2rem;
  height: 2rem;
  border: 2px solid #f3f4f6;
  border-top: 2px solid #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.error-message {
  color: #ef4444;
  margin-bottom: 1rem;
}

.retry-btn {
  padding: 0.5rem 1rem;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.retry-btn:hover {
  background-color: #2563eb;
}

.empty-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.empty-message {
  color: #6b7280;
}

.articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.article-card {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.article-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.article-image {
  width: 100%;
  height: 200px;
  overflow: hidden;
}

.article-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.article-content {
  padding: 1rem;
}

.article-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.5rem;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.article-summary {
  color: #6b7280;
  font-size: 0.875rem;
  line-height: 1.5;
  margin-bottom: 0.75rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.article-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  font-size: 0.75rem;
  color: #9ca3af;
  margin-bottom: 0.5rem;
}

.article-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
}

.tag {
  background-color: #f3f4f6;
  color: #374151;
  padding: 0.125rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.pagination-btn {
  padding: 0.5rem 1rem;
  background-color: #f9fafb;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.pagination-btn:hover:not(:disabled) {
  background-color: #f3f4f6;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  color: #6b7280;
  font-size: 0.875rem;
}
</style>
