<template>
  <div class="pattern-library-page">
    <div class="container">
      <h1 class="page-title">扎染纹样基因库</h1>
      <p class="page-description">探索传统扎染纹样的奥秘，传承千年工艺之美</p>
      
      <!-- 搜索和筛选 -->
      <div class="search-filter-section">
        <div class="search-bar">
          <input 
            v-model="searchKeyword" 
            type="text" 
            placeholder="搜索纹样..."
            class="search-input"
            @keyup.enter="handleSearch"
          >
          <button @click="handleSearch" class="search-btn">
            <i class="fas fa-search"></i>
          </button>
        </div>
        
        <div class="filter-tabs">
          <button
            v-for="category in categories"
            :key="category.id"
            :class="['filter-tab', { active: isCategoryActive(category.name) }]"
            @click="handleCategorySearch(category.name)"
          >
            {{ category.name }}
          </button>
        </div>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="loading-state">
        <div class="loading-spinner">
          <i class="fas fa-spinner fa-spin"></i>
        </div>
        <p>正在加载基因库数据...</p>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="error-state">
        <div class="error-icon">
          <i class="fas fa-exclamation-triangle"></i>
        </div>
        <p>{{ error }}</p>
        <button @click="fetchPatterns()" class="retry-btn">
          <i class="fas fa-redo"></i>
          重试
        </button>
      </div>

      <!-- 空状态 -->
      <div v-else-if="filteredPatterns.length === 0" class="empty-state">
        <div class="empty-icon">
          <i class="fas fa-search"></i>
        </div>
        <p>没有找到符合条件的纹样</p>
        <p class="empty-hint">尝试调整搜索关键词或筛选条件</p>
      </div>

      <!-- 纹样网格 -->
      <div v-else class="patterns-grid">
        <div class="pattern-card" v-for="pattern in filteredPatterns" :key="pattern.id">
          <div class="pattern-image">
            <img :src="pattern.image" :alt="pattern.name" loading="lazy">
            <div class="pattern-overlay">
              <button class="view-detail-btn" @click="viewPatternDetail(pattern)">
                <i class="fas fa-eye"></i>
                查看详情
              </button>
              <button class="download-btn" @click="downloadPattern(pattern)" :disabled="downloading">
                <i v-if="downloading" class="fas fa-spinner fa-spin"></i>
                <i v-else class="fas fa-download"></i>
                {{ downloading ? '下载中...' : '下载' }}
              </button>
            </div>
          </div>
          <div class="pattern-info">
            <h3 class="pattern-name">{{ pattern.name }}</h3>
            <p class="pattern-description">{{ pattern.description }}</p>
            <div class="pattern-meta">
              <span class="pattern-category">{{ pattern.category }}</span>
              <span class="pattern-origin">{{ pattern.origin }}</span>
            </div>
            <div class="pattern-stats">
              <span class="downloads">
                <i class="fas fa-download"></i>
                {{ pattern.downloads }}
              </span>
              <span class="likes">
                <i class="fas fa-heart"></i>
                {{ pattern.likes }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- 分页 -->
      <div v-if="totalPages > 1 && !loading && !error" class="pagination">
        <button
          @click="fetchPatterns(currentPage - 1, searchKeyword)"
          :disabled="currentPage <= 1"
          class="page-btn"
        >
          <i class="fas fa-chevron-left"></i>
          上一页
        </button>

        <span class="page-info">
          第 {{ currentPage }} 页，共 {{ totalPages }} 页
        </span>

        <button
          @click="fetchPatterns(currentPage + 1, searchKeyword)"
          :disabled="currentPage >= totalPages"
          class="page-btn"
        >
          下一页
          <i class="fas fa-chevron-right"></i>
        </button>
      </div>

      <!-- 纹样详情模态框 -->
      <div v-if="selectedPattern" class="pattern-modal" @click="closeModal">
        <div class="modal-content" @click.stop>
          <button class="close-modal" @click="closeModal">
            <i class="fas fa-times"></i>
          </button>
          <div class="modal-image">
            <img :src="selectedPattern.image" :alt="selectedPattern.name" loading="lazy">
          </div>
          <div class="modal-info">
            <h2 class="modal-title">{{ selectedPattern.name }}</h2>
            <p class="modal-description">{{ selectedPattern.description }}</p>
            <div class="modal-details">
              <div class="detail-item">
                <span class="label">分类:</span>
                <span class="value">{{ selectedPattern.category }}</span>
              </div>
              <div class="detail-item">
                <span class="label">起源:</span>
                <span class="value">{{ selectedPattern.origin }}</span>
              </div>
              <div class="detail-item">
                <span class="label">工艺:</span>
                <span class="value">{{ selectedPattern.technique }}</span>
              </div>
              <div class="detail-item">
                <span class="label">难度:</span>
                <span class="value">{{ selectedPattern.difficulty }}</span>
              </div>
            </div>
            <div class="modal-actions">
              <button class="download-btn-large" @click="downloadPattern(selectedPattern)" :disabled="downloading">
                <i v-if="downloading" class="fas fa-spinner fa-spin"></i>
                <i v-else class="fas fa-download"></i>
                {{ downloading ? '下载中...' : '下载纹样' }}
              </button>
              <button class="use-in-workshop-btn" @click="useInWorkshop(selectedPattern)">
                <i class="fas fa-paint-brush"></i>
                在工坊中使用
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// 页面元数据
useHead({
  title: '扎染纹样基因库 - 檐下风铃',
  meta: [
    { name: 'description', content: '传统扎染纹样资源库' }
  ]
})

// 使用基因库 composable
const {
  loading,
  error,
  currentPage,
  categories,
  activeCategory,
  searchKeyword,
  selectedPattern,
  downloading,
  totalPages,
  filteredPatterns,
  fetchPatterns,
  searchPatterns,
  viewPatternDetail,
  closeModal,
  downloadPattern,
  useInWorkshop
} = useGeneLibrary()

// 搜索处理
const handleSearch = async () => {
  await searchPatterns(searchKeyword.value)
}

// 分类搜索处理
const handleCategorySearch = async (categoryName: string) => {
  // 如果是"全部"，清空搜索关键词
  if (categoryName === '全部') {
    searchKeyword.value = ''
    await searchPatterns('')
  } else {
    // 否则搜索对应的关键词
    const searchKeywords: Record<string, string> = {
      '传统纹样': '传统',
      '现代创新': '现代',
      '几何图案': '几何',
      '花卉纹样': '花卉',
      '抽象艺术': '抽象'
    }
    const keyword = searchKeywords[categoryName] || categoryName
    searchKeyword.value = keyword
    await searchPatterns(keyword)
  }
}

// 判断分类是否激活
const isCategoryActive = (categoryName: string) => {
  if (categoryName === '全部') {
    return !searchKeyword.value || searchKeyword.value === ''
  }

  const searchKeywords: Record<string, string> = {
    '传统纹样': '传统',
    '现代创新': '现代',
    '几何图案': '几何',
    '花卉纹样': '花卉',
    '抽象艺术': '抽象'
  }

  const keyword = searchKeywords[categoryName] || categoryName
  return searchKeyword.value === keyword
}
</script>

<style scoped>
.pattern-library-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  padding: 2rem 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 700;
  text-align: center;
  margin-bottom: 0.5rem;
  background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.page-description {
  text-align: center;
  color: var(--dark-gray-color);
  font-size: 1.1rem;
  margin-bottom: 3rem;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

/* 搜索和筛选区域 */
.search-filter-section {
  background: white;
  border-radius: 16px;
  padding: 2rem;
  margin-bottom: 3rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
}

.search-bar {
  display: flex;
  max-width: 500px;
  margin: 0 auto 2rem;
  background: #f8f9fa;
  border-radius: 50px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.search-bar:focus-within {
  transform: translateY(-2px);
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.15);
}

.search-input {
  flex: 1;
  padding: 1rem 1.5rem;
  border: none;
  outline: none;
  font-size: 1rem;
  background: transparent;
  color: var(--text-color);
}

.search-input::placeholder {
  color: var(--medium-gray-color);
}

.search-btn {
  padding: 1rem 1.5rem;
  background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
  color: white;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.search-btn:hover {
  transform: scale(1.05);
}

.search-btn i {
  font-size: 1.1rem;
}

/* 筛选标签 */
.filter-tabs {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.filter-tab {
  padding: 0.75rem 1.5rem;
  background: #f8f9fa;
  color: var(--dark-gray-color);
  border: 2px solid transparent;
  border-radius: 50px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-weight: 500;
  font-size: 0.9rem;
}

.filter-tab:hover {
  background: #e9ecef;
  transform: translateY(-2px);
}

.filter-tab.active {
  background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
  color: white;
  border-color: var(--primary-color);
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(7, 108, 209, 0.3);
}

/* 状态样式 */
.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  color: var(--medium-gray-color);
}

.loading-spinner {
  font-size: 3rem;
  margin-bottom: 1rem;
  color: var(--primary-color);
}

.loading-spinner i {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.error-icon,
.empty-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  color: var(--accent-color-2);
}

.empty-icon {
  color: var(--medium-gray-color);
}

.retry-btn {
  margin-top: 1rem;
  padding: 0.75rem 1.5rem;
  background: var(--primary-color);
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.retry-btn:hover {
  background: #065bb5;
  transform: translateY(-2px);
}

.empty-hint {
  font-size: 0.9rem;
  margin-top: 0.5rem;
  opacity: 0.7;
}

/* 分页样式 */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 2rem;
  margin-top: 3rem;
  padding: 2rem 0;
}

.page-btn {
  padding: 0.75rem 1.5rem;
  background: var(--primary-color);
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 500;
}

.page-btn:hover:not(:disabled) {
  background: #065bb5;
  transform: translateY(-2px);
}

.page-btn:disabled {
  background: var(--medium-gray-color);
  cursor: not-allowed;
  transform: none;
}

.page-info {
  font-weight: 500;
  color: var(--dark-gray-color);
  padding: 0 1rem;
}

/* 纹样网格 */
.patterns-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.pattern-card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  transition: all 0.4s ease;
  position: relative;
}

.pattern-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
}

.pattern-image {
  position: relative;
  height: 250px;
  overflow: hidden;
}

.pattern-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.pattern-card:hover .pattern-image img {
  transform: scale(1.1);
}

.pattern-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(7, 108, 209, 0.9), rgba(0, 168, 150, 0.9));
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  opacity: 0;
  transition: all 0.3s ease;
}

