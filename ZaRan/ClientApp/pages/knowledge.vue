<template>
  <div class="knowledge-page">
    <div class="container">
      <h1 class="page-title">扎染工艺科普百科</h1>

      <!-- 搜索栏 -->
      <div class="search-section mb-6">
        <div class="search-bar">
          <input
            v-model="searchKeyword"
            type="text"
            placeholder="搜索科普内容..."
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
        <button @click="fetchArticles" class="retry-btn">重试</button>
      </div>

      <!-- 文章列表 -->
      <div v-else-if="articles.length > 0" class="knowledge-grid">
        <div v-for="article in articles" :key="article.id" class="knowledge-card"
          @click="goToArticleDetail(article.id!)">
          <div class="knowledge-image" v-if="article.imageSmallUrl">
            <img :src="article.imageSmallUrl" :alt="article.title" loading="lazy">
          </div>
          <div class="knowledge-content">
            <h3 class="knowledge-title">{{ article.title }}</h3>
            <p class="knowledge-summary">{{ article.summary }}</p>
            <div class="knowledge-meta">
              <span class="knowledge-date">{{ formatDate(article.createdAt!) }}</span>
              <span class="knowledge-author" v-if="article.author">
                {{ article.author.nickname }}
              </span>
            </div>
            <div class="knowledge-tags" v-if="article.tags && article.tags.length > 0">
              <span v-for="tag in article.tags.slice(0, 3)" :key="tag" class="tag">
                {{ tag }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-else class="empty-state">
        <i class="fas fa-microscope"></i>
        <p>暂无科普内容</p>
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

      <!-- 贡献者部分 -->
      <div class="contribute-section">
        <h2>想要分享扎染知识？</h2>
        <p>我们欢迎专业人士和爱好者贡献百科内容</p>
        <NuxtLink to="/admin/articles" class="contribute-btn">
          <i class="fas fa-plus"></i> 贡献百科内容
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// 页面元数据
useHead({
  title: '扎染工艺科普百科 - 檐下风铃',
  meta: [
    { name: 'description', content: '了解传统扎染工艺技术' }
  ]
})

const { articles, loading, error, totalPages, fetchArticles } = useArticles('wiki'); // Wiki类型

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

const goToArticleDetail = (id: number) => {
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
.knowledge-page {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1rem;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: #2c3e50;
  text-align: center;
  margin-bottom: 3rem;
}

/* 搜索栏 */
.search-section {
  display: flex;
  justify-content: center;
  margin-bottom: 2rem;
}

.search-bar {
  display: flex;
  max-width: 500px;
  width: 100%;
}

.search-input {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 1px solid #ddd;
  border-radius: 0.375rem 0 0 0.375rem;
  font-size: 1rem;
  outline: none;
}

.search-input:focus {
  border-color: #00A896;
  box-shadow: 0 0 0 0.2rem rgba(0, 168, 150, 0.25);
}

.search-btn {
  padding: 0.75rem 1rem;
  background-color: #00A896;
  color: white;
  border: 1px solid #00A896;
  border-radius: 0 0.375rem 0.375rem 0;
  cursor: pointer;
  transition: background-color 0.2s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.search-btn:hover {
  background-color: #008a7a;
}

/* 知识网格 */
.knowledge-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.knowledge-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s, box-shadow 0.3s;
  cursor: pointer;
  border-top: 4px solid #00A896;
}

.knowledge-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.knowledge-image {
  width: 100%;
  height: 200px;
  overflow: hidden;
}

.knowledge-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.knowledge-content {
  padding: 1.5rem;
}

.knowledge-title {
  font-size: 1.3rem;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 0.5rem;
  line-height: 1.4;
}

.knowledge-summary {
  color: #6c757d;
  font-size: 0.9rem;
  line-height: 1.6;
  margin-bottom: 1rem;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.knowledge-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  font-size: 0.8rem;
  color: #6c757d;
}

.knowledge-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.tag {
  background-color: #e7f5f3;
  color: #00A896;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

/* 分页 */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 3rem;
}

.page-btn {
  padding: 0.5rem 1rem;
  background-color: #00A896;
  color: white;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.page-btn:hover:not(:disabled) {
  background-color: #008a7a;
}

.page-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.page-info {
  font-weight: 500;
  color: #495057;
}

/* 加载状态 */
.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  color: #6c757d;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e9ecef;
  border-top: 4px solid #00A896;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 错误状态 */
.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  color: #dc3545;
  text-align: center;
}

.error-state i {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.retry-btn {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background-color: #dc3545;
  color: white;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.retry-btn:hover {
  background-color: #c82333;
}

/* 空状态 */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  color: #6c757d;
  text-align: center;
}

.empty-state i {
  font-size: 4rem;
  margin-bottom: 1rem;
  opacity: 0.5;
  color: #00A896;
}

.contribute-section {
  background: white;
  padding: 3rem 2rem;
  margin-top: 3rem;
  text-align: center;
  border-radius: var(--border-radius);
  box-shadow: var(--box-shadow);
}

.contribute-section h2 {
  color: var(--dark-color);
  margin-bottom: 1rem;
  font-size: 1.8rem;
}

.contribute-section p {
  color: var(--dark-gray-color);
  margin-bottom: 2rem;
  font-size: 1.1rem;
}

.contribute-btn {
  background: var(--primary-color);
  color: white;
  padding: 12px 25px;
  border: none;
  border-radius: 30px;
  font-size: 1.1rem;
  cursor: pointer;
  transition: all 0.3s;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  text-decoration: none;
  box-shadow: 0 4px 15px rgba(58, 123, 213, 0.3);
}

.contribute-btn:hover {
  background-color: #2a68c4;
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(58, 123, 213, 0.4);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .container {
    padding: 1rem 0.5rem;
  }

  .page-title {
    font-size: 2rem;
  }

  .knowledge-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .search-bar {
    max-width: 100%;
  }

  .search-btn {
    padding: 0.75rem;
  }
}
</style>
