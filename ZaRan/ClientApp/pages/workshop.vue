<template>
  <div class="workshop-page">
    <!-- 导航栏已在 layout 中处理 -->
    
    <!-- 创意工坊内容 -->
    <main class="workshop-container">
      <div class="container">
        <h1 class="page-title">创意工坊</h1>
        <p class="page-description">使用纹样基因库中的元素创作您独特的扎染设计</p>

        <div class="workshop-layout">
          <!-- 画布区域 -->
          <div class="canvas-section">
            <!-- 作品信息输入 -->
            <div class="work-info-section">
              <div class="input-group">
                <input 
                  v-model="workTitle" 
                  type="text" 
                  placeholder="为您的作品命名"
                  class="work-title-input"
                >
                <textarea 
                  v-model="workDescription" 
                  placeholder="添加作品简介..."
                  class="work-description-input"
                  rows="3"
                ></textarea>
              </div>
              <div class="action-buttons">
                <button @click="saveWork" class="save-btn" :disabled="!workTitle">
                  <i class="fas fa-save"></i> 保存设计
                </button>
                <button @click="publishWork" class="publish-btn" :disabled="!workTitle || !workDescription">
                  <i class="fas fa-upload"></i> 发布作品
                </button>
              </div>
            </div>

            <!-- 绘图板组件 -->
            <div class="drawing-board-container">
              <div v-if="!canvasReady" class="canvas-loading">
                <i class="fas fa-spinner fa-spin"></i>
                <span>画布加载中...</span>
              </div>
              <WorkingDrawingBoard
                ref="drawingBoardRef"
                @canvas-ready="onCanvasReady"
                style="opacity: canvasReady ? 1 : 0"
              />
            </div>
          </div>

          <!-- 纹样库侧边栏 -->
          <div class="pattern-sidebar">
            <div class="pattern-library">
              <h2 class="sidebar-title">纹样基因库</h2>
              
              <!-- 纹样分类选择 -->
              <div class="pattern-categories">
                <button
                  v-for="category in patternCategories"
                  :key="category.id"
                  :class="['category-btn', { active: activeCategory === category.id }]"
                  @click="selectCategory(category.id)"
                >
                  {{ category.name }}
                </button>
              </div>

              <!-- 纹样网格 -->
              <div class="pattern-grid" v-if="!loading">
                <div 
                  v-for="pattern in filteredPatterns" 
                  :key="pattern.id"
                  class="pattern-item"
                  @click="selectPattern(pattern)"
                >
                  <div class="pattern-preview">
                    <img v-if="pattern.image" :src="pattern.image" :alt="pattern.name" />
                    <div v-else class="pattern-icon">
                      <i :class="pattern.icon || 'fas fa-image'"></i>
                    </div>
                  </div>
                  <div class="pattern-info">
                    <h4 class="pattern-name">{{ pattern.name }}</h4>
                    <p class="pattern-desc">{{ pattern.description }}</p>
                  </div>
                </div>
              </div>

              <!-- 加载状态 -->
              <div v-if="loading" class="loading-state">
                <i class="fas fa-spinner fa-spin"></i>
                <span>加载中...</span>
              </div>

              <!-- 空状态 -->
              <div v-if="!loading && filteredPatterns.length === 0" class="empty-state">
                <i class="fas fa-image"></i>
                <p>暂无纹样</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- 发布确认模态框 -->
    <div v-if="showPublishModal" class="modal-overlay" @click="closePublishModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>发布作品</h3>
          <button @click="closePublishModal" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <p>确定要发布作品《{{ workTitle }}》吗？</p>
          <p class="modal-note">发布后的作品将在社区中展示，其他用户可以查看和学习。</p>
        </div>
        <div class="modal-footer">
          <button @click="closePublishModal" class="cancel-btn">取消</button>
          <button @click="confirmPublish" class="confirm-btn" :disabled="publishing">
            <i v-if="publishing" class="fas fa-spinner fa-spin"></i>
            <i v-else class="fas fa-upload"></i>
            {{ publishing ? '发布中...' : '确认发布' }}
          </button>
        </div>
      </div>
    </div>

    <!-- 使用提示 -->
    <div v-if="showTips" class="tips-overlay" @click="closeTips">
      <div class="tips-content" @click.stop>
        <div class="tips-header">
          <h3>创意工坊使用指南</h3>
          <button @click="closeTips" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="tips-body">
          <div class="tip-item">
            <i class="fas fa-palette"></i>
            <div>
              <h4>选择纹样</h4>
              <p>从右侧纹样库中选择喜欢的图案，点击即可添加到画布</p>
            </div>
          </div>
          <div class="tip-item">
            <i class="fas fa-edit"></i>
            <div>
              <h4>编辑设计</h4>
              <p>使用画布工具栏进行绘制、编辑和调整</p>
            </div>
          </div>
          <div class="tip-item">
            <i class="fas fa-save"></i>
            <div>
              <h4>保存作品</h4>
              <p>填写作品标题和简介，然后保存或发布您的创作</p>
            </div>
          </div>
        </div>
        <div class="tips-footer">
          <label class="dont-show-again">
            <input type="checkbox" v-model="dontShowTipsAgain">
            不再显示此提示
          </label>
          <button @click="closeTips" class="tips-ok-btn">开始创作</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useNotifications } from '~/composables/useNotifications'
