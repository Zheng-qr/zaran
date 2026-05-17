<template>
    <div class="ip-story-page">
        <div class="container">
            <!-- 面包屑导航 -->
            <nav class="breadcrumb">
                <NuxtLink to="/ip-story" class="breadcrumb-item">
                    <i class="fas fa-home"></i> IP故事
                </NuxtLink>
                <span class="breadcrumb-separator">/</span>
                <span class="breadcrumb-item current">
                    {{ collection?.name || '加载中...' }}
                </span>
            </nav>

            <!-- 集合信息 -->
            <div v-if="collection" class="collection-header">
                <h1 class="page-title">{{ collection.name }}</h1>
                <p v-if="collection.summary" class="page-subtitle">{{ collection.summary }}</p>

                <!-- Markdown 描述区域 -->
                <div v-if="collection.description" class="collection-description">
                    <div class="description-content" v-html="renderedDescription"></div>
                </div>
            </div>

            <!-- 搜索栏 -->
            <div class="search-section">
                <div class="search-box">
                    <input v-model="searchKeyword" @keyup.enter="handleSearch()" type="text"
                        placeholder="搜索故事..." class="search-input" />
                    <button @click="handleSearch()" class="search-btn">
                        <i class="fas fa-search"></i>
                    </button>
                </div>
            </div>

            <!-- 加载状态 -->
            <div v-if="articlesLoading" class="loading">
                <div class="spinner"></div>
                <p>正在加载故事...</p>
            </div>

            <!-- 错误状态 -->
            <div v-else-if="articlesError" class="error">
                <i class="fas fa-exclamation-triangle"></i>
                <p>{{ articlesError }}</p>
                <button @click="loadArticles" class="retry-btn">重试</button>
            </div>

            <!-- 文章网格 -->
            <div v-else-if="articles.length > 0" class="articles-grid">
                <NuxtLink v-for="article in articles" :key="article.id"
                    :to="`/articles/${article.id}`" class="article-card">
                    <div class="article-image" v-if="article.imageSmallUrl">
                        <img :src="article.imageSmallUrl" :alt="article.title" loading="lazy">
                    </div>
                    <div class="article-content">
                        <h3 class="article-title">{{ article.title }}</h3>
                        <p class="article-summary">{{ article.summary }}</p>
                        <div class="article-meta">
                            <span class="article-date">{{ formatDate(article.createdAt!) }}</span>
                            <span class="article-author" v-if="article.author">
                                {{ article.author.nickname }}
                            </span>
                        </div>
                        <div class="article-tags" v-if="article.tags && article.tags.length > 0">
                            <span v-for="tag in article.tags.slice(0, 3)" :key="tag" class="tag">
                                {{ tag }}
                            </span>
                        </div>
                    </div>
                </NuxtLink>
            </div>

            <!-- 空状态 -->
            <div v-else class="empty-state">
                <i class="fas fa-newspaper"></i>
                <p>暂无故事内容</p>
            </div>

            <!-- 分页 -->
            <div v-if="totalPages > 1" class="pagination">
                <button @click="changePage(currentPage - 1)" :disabled="currentPage <= 1" class="page-btn">
                    上一页
                </button>
                <span class="page-info">{{ currentPage }} / {{ totalPages }}</span>
                <button @click="changePage(currentPage + 1)" :disabled="currentPage >= totalPages" class="page-btn">
                    下一页
                </button>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { CollectionDetailResponse, ArticleDetailResponse } from '~/api'
import { marked } from 'marked'

// 获取路由参数
const route = useRoute()
const collectionId = route.params.collectionId as string

// 状态管理
const collection = ref<CollectionDetailResponse | null>(null)

// 页面元数据
useHead({
  title: computed(() => collection.value ? `${collection.value.name} - IP故事` : 'IP故事'),
  meta: [
    { name: 'description', content: computed(() => collection.value?.description || '探索IP故事') }
  ]
})
const articles = ref<ArticleDetailResponse[]>([])
const articlesLoading = ref(false)
const articlesError = ref<string | null>(null)
const total = ref(0)

// 分页和搜索
const currentPage = ref(1)
const searchKeyword = ref('')

// 计算总页数
const totalPages = computed(() => {
  return Math.ceil(total.value / 12)
})

// 渲染 Markdown 描述
const renderedDescription = computed(() => {
  if (!collection.value?.description) return ''
  return marked(collection.value.description)
})

// 加载集合信息
const loadCollection = async () => {
  try {
    const { getCollectionById } = useCollections()
    const result = await getCollectionById(collectionId)
    if (result) {
      collection.value = result
    }
  } catch (err) {
    console.error('加载集合信息失败:', err)
  }
}

// 加载文章列表
const loadArticles = async () => {
  articlesLoading.value = true
  articlesError.value = null

  try {
    const { getCollectionItems } = useCollections()
    const response = await getCollectionItems(
      collectionId,
      currentPage.value,
      12
    )

    if (response) {
      articles.value = response.items as ArticleDetailResponse[]
      total.value = response.total
    } else {
      articles.value = []
      total.value = 0
    }
  } catch (err) {
    articlesError.value = '加载文章失败，请稍后重试'
    console.error('加载文章失败:', err)
    articles.value = []
    total.value = 0
  } finally {
    articlesLoading.value = false
  }
}

// 搜索处理
const handleSearch = () => {
  currentPage.value = 1
  loadArticles()
}

// 分页处理
const changePage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    loadArticles()
  }
}

