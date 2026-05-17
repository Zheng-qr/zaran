<template>
  <div class="articles-page">
    <div class="container">
      <h1 class="page-title">文章资讯</h1>
      
      <!-- 搜索栏 -->
      <div class="search-section">
        <div class="search-bar">
          <input 
            v-model="searchKeyword" 
            type="text" 
            placeholder="搜索文章..."
            class="search-input"
            @keyup.enter="handleSearch"
          >
          <button @click="handleSearch" class="search-btn">
            <i class="fas fa-search"></i> 搜索
          </button>
        </div>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>加载中...</p>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="error-state">
        <i class="fas fa-exclamation-triangle"></i>
        <p>{{ error }}</p>
        <button @click="loadArticles" class="retry-btn">重试</button>
      </div>

      <!-- 文章列表 -->
      <div v-else-if="articles.length > 0" class="articles-grid">
        <div v-for="article in articles" :key="article.id" class="article-card"
          @click="goToArticleDetail(article.id)">
          <div class="article-image" v-if="article.imageSmallUrl">
            <img :src="article.imageSmallUrl" :alt="article.title" loading="lazy">
          </div>
          <div class="article-content">
            <h3 class="article-title">{{ article.title }}</h3>
            <p class="article-summary">{{ article.summary }}</p>
            <div class="article-meta">
              <span class="article-date">{{ formatDate(article.createdAt) }}</span>
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
        </div>
      </div>

      <!-- 空状态 -->
      <div v-else class="empty-state">
        <i class="fas fa-newspaper"></i>
        <p>暂无文章内容</p>
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
// 页面元数据
useHead({
  title: '文章资讯 - 檐下风铃',
  meta: [
    { name: 'description', content: '扎染文化相关文章资讯' }
  ]
})

const { articles, loading, error, totalPages, fetchArticles } = useArticles(); // 所有类型

const currentPage = ref(1);
const searchKeyword = ref('');

const loadArticles = () => {
  fetchArticles(currentPage.value, 12, searchKeyword.value);
};

const handleSearch = () => {
  currentPage.value = 1;
  loadArticles();
};

const changePage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
    loadArticles();
  }
};

const goToArticleDetail = (id: string) => {
  navigateTo(`/articles/${id}`);
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
};

onMounted(() => {
  loadArticles();
});
</script>

<style scoped>
/* 文章页面样式 */
.articles-page {
  padding: 2rem 0;
}

/* 文章卡片样式 */
.article-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(0,0,0,0.1);
  transition: transform 0.3s, box-shadow 0.3s;
  border-top: 5px solid var(--primary-color);
  cursor: pointer;
}

.article-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15);
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
  transition: transform 0.3s;
}

.article-card:hover .article-image img {
  transform: scale(1.05);
}

.article-content {
  padding: 20px;
}

.article-title {
  font-size: 1.2rem;
  margin-bottom: 10px;
  color: var(--dark-color);
  line-height: 1.4;
}

.article-summary {
  color: var(--medium-gray-color);
  margin-bottom: 15px;
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
  margin-bottom: 10px;
  font-size: 0.9rem;
  color: var(--medium-gray-color);
}

.article-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .articles-page {
    padding: 1rem 0;
  }

  .article-meta {
    flex-direction: column;
    align-items: flex-start;
    gap: 5px;
  }
}
</style>