import { useArticleManagement } from '~/composables/useArticleManagement'
import { useArticles } from '~/composables/useArticles'
import { useCollections } from '~/composables/useCollections'
// 移除不存在的 useAuth 导入
import { ArticleType, ArticleStatus, type ArticlePostEndpointRequest } from '~/api'
import WorkingDrawingBoard from '~/components/WorkingDrawingBoard.vue'

// 页面元数据
useHead({
  title: '创意工坊 - 檐下风铃',
  meta: [
    { name: 'description', content: '扎染创意设计工坊' }
  ]
})

// 组件引用
const drawingBoardRef = ref<InstanceType<typeof WorkingDrawingBoard>>()
const router = useRouter()
const { success: showSuccess, error: showError, info: showInfo } = useNotifications()
const { createArticle } = useArticleManagement()
const userStore = useUserStore()

// 作品信息
const workTitle = ref('')
const workDescription = ref('')

// 发布状态
const showPublishModal = ref(false)
const publishing = ref(false)

// 画布状态
const canvasReady = ref(false)

// 提示状态
const showTips = ref(false)
const dontShowTipsAgain = ref(false)

// 纹样相关状态
const loading = ref(false)
const activeCategory = ref('all')
const patterns = ref<any[]>([])
const userCollections = ref<any[]>([])

// 纹样分类
const patternCategories = ref([
  { id: 'all', name: '全部' },
  { id: 'predefined', name: '预定义图案' },
  { id: 'collected', name: '我的收藏' },
  { id: 'geometric', name: '几何纹样' },
  { id: 'nature', name: '自然纹样' },
  { id: 'abstract', name: '抽象纹样' }
])

// 预定义图案数据
const predefinedPatterns = ref([
  { 
    id: 'spiral', 
    name: '螺旋纹', 
    description: '经典螺旋图案',
    icon: 'fas fa-spinner',
    category: 'geometric',
    svgPath: 'M20,50 Q50,20 80,50 Q50,80 20,50 Z'
  },
  { 
    id: 'grid', 
    name: '方格纹', 
    description: '规整方格图案',
    icon: 'fas fa-border-all',
    category: 'geometric',
    svgPath: 'M25,25 H75 V75 H25 Z M35,35 V65 H65 V35 Z'
  },
  { 
    id: 'leaf', 
    name: '叶子', 
    description: '自然叶片图案',
    icon: 'fas fa-leaf',
    category: 'nature',
    svgPath: 'M50,10 Q60,30 50,50 Q40,30 50,10 Z M50,50 L50,90'
  },
  { 
    id: 'paw', 
    name: '小猫爪印', 
    description: '可爱爪印图案',
    icon: 'fas fa-paw',
    category: 'nature',
    svgPath: 'M40,60 Q50,40 60,60 Q50,80 40,60 Z M30,40 Q40,20 50,40 Q40,60 30,40 Z M50,40 Q60,20 70,40 Q60,60 50,40 Z M60,60 Q70,40 80,60 Q70,80 60,60 Z'
  },
  { 
    id: 'cloud', 
    name: '云朵', 
    description: '飘逸云朵图案',
    icon: 'fas fa-cloud',
    category: 'nature',
    svgPath: 'M20,50 Q30,30 50,30 Q70,30 80,50 Q90,50 90,60 Q90,70 80,70 Q70,90 50,90 Q30,90 20,70 Q10,70 10,60 Q10,50 20,50 Z'
  },
  { 
    id: 'wave', 
    name: '水波', 
    description: '流动水波图案',
    icon: 'fas fa-water',
    category: 'abstract',
    svgPath: 'M10,50 Q25,30 40,50 Q55,30 70,50 Q85,30 90,50'
  },
  { 
    id: 'circle', 
    name: '扎染晕染', 
    description: '传统晕染效果',
    icon: 'fas fa-circle',
    category: 'abstract',
    svgPath: 'M50,10 A40,40 0 1,1 50,90 A40,40 0 1,1 50,10 Z M50,30 A20,20 0 1,0 50,70 A20,20 0 1,0 50,30 Z'
  },
  { 
    id: 'star', 
    name: '星空', 
    description: '璀璨星空图案',
    icon: 'fas fa-star',
    category: 'abstract',
    svgPath: 'M50,10 L55,35 L80,35 L60,50 L65,75 L50,60 L35,75 L40,50 L20,35 L45,35 Z'
  }
])