.pattern-card:hover .pattern-overlay {
  opacity: 1;
}

.view-detail-btn,
.download-btn {
  padding: 0.75rem 1.5rem;
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-weight: 500;
  backdrop-filter: blur(10px);
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.view-detail-btn:hover,
.download-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.3);
  border-color: rgba(255, 255, 255, 0.5);
  transform: translateY(-2px);
}

.download-btn:disabled {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  cursor: not-allowed;
  transform: none;
  opacity: 0.6;
}

.pattern-info {
  padding: 1.5rem;
}

.pattern-name {
  font-size: 1.3rem;
  font-weight: 600;
  color: var(--dark-color);
  margin-bottom: 0.5rem;
}

.pattern-description {
  color: var(--dark-gray-color);
  font-size: 0.95rem;
  line-height: 1.6;
  margin-bottom: 1rem;
}

.pattern-meta {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

.pattern-category,
.pattern-origin {
  padding: 0.25rem 0.75rem;
  background: #f8f9fa;
  color: var(--dark-gray-color);
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
}

.pattern-category {
  background: linear-gradient(135deg, var(--accent-color-1), #ffd54f);
  color: white;
}

.pattern-stats {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.downloads,
.likes {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--medium-gray-color);
  font-size: 0.9rem;
}

.downloads i,
.likes i {
  color: var(--primary-color);
}

/* 模态框样式 */
.pattern-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
  backdrop-filter: blur(5px);
}

.modal-content {
  background: white;
  border-radius: 24px;
  max-width: 900px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  position: relative;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.close-modal {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: rgba(0, 0, 0, 0.5);
  color: white;
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10;
  transition: all 0.3s ease;
}

.close-modal:hover {
  background: rgba(0, 0, 0, 0.7);
  transform: scale(1.1);
}

.modal-image {
  border-radius: 24px 0 0 24px;
  overflow: hidden;
}

.modal-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.modal-info {
  padding: 2rem;
  display: flex;
  flex-direction: column;
}

.modal-title {
  font-size: 2rem;
  font-weight: 700;
  color: var(--dark-color);
  margin-bottom: 1rem;
}

.modal-description {
  color: var(--dark-gray-color);
  line-height: 1.6;
  margin-bottom: 2rem;
  font-size: 1.1rem;
}

.modal-details {
  margin-bottom: 2rem;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.detail-item:last-child {
  border-bottom: none;
}

.detail-item .label {
  font-weight: 600;
  color: var(--dark-color);
}

.detail-item .value {
  color: var(--dark-gray-color);
  font-weight: 500;
}

.modal-actions {
  display: flex;
  gap: 1rem;
  margin-top: auto;
}

.download-btn-large,
.use-in-workshop-btn {
  flex: 1;
  padding: 1rem 1.5rem;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.download-btn-large {
  background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
  color: white;
}

.download-btn-large:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(7, 108, 209, 0.3);
}

.download-btn-large:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
  opacity: 0.6;
}

.use-in-workshop-btn {
  background: linear-gradient(135deg, var(--accent-color-2), var(--accent-color-1));
  color: white;
}

.use-in-workshop-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(232, 90, 79, 0.3);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .pattern-library-page {
    padding: 1rem 0;
  }

  .container {
    padding: 0 0.5rem;
  }

  .page-title {
    font-size: 2rem;
  }

  .search-filter-section {
    padding: 1.5rem;
    margin-bottom: 2rem;
  }

  .search-bar {
    margin-bottom: 1.5rem;
  }

  .filter-tabs {
    gap: 0.25rem;
  }

  .filter-tab {
    padding: 0.5rem 1rem;
    font-size: 0.8rem;
  }

  .patterns-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }

  .pattern-card {
    border-radius: 16px;
  }

  .pattern-image {
    height: 200px;
  }

  .modal-content {
    grid-template-columns: 1fr;
    max-width: 95vw;
    margin: 1rem;
  }

  .modal-image {
    border-radius: 24px 24px 0 0;
    height: 250px;
  }

  .modal-info {
    padding: 1.5rem;
  }

  .modal-title {
    font-size: 1.5rem;
  }

  .modal-actions {
    flex-direction: column;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.75rem;
  }

  .search-filter-section {
    padding: 1rem;
  }

  .filter-tabs {
    justify-content: flex-start;
    overflow-x: auto;
    padding-bottom: 0.5rem;
  }

  .filter-tab {
    white-space: nowrap;
  }

  .pattern-info {
    padding: 1rem;
  }

  .pattern-meta {
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>
