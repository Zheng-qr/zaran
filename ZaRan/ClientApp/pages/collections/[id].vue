<template>
  <div class="collection-detail-page">
    <div class="container">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>加载集合详情中...</p>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="error-state">
        <i class="fas fa-exclamation-triangle"></i>
        <p>{{ error }}</p>
        <button @click="loadCollectionDetail" class="retry-btn">重试</button>
      </div>

      <!-- 集合详情 -->
      <div v-else-if="collectionData" class="collection-content">
        <!-- 集合头部信息 -->
        <div class="collection-header">
          <div class="collection-info">
            <h1 class="collection-title">{{ collectionData.collection.name }}</h1>
            <p class="collection-summary">{{ collectionData.collection.summary }}</p>
            <div class="collection-meta">
              <span class="creator">
                <i class="fas fa-user"></i>
                {{ collectionData.collection.creator?.nickname || collectionData.collection.creator?.username || '匿名用户' }}
              </span>
              <span class="items-count">
                <i class="fas fa-images"></i>
                {{ collectionData.items.length }} 件作品
              </span>
              <span class="created-time">
                <i class="fas fa-clock"></i>
                {{ formatTime(collectionData.collection.createdAt) }}
              </span>
            </div>
            <div v-if="collectionData.collection.tags && collectionData.collection.tags.length > 0" class="collection-tags">
              <span v-for="tag in collectionData.collection.tags" :key="tag" class="tag">
                {{ tag }}
              </span>
            </div>
          </div>
        </div>

        <!-- 集合描述 -->
        <div v-if="collectionData.collection.description" class="collection-description">
          <h2>详细介绍</h2>
          <div class="description-content markdown-content"
               v-html="formatDescription(collectionData.collection.description)"
               :style="markdownStyle"></div>
        </div>

        <!-- 集合内容 -->
        <div class="collection-items">
          <h2>作品展示</h2>
          <div v-if="collectionData.items.length === 0" class="empty-state">
            <i class="fas fa-images"></i>
            <p>该集合暂无作品</p>
          </div>
          <div v-else class="items-grid">
            <div 
              v-for="item in collectionData.items" 
              :key="item.id" 
              class="item-card"
              @click="viewItem(item)"
            >
              <div class="item-image">
                <img :src="item.imageUrl || '/image/default-artwork.jpg'" :alt="item.title" loading="lazy">
              </div>
              <div class="item-info">
                <h3 class="item-title">{{ item.title }}</h3>
                <p class="item-summary">{{ getItemSummary(item) }}</p>
                <div class="item-meta">
                  <span class="item-type">{{ getItemTypeName(item.type) }}</span>
                  <span class="item-time">{{ formatTime(item.createdAt) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { CollectionDetailResponse } from '~/api'
import { marked } from 'marked'

// 页面参数
const route = useRoute()
const collectionId = route.params.id as string

// 页面元数据
useHead({
  title: '集合详情 - 檐下风铃',
  meta: [
    { name: 'description', content: '查看集合详情和作品' }
  ]
})

// 使用社区 composable
const {
  error,
  getCollectionWithItems,
  formatTime,
  getArticleSummary
} = useCommunity()

// 加载状态
const loading = ref(false)

// 集合数据
const collectionData = ref<{
  collection: CollectionDetailResponse
  items: any[]
} | null>(null)

// 加载集合详情
const loadCollectionDetail = async () => {
  if (!collectionId) return
  
  const result = await getCollectionWithItems(collectionId)
  if (result) {
    collectionData.value = result
    
    // 更新页面标题
    useHead({
      title: `${result.collection.name} - 檐下风铃`,
      meta: [
        { name: 'description', content: result.collection.summary || '查看集合详情和作品' }
      ]
    })
  }
}

// 查看单个作品
const viewItem = (item: any) => {
  if (item.type !== undefined) {
    // 如果是文章类型
    navigateTo(`/articles/${item.id}`)
  } else {
    // 其他类型的处理
    console.log('查看作品:', item)
  }
}

// 获取作品摘要
const getItemSummary = (item: any): string => {
  return getArticleSummary(item, 80)
}

// 获取作品类型名称
const getItemTypeName = (type: any): string => {
  const typeNames: Record<number, string> = {
    0: '故事',
    1: '角色',
    2: '基因',
    3: '百科',
    4: '帖子'
  }
  return typeNames[type] || '作品'
}

// Markdown格式化函数
const formatDescription = (content: string): string => {
  if (!content) return ''
  return marked(content)
}

// Markdown样式计算属性
const markdownStyle = computed(() => {
  const collection = collectionData.value?.collection
  if (!collection?.color) return {}

  return {
    '--article-color': collection.color,
    '--article-color-light': `${collection.color}26` // 添加透明度
  }
})

// 页面加载时获取数据
onMounted(() => {
  loadCollectionDetail()
})
</script>

<style scoped>
.collection-detail-page {
  padding: 2rem 0;
  min-height: 100vh;
}

.collection-header {
  background: white;
  border-radius: var(--border-radius);
  padding: 2rem;
  margin-bottom: 2rem;
  box-shadow: var(--box-shadow);
}

.collection-title {
  font-size: 2rem;
  color: var(--dark-color);
  margin-bottom: 1rem;
}

.collection-summary {
  font-size: 1.1rem;
  color: var(--dark-gray-color);
  margin-bottom: 1.5rem;
  line-height: 1.6;
}

.collection-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 1.5rem;
  margin-bottom: 1rem;
  font-size: 0.9rem;
  color: var(--medium-gray-color);
}

.collection-meta span {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.collection-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.tag {
  background: var(--light-color);
  color: var(--dark-color);
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.8rem;
}

.collection-description {
  background: white;
  border-radius: var(--border-radius);
  padding: 2rem;
  margin-bottom: 2rem;
  box-shadow: var(--box-shadow);
}

.collection-description h2 {
  margin-bottom: 1rem;
  color: var(--dark-color);
}

.description-content {
  line-height: 1.7;
  color: var(--dark-gray-color);
}

/* Markdown 内容样式 */
.markdown-content {
  line-height: 1.8;
  color: var(--dark-gray-color);
}

.markdown-content h1 {
  font-size: 2rem;
  font-weight: 700;
  margin: 2rem 0 1rem 0;
  color: var(--article-color, var(--primary-color));
  border-bottom: 2px solid var(--article-color-light, var(--light-color));
  padding-bottom: 0.5rem;
}

.markdown-content h2 {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 1.5rem 0 1rem 0;
  color: var(--article-color, var(--primary-color));
}

.markdown-content h3 {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 1.25rem 0 0.75rem 0;
  color: var(--article-color, var(--primary-color));
}

.markdown-content p {
  margin-bottom: 1rem;
  line-height: 1.8;
}

.markdown-content ul, .markdown-content ol {
  margin: 1rem 0;
  padding-left: 2rem;
}

.markdown-content li {
  margin-bottom: 0.5rem;
  line-height: 1.6;
}

.markdown-content strong {
  font-weight: 600;
  color: var(--article-color, var(--primary-color));
}

.markdown-content em {
  font-style: italic;
  color: var(--dark-gray-color);
}

.markdown-content blockquote {
  border-left: 4px solid var(--article-color, var(--primary-color));
  background: var(--article-color-light, var(--light-color));
  padding: 1rem 1.5rem;
  margin: 1.5rem 0;
  font-style: italic;
  border-radius: 0 8px 8px 0;
}

.markdown-content code {
  background: var(--light-color);
  padding: 0.2rem 0.4rem;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
  font-size: 0.9rem;
  color: var(--article-color, var(--primary-color));
}

.markdown-content pre {
  background: var(--light-color);
  padding: 1rem;
  border-radius: 8px;
  overflow-x: auto;
  margin: 1rem 0;
}

.markdown-content pre code {
  background: none;
  padding: 0;
  color: var(--dark-color);
}

.collection-items {
  background: white;
  border-radius: var(--border-radius);
  padding: 2rem;
  box-shadow: var(--box-shadow);
}

.collection-items h2 {
  margin-bottom: 2rem;
  color: var(--dark-color);
}

.items-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 2rem;
}

.item-card {
  background: var(--light-color);
  border-radius: var(--border-radius);
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.item-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);
}