// 计算过滤后的纹样
const filteredPatterns = computed(() => {
  if (activeCategory.value === 'all') {
    return [...predefinedPatterns.value, ...userCollections.value]
  } else if (activeCategory.value === 'predefined') {
    return predefinedPatterns.value
  } else if (activeCategory.value === 'collected') {
    return userCollections.value
  } else {
    return predefinedPatterns.value.filter(p => p.category === activeCategory.value)
  }
})

// 选择分类
const selectCategory = (categoryId: string) => {
  activeCategory.value = categoryId
}

// 画布准备就绪
const onCanvasReady = () => {
  canvasReady.value = true
}

// 关闭提示
const closeTips = () => {
  showTips.value = false
  if (dontShowTipsAgain.value) {
    localStorage.setItem('workshop-tips-dismissed', 'true')
  }
}

// 检查是否显示提示
const checkShowTips = () => {
  const dismissed = localStorage.getItem('workshop-tips-dismissed')
  if (!dismissed) {
    // 延迟显示提示，让页面先加载完成
    setTimeout(() => {
      showTips.value = true
    }, 1000)
  }
}

// 选择纹样
const selectPattern = async (pattern: any) => {
  if (!drawingBoardRef.value || !drawingBoardRef.value.addPattern) {
    showError('画布未准备就绪')
    return
  }

  try {
    if (pattern.type === 'user-pattern' && pattern.canvasData) {
      // 用户收藏的纹样，需要特殊处理
      // 这里可以选择导入整个画布数据或者提取其中的元素
      showInfo(`用户纹样"${pattern.name}"需要特殊处理`)
      // TODO: 实现用户纹样的导入逻辑
    } else {
      // 预定义纹样
      await drawingBoardRef.value.addPattern(pattern)
      showSuccess(`已添加纹样"${pattern.name}"到画布`)
    }
  } catch (error) {
    console.error('添加纹样失败:', error)
    showError('添加纹样失败')
  }
}

// 保存作品
const saveWork = () => {
  if (!workTitle.value) {
    showError('请输入作品标题')
    return
  }

  // 获取画布数据
  const canvasData = drawingBoardRef.value?.exportData?.()
  if (!canvasData) {
    showError('画布数据获取失败')
    return
  }

  // 保存到本地存储
  const workData = {
    title: workTitle.value,
    description: workDescription.value,
    canvasData: canvasData,
    timestamp: new Date().toISOString()
  }

  localStorage.setItem(`workshop-${Date.now()}`, JSON.stringify(workData))
  showSuccess('作品已保存到本地')
}

// 发布作品
const publishWork = () => {
  if (!workTitle.value || !workDescription.value) {
    showError('请填写完整的作品信息')
    return
  }

  showPublishModal.value = true
}

// 关闭发布模态框
const closePublishModal = () => {
  showPublishModal.value = false
}

// 确认发布
const confirmPublish = async () => {
  publishing.value = true

  try {
    // 获取画布数据
    const canvasData = drawingBoardRef.value?.exportData?.()
    if (!canvasData) {
      throw new Error('画布数据获取失败')
    }

    // 创建 Pattern 类型的文章 (ArticleType._5 = 5 对应 Pattern)
    const articleData: ArticlePostEndpointRequest = {
      title: workTitle.value,
      body: JSON.stringify(canvasData), // 将画布数据作为文章内容
      summary: workDescription.value,
      tags: ['扎染', '创意工坊', '纹样设计'],
      type: ArticleType._5, // Pattern 类型 (5)
      imageUrl: null,
      color: null,
      imageSmallUrl: null
    }

    const result = await createArticle(articleData)

    if (result) {
      showSuccess('作品发布成功！')
      closePublishModal()

      // 可选：跳转到作品详情页
      // router.push(`/articles/${result.id}`)

      // 清空表单
      workTitle.value = ''
      workDescription.value = ''
    }
  } catch (error) {
    console.error('发布失败:', error)
    showError('发布失败，请重试')
  } finally {
    publishing.value = false
  }
}

