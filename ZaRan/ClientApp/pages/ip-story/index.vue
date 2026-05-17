<template>
    <div class="ip-story-page">
        <div class="container">
            <h1 class="page-title">探索多元文化IP故事</h1>
            <p class="page-subtitle">发现世界各地丰富多彩的文化遗产与创意表达</p>

            <!-- 加载状态 -->
            <div v-if="collectionsLoading" class="loading">
                <div class="spinner"></div>
                <p>正在加载故事集合...</p>
            </div>

            <!-- 错误状态 -->
            <div v-else-if="collectionsError" class="error">
                <i class="fas fa-exclamation-triangle"></i>
                <p>{{ collectionsError }}</p>
                <button @click="loadCollections" class="retry-btn">重试</button>
            </div>

            <!-- 集合网格 -->
            <div v-else-if="collections.length > 0" class="collections-grid">
                <NuxtLink v-for="collection in collections" :key="collection.id"
                    :to="`/ip-story/${collection.id}`" class="collection-card">
                    <div class="collection-header">
                        <div class="collection-icon">
                            <i class="fas fa-palette"></i>
                        </div>
                        <h3 class="collection-title">{{ collection.name }}</h3>
                        <p class="collection-desc">{{ collection.summary }}</p>
                        <div class="collection-stats">
                            <span>{{ collection.childrenIds?.length || 0 }}个故事</span>
                        </div>
                    </div>
                </NuxtLink>
            </div>

            <!-- 空状态 -->
            <div v-else class="empty-state">
                <i class="fas fa-folder-open"></i>
                <p>暂无故事集合</p>
            </div>

            <!-- 贡献者部分 -->
            <div class="contribute-section">
                <h2>拥有文化IP想要分享？</h2>
                <p>我们欢迎经过认证的文化传承者贡献内容</p>
                <NuxtLink to="/admin/articles" class="contribute-btn">
                    <i class="fas fa-plus"></i> 申请成为贡献者
                </NuxtLink>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { CollectionDetailResponse } from '~/api'
import { CollectionType } from '~/api'

// 页面元数据
useHead({
  title: 'IP故事 - 檐下风铃',
  meta: [
    { name: 'description', content: '探索扎染的IP故事世界' }
  ]
})

// 集合相关状态
const {
  collections,
  loading: collectionsLoading,
  error: collectionsError,
  fetchCollections
} = useCollections(CollectionType._0, undefined) // Article type, no specific creator

// 加载集合列表
const loadCollections = async () => {
  await fetchCollections(1, 20) // 获取更多集合
}

// 页面加载时获取集合
onMounted(() => {
  loadCollections()
})
</script>

<style scoped>
.ip-story-page {
  min-height: 100vh;
  padding: 2rem 0;
  position: relative;
  overflow: hidden;
  background: #1a1a2e;
}