// 工具函数
const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// 页面加载时获取数据
onMounted(async () => {
  await loadCollection()
  await loadArticles()
})

// 监听路由变化
watch(() => route.params.collectionId, async (newId) => {
  if (newId && newId !== collectionId) {
    currentPage.value = 1
    searchKeyword.value = ''
    await loadCollection()
    await loadArticles()
  }
})
</script>

<style scoped>
.ip-story-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.breadcrumb {
  display: flex;
  align-items: center;
  margin-bottom: 2rem;
  font-size: 0.9rem;
}

.breadcrumb-item {
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  transition: color 0.3s ease;
}

.breadcrumb-item:hover {
  color: white;
}

.breadcrumb-item.current {
  color: white;
  font-weight: 500;
}

.breadcrumb-separator {
  margin: 0 0.5rem;
  color: rgba(255, 255, 255, 0.6);
}

.collection-header {
  text-align: center;
  margin-bottom: 2rem;
}

.page-title {
  font-size: 2.5rem;
  font-weight: bold;
  margin-bottom: 1rem;
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.page-subtitle {
  font-size: 1.2rem;
  color: rgba(255, 255, 255, 0.9);
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
  margin-bottom: 1rem;
}

.collection-description {
  max-width: 800px;
  margin: 0 auto;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  margin-bottom: 2rem;
}

.description-content {
  color: #333;
  line-height: 1.8;
}

.description-content h1,
.description-content h2,
.description-content h3,
.description-content h4,
.description-content h5,
.description-content h6 {
  color: #2c3e50;
  margin-top: 1.5rem;
  margin-bottom: 1rem;
  font-weight: 600;
}

.description-content h1 {
  font-size: 1.8rem;
  border-bottom: 2px solid #667eea;
  padding-bottom: 0.5rem;
}

.description-content h2 {
  font-size: 1.5rem;
}

.description-content h3 {
  font-size: 1.3rem;
}

.description-content p {
  margin-bottom: 1rem;
}

.description-content ul,
.description-content ol {
  margin-bottom: 1rem;
  padding-left: 2rem;
}

.description-content li {
  margin-bottom: 0.5rem;
}

.description-content blockquote {
  border-left: 4px solid #667eea;
  padding-left: 1rem;
  margin: 1rem 0;
  background: rgba(102, 126, 234, 0.1);
  border-radius: 0 0.5rem 0.5rem 0;
  padding: 1rem;
}

.description-content code {
  background: #f8f9fa;
  padding: 0.2rem 0.4rem;
  border-radius: 0.25rem;
  font-family: 'Courier New', monospace;
  font-size: 0.9rem;
  color: #e83e8c;
}

.description-content pre {
  background: #f8f9fa;
  padding: 1rem;
  border-radius: 0.5rem;
  overflow-x: auto;
  margin: 1rem 0;
}

.description-content pre code {
  background: none;
  padding: 0;
  color: #333;
}

.description-content a {
  color: #667eea;
  text-decoration: none;
  border-bottom: 1px solid transparent;
  transition: border-color 0.3s ease;
}

.description-content a:hover {
  border-bottom-color: #667eea;
}

.description-content table {
  width: 100%;
  border-collapse: collapse;
  margin: 1rem 0;
}

.description-content th,
.description-content td {
  border: 1px solid #dee2e6;
  padding: 0.75rem;
  text-align: left;
}

.description-content th {
  background: #f8f9fa;
  font-weight: 600;
}

.search-section {
  margin-bottom: 2rem;
  display: flex;
  justify-content: center;
}

.search-box {
  display: flex;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 2rem;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  max-width: 400px;
  width: 100%;
}

.search-input {
  flex: 1;
  padding: 0.75rem 1.5rem;
  border: none;
  outline: none;
  font-size: 1rem;
  background: transparent;
}

.search-btn {
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: white;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
}

.search-btn:hover {
  background: linear-gradient(135deg, #5a6fd8, #6a4190);
}

.loading, .error, .empty-state {
  text-align: center;
  padding: 3rem;
  color: white;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.retry-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 1rem;
}

.retry-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.articles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.article-card {
  background: rgba(255, 255, 255, 0.95);
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s ease;
  text-decoration: none;
  color: inherit;
}

.article-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.15);
  background: white;
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
  transition: transform 0.3s ease;
}

.article-card:hover .article-image img {
  transform: scale(1.05);
}

.article-content {
  padding: 1.5rem;
}

.article-title {
  font-size: 1.25rem;
  font-weight: bold;
  margin-bottom: 0.5rem;
  color: #333;
  line-height: 1.4;
}

.article-summary {
  color: #666;
  margin-bottom: 1rem;
  line-height: 1.6;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.article-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  font-size: 0.9rem;
  color: #888;
}

.article-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.tag {
  background: linear-gradient(135deg, #667eea, #764ba2);
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.8rem;
  font-weight: 500;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.page-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.page-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.3);
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-info {
  color: white;
  font-weight: 500;
}

@media (max-width: 768px) {
  .page-title {
    font-size: 2rem;
  }

  .page-subtitle {
    font-size: 1rem;
  }

  .collection-description {
    margin: 0 1rem 2rem;
    padding: 1.5rem;
  }

  .description-content h1 {
    font-size: 1.5rem;
  }

  .description-content h2 {
    font-size: 1.3rem;
  }

  .description-content h3 {
    font-size: 1.1rem;
  }

  .articles-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .search-box {
    max-width: 100%;
  }

  .article-content {
    padding: 1rem;
  }

  .pagination {
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>