// 加载用户收藏
const loadUserCollections = async () => {
  loading.value = true

  try {
    const userInfo = userStore.user
    if (!userInfo) {
      userCollections.value = []
      return
    }

    // 获取用户创建的 Pattern 类型文章（用户的纹样作品）
    const { fetchArticles } = useArticles('5') // ArticleType._5 = Pattern
    await fetchArticles(1, 20) // 获取前20个作品

    // 将用户的 Pattern 文章转换为纹样格式
    const { articles } = useArticles('5')
    const userPatterns = articles.value
      .filter(article => article.author?.id === userInfo.id)
      .map(article => ({
        id: `user-${article.id}`,
        name: article.title || '未命名作品',
        description: article.summary || '用户创作的纹样',
        category: 'collected',
        type: 'user-pattern',
        articleId: article.id,
        canvasData: article.body ? JSON.parse(article.body) : null,
        createdAt: article.createdAt
      }))

    userCollections.value = userPatterns
  } catch (error) {
    console.error('加载用户收藏失败:', error)
    userCollections.value = []
  } finally {
    loading.value = false
  }
}

// 组件挂载时加载数据
onMounted(() => {
  loadUserCollections()
  checkShowTips()
})
</script>

<style scoped>
/* 主容器样式 */
.workshop-page {
  min-height: 100vh;
  background-color: #f4f7f6;
}

.workshop-container {
  padding: 2rem 0;
}

.container {
  width: 90%;
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 15px;
}

.page-title {
  font-size: 2.5rem;
  font-weight: bold;
  color: #2c3e50;
  margin-bottom: 0.5rem;
  text-align: center;
}

.page-description {
  font-size: 1.1rem;
  color: #6c757d;
  text-align: center;
  margin-bottom: 2rem;
}

/* 工坊布局 */
.workshop-layout {
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 2rem;
  align-items: start;
}

/* 画布区域 */
.canvas-section {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  overflow: hidden;
}

.work-info-section {
  padding: 1.5rem;
  border-bottom: 1px solid #e9ecef;
  background: #f8f9fa;
}

.input-group {
  margin-bottom: 1rem;
}

.work-title-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
  margin-bottom: 0.75rem;
  transition: border-color 0.3s;
}

.work-title-input:focus {
  outline: none;
  border-color: #3A7BD5;
  box-shadow: 0 0 0 3px rgba(58, 123, 213, 0.1);
}

.work-description-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 0.95rem;
  resize: vertical;
  min-height: 80px;
  transition: border-color 0.3s;
  font-family: inherit;
}

.work-description-input:focus {
  outline: none;
  border-color: #3A7BD5;
  box-shadow: 0 0 0 3px rgba(58, 123, 213, 0.1);
}

.action-buttons {
  display: flex;
  gap: 1rem;
}

.save-btn, .publish-btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 0.95rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.save-btn {
  background: #6c757d;
  color: white;
}

.save-btn:hover:not(:disabled) {
  background: #5a6268;
}

.publish-btn {
  background: #3A7BD5;
  color: white;
}

.publish-btn:hover:not(:disabled) {
  background: #2a62b9;
}

.save-btn:disabled, .publish-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.drawing-board-container {
  padding: 1rem;
  position: relative;
  min-height: 600px;
}

.canvas-loading {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  color: #6c757d;
  font-size: 1.1rem;
}

.canvas-loading i {
  font-size: 2rem;
}

/* 纹样库侧边栏 */
.pattern-sidebar {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  height: fit-content;
  max-height: calc(100vh - 200px);
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.pattern-library {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.sidebar-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 1rem;
  text-align: center;
}

/* 纹样分类 */
.pattern-categories {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.category-btn {
  padding: 0.5rem 0.75rem;
  border: 1px solid #ddd;
  border-radius: 20px;
  background: white;
  color: #6c757d;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.3s;
  white-space: nowrap;
}

.category-btn:hover {
  border-color: #3A7BD5;
  color: #3A7BD5;
}

.category-btn.active {
  background: #3A7BD5;
  color: white;
  border-color: #3A7BD5;
}

/* 纹样网格 */
.pattern-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 1rem;
  flex: 1;
  overflow-y: auto;
  max-height: 500px;
  padding-right: 0.5rem;
}

.pattern-item {
  border: 1px solid #e9ecef;
  border-radius: 8px;
  padding: 1rem;
  cursor: pointer;
  transition: all 0.3s;
  text-align: center;
  background: white;
}

.pattern-item:hover {
  border-color: #3A7BD5;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(58, 123, 213, 0.15);
}

.pattern-item:active {
  transform: translateY(0);
  box-shadow: 0 2px 6px rgba(58, 123, 213, 0.2);
}

.pattern-preview {
  width: 60px;
  height: 60px;
  margin: 0 auto 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 6px;
  background: #f8f9fa;
}

.pattern-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 4px;
}