.item-image {
  width: 100%;
  height: 180px;
  overflow: hidden;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.item-card:hover .item-image img {
  transform: scale(1.05);
}

.item-info {
  padding: 1rem;
}

.item-title {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: var(--dark-color);
}

.item-summary {
  font-size: 0.9rem;
  color: var(--dark-gray-color);
  margin-bottom: 0.75rem;
  line-height: 1.5;
}

.item-meta {
  display: flex;
  justify-content: space-between;
  font-size: 0.8rem;
  color: var(--medium-gray-color);
}

/* 加载和错误状态样式 */
.loading-state,
.error-state {
  text-align: center;
  padding: 4rem 0;
}

.spinner {
  display: inline-block;
  width: 40px;
  height: 40px;
  border: 4px solid var(--light-color);
  border-radius: 50%;
  border-top-color: var(--primary-color);
  animation: spin 1s ease-in-out infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.error-state {
  color: var(--danger-color);
}

.retry-btn {
  background: var(--primary-color);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: var(--border-radius);
  cursor: pointer;
  margin-top: 1rem;
  font-weight: 500;
  transition: all 0.3s ease;
}

.retry-btn:hover {
  background: #065bb5;
  transform: translateY(-1px);
}

.empty-state {
  text-align: center;
  padding: 3rem 0;
  color: var(--medium-gray-color);
}

.empty-state i {
  font-size: 3rem;
  margin-bottom: 1rem;
  display: block;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .collection-detail-page {
    padding: 1rem 0;
  }

  .collection-header,
  .collection-description,
  .collection-items {
    padding: 1.5rem;
    margin-bottom: 1.5rem;
  }

  .collection-title {
    font-size: 1.5rem;
  }

  .collection-meta {
    gap: 1rem;
  }

  .items-grid {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 1.5rem;
  }
}

@media (max-width: 480px) {
  .collection-header,
  .collection-description,
  .collection-items {
    padding: 1rem;
  }

  .items-grid {
    grid-template-columns: 1fr;
  }

  .collection-meta {
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>