.ip-story-page::before {
  content: '';
  position: absolute;
  top: -100%;
  left: -100%;
  right: -100%;
  bottom: -100%;
  width: 300%;
  height: 300%;
  background: 
    radial-gradient(ellipse 1200px 900px at 20% 80%, #667eea 0%, rgba(102, 126, 234, 0.4) 30%, rgba(102, 126, 234, 0.2) 60%, transparent 90%),
    radial-gradient(ellipse 1400px 1000px at 80% 20%, #764ba2 0%, rgba(118, 75, 162, 0.5) 25%, rgba(118, 75, 162, 0.2) 55%, transparent 85%),
    radial-gradient(ellipse 1100px 800px at 40% 40%, #f093fb 0%, rgba(240, 147, 251, 0.4) 35%, rgba(240, 147, 251, 0.15) 65%, transparent 95%),
    radial-gradient(ellipse 1600px 1200px at 90% 90%, #f5576c 0%, rgba(245, 87, 108, 0.3) 20%, rgba(245, 87, 108, 0.1) 50%, transparent 80%),
    radial-gradient(ellipse 1000px 700px at 10% 10%, #4facfe 0%, rgba(79, 172, 254, 0.5) 40%, rgba(79, 172, 254, 0.2) 70%, transparent 100%),
    radial-gradient(ellipse 1300px 950px at 70% 70%, #43e97b 0%, rgba(67, 233, 123, 0.4) 30%, rgba(67, 233, 123, 0.15) 60%, transparent 90%),
    radial-gradient(ellipse 1150px 850px at 30% 60%, #ff6b6b 0%, rgba(255, 107, 107, 0.4) 35%, rgba(255, 107, 107, 0.2) 65%, transparent 95%),
    radial-gradient(ellipse 1500px 1100px at 60% 30%, #feca57 0%, rgba(254, 202, 87, 0.3) 25%, rgba(254, 202, 87, 0.1) 55%, transparent 85%);
  animation: fluidMove 20s ease-in-out infinite;
  opacity: 0.9;
}

.ip-story-page::after {
  content: '';
  position: absolute;
  top: -100%;
  left: -100%;
  right: -100%;
  bottom: -100%;
  width: 300%;
  height: 300%;
  background: 
    radial-gradient(ellipse 1200px 900px at 60% 10%, #ff9a9e 0%, rgba(255, 154, 158, 0.5) 30%, rgba(255, 154, 158, 0.2) 60%, transparent 90%),
    radial-gradient(ellipse 1100px 800px at 30% 90%, #a8edea 0%, rgba(168, 237, 234, 0.4) 35%, rgba(168, 237, 234, 0.15) 65%, transparent 95%),
    radial-gradient(ellipse 1400px 1000px at 90% 60%, #fecfef 0%, rgba(254, 207, 239, 0.4) 25%, rgba(254, 207, 239, 0.1) 55%, transparent 85%),
    radial-gradient(ellipse 1000px 750px at 10% 50%, #ffecd2 0%, rgba(255, 236, 210, 0.5) 40%, rgba(255, 236, 210, 0.2) 70%, transparent 100%),
    radial-gradient(ellipse 1700px 1300px at 75% 25%, #48cae4 0%, rgba(72, 202, 228, 0.3) 20%, rgba(72, 202, 228, 0.1) 50%, transparent 80%),
    radial-gradient(ellipse 1050px 800px at 25% 75%, #f72585 0%, rgba(247, 37, 133, 0.4) 30%, rgba(247, 37, 133, 0.15) 60%, transparent 90%),
    radial-gradient(ellipse 1300px 950px at 85% 15%, #7209b7 0%, rgba(114, 9, 183, 0.4) 35%, rgba(114, 9, 183, 0.2) 65%, transparent 95%),
    radial-gradient(ellipse 1150px 850px at 15% 85%, #06ffa5 0%, rgba(6, 255, 165, 0.3) 25%, rgba(6, 255, 165, 0.1) 55%, transparent 85%);
  animation: fluidMove2 25s ease-in-out infinite reverse;
  opacity: 0.8;
}

@keyframes fluidMove {
  0% {
    transform: translate(0%, 0%) rotate(0deg) scale(1);
  }
  20% {
    transform: translate(12%, -8%) rotate(72deg) scale(1.2);
  }
  40% {
    transform: translate(-5%, 10%) rotate(144deg) scale(0.85);
  }
  60% {
    transform: translate(-12%, -3%) rotate(216deg) scale(1.3);
  }
  80% {
    transform: translate(8%, -12%) rotate(288deg) scale(0.9);
  }
  100% {
    transform: translate(0%, 0%) rotate(360deg) scale(1);
  }
}

@keyframes fluidMove2 {
  0% {
    transform: translate(0%, 0%) rotate(0deg) scale(1);
  }
  25% {
    transform: translate(-10%, 6%) rotate(90deg) scale(1.4);
  }
  50% {
    transform: translate(6%, -10%) rotate(180deg) scale(0.75);
  }
  75% {
    transform: translate(10%, 5%) rotate(270deg) scale(1.15);
  }
  100% {
    transform: translate(0%, 0%) rotate(360deg) scale(1);
  }
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
  position: relative;
  z-index: 1;
}

.page-title {
  font-size: 2.5rem;
  font-weight: bold;
  text-align: center;
  margin-bottom: 1rem;
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.page-subtitle {
  font-size: 1.2rem;
  text-align: center;
  margin-bottom: 3rem;
  color: rgba(255, 255, 255, 0.9);
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
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

.collections-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
  margin-top: 2rem;
}

.collection-card {
  background: rgba(255, 255, 255, 0.95);
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s ease;
  cursor: pointer;
  text-decoration: none;
  color: inherit;
}

.collection-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.15);
  background: white;
}

.collection-header {
  text-align: center;
}

.collection-icon {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
  color: white;
  font-size: 1.5rem;
}

.collection-title {
  font-size: 1.5rem;
  font-weight: bold;
  margin-bottom: 0.5rem;
  color: #333;
}

.collection-desc {
  color: #666;
  margin-bottom: 1rem;
  line-height: 1.6;
}

.collection-stats {
  display: flex;
  justify-content: center;
  gap: 1rem;
  font-size: 0.9rem;
  color: #888;
}

.empty-state {
  font-size: 1.2rem;
}

.empty-state i {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.7;
}

.contribute-section {
  background: rgba(255, 255, 255, 0.95);
  padding: 3rem 2rem;
  margin-top: 3rem;
  text-align: center;
  border-radius: 1rem;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.contribute-section h2 {
  color: #333;
  margin-bottom: 1rem;
  font-size: 1.8rem;
}

.contribute-section p {
  color: #666;
  margin-bottom: 2rem;
  font-size: 1.1rem;
}

.contribute-btn {
  background: linear-gradient(135deg, #667eea, #764ba2);
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
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.contribute-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

@media (max-width: 768px) {
  .page-title {
    font-size: 2rem;
  }

  .collections-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .collection-card {
    padding: 1.5rem;
  }
}
</style>