.pattern-icon {
  font-size: 1.5rem;
  color: #3A7BD5;
}

.pattern-info {
  text-align: center;
}

.pattern-name {
  font-size: 0.9rem;
  font-weight: 500;
  color: #2c3e50;
  margin-bottom: 0.25rem;
}

.pattern-desc {
  font-size: 0.8rem;
  color: #6c757d;
  line-height: 1.3;
}

/* 加载和空状态 */
.loading-state, .empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: #6c757d;
  text-align: center;
}

.loading-state i {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.empty-state i {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  opacity: 0.5;
}

/* 模态框样式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow: hidden;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.25rem;
  color: #6c757d;
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 4px;
  transition: all 0.3s;
}

.close-btn:hover {
  background: #f8f9fa;
  color: #2c3e50;
}

.modal-body {
  padding: 1.5rem;
}

.modal-body p {
  margin-bottom: 1rem;
  color: #2c3e50;
}

.modal-note {
  font-size: 0.9rem;
  color: #6c757d;
  background: #f8f9fa;
  padding: 1rem;
  border-radius: 6px;
  border-left: 4px solid #3A7BD5;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  padding: 1.5rem;
  border-top: 1px solid #e9ecef;
  background: #f8f9fa;
}

.cancel-btn, .confirm-btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-size: 0.95rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.cancel-btn {
  background: #6c757d;
  color: white;
}

.cancel-btn:hover {
  background: #5a6268;
}

.confirm-btn {
  background: #3A7BD5;
  color: white;
}

.confirm-btn:hover:not(:disabled) {
  background: #2a62b9;
}

.confirm-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* 提示框样式 */
.tips-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1001;
}

.tips-content {
  background: white;
  border-radius: 12px;
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.3);
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow: hidden;
}

.tips-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 2rem;
  border-bottom: 1px solid #e9ecef;
  background: linear-gradient(135deg, #3A7BD5, #00D2FF);
  color: white;
}

.tips-header h3 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
}

.tips-header .close-btn {
  color: white;
  background: rgba(255, 255, 255, 0.2);
}

.tips-header .close-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.tips-body {
  padding: 2rem;
}

.tip-item {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.tip-item:last-child {
  margin-bottom: 0;
}

.tip-item i {
  font-size: 1.5rem;
  color: #3A7BD5;
  margin-top: 0.25rem;
  flex-shrink: 0;
}

.tip-item h4 {
  margin: 0 0 0.5rem 0;
  font-size: 1.1rem;
  font-weight: 600;
  color: #2c3e50;
}

.tip-item p {
  margin: 0;
  color: #6c757d;
  line-height: 1.5;
}

.tips-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 2rem;
  border-top: 1px solid #e9ecef;
  background: #f8f9fa;
}

.dont-show-again {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  color: #6c757d;
  cursor: pointer;
}

.dont-show-again input[type="checkbox"] {
  margin: 0;
}

.tips-ok-btn {
  padding: 0.75rem 2rem;
  border: none;
  border-radius: 6px;
  background: #3A7BD5;
  color: white;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s;
}

.tips-ok-btn:hover {
  background: #2a62b9;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .workshop-layout {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .pattern-sidebar {
    order: -1;
    max-height: 300px;
  }

  .pattern-grid {
    grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
    gap: 0.75rem;
    max-height: 200px;
  }

  .pattern-categories {
    justify-content: center;
  }

  .action-buttons {
    flex-direction: column;
  }

  .page-title {
    font-size: 2rem;
  }

  .container {
    width: 95%;
    padding: 0 10px;
  }
}

@media (max-width: 480px) {
  .workshop-container {
    padding: 1rem 0;
  }

  .pattern-grid {
    grid-template-columns: repeat(auto-fill, minmax(80px, 1fr));
  }

  .pattern-item {
    padding: 0.75rem;
  }

  .pattern-preview {
    width: 50px;
    height: 50px;
  }

  .modal-content {
    width: 95%;
    margin: 1rem;
  }

  .modal-header, .modal-body, .modal-footer {
    padding: 1rem;
  }
}
</style>
