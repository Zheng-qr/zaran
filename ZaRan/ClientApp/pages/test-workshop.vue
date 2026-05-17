<template>
  <div class="test-workshop">
    <h1>Workshop 测试页面</h1>
    
    <div class="test-section">
      <h2>通知测试</h2>
      <button @click="testNotifications" class="test-btn">测试通知</button>
    </div>

    <div class="test-section">
      <h2>用户认证测试</h2>
      <p>当前用户: {{ userStore.user ? userStore.user.username : '未登录' }}</p>
    </div>

    <div class="test-section">
      <h2>图案测试</h2>
      <div class="pattern-test">
        <div 
          v-for="pattern in testPatterns" 
          :key="pattern.id"
          class="pattern-item"
          @click="testPattern(pattern)"
        >
          <i :class="pattern.icon"></i>
          <span>{{ pattern.name }}</span>
        </div>
      </div>
    </div>

    <div class="test-section">
      <h2>WorkingDrawingBoard 测试</h2>
      <div class="drawing-test">
        <WorkingDrawingBoard ref="drawingBoardRef" />
        <div class="drawing-controls">
          <button @click="testAddPattern" class="test-btn">添加测试图案</button>
          <button @click="testExportData" class="test-btn">导出数据</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useNotifications } from '~/composables/useNotifications'
// 使用 useUserStore 替代 useAuth
import WorkingDrawingBoard from '~/components/WorkingDrawingBoard.vue'

// 页面元数据
useHead({
  title: 'Workshop 测试页面',
  meta: [
    { name: 'description', content: '测试 Workshop 功能' }
  ]
})

const { success: showSuccess, error: showError, info: showInfo } = useNotifications()
const userStore = useUserStore()
const drawingBoardRef = ref<InstanceType<typeof WorkingDrawingBoard>>()

// 测试图案
const testPatterns = ref([
  { 
    id: 'spiral', 
    name: '螺旋纹', 
    icon: 'fas fa-spinner',
    svgPath: 'M20,50 Q50,20 80,50 Q50,80 20,50 Z'
  },
  { 
    id: 'circle', 
    name: '圆形', 
    icon: 'fas fa-circle',
    svgPath: 'M50,10 A40,40 0 1,1 50,90 A40,40 0 1,1 50,10 Z'
  }
])

// 测试通知
const testNotifications = () => {
  showSuccess('成功通知测试')
  setTimeout(() => showError('错误通知测试'), 1000)
  setTimeout(() => showInfo('信息通知测试'), 2000)
}

// 测试图案
const testPattern = (pattern: any) => {
  showInfo(`选择了图案: ${pattern.name}`)
  testAddPattern(pattern)
}

// 测试添加图案
const testAddPattern = (pattern?: any) => {
  const testPattern = pattern || testPatterns.value[0]
  
  if (drawingBoardRef.value && drawingBoardRef.value.addPattern) {
    drawingBoardRef.value.addPattern(testPattern)
      .then(() => {
        showSuccess(`成功添加图案: ${testPattern.name}`)
      })
      .catch((error: any) => {
        console.error('添加图案失败:', error)
        showError(`添加图案失败: ${error.message}`)
      })
  } else {
    showError('画布未准备就绪')
  }
}

// 测试导出数据
const testExportData = () => {
  if (drawingBoardRef.value && drawingBoardRef.value.exportData) {
    const data = drawingBoardRef.value.exportData()
    if (data) {
      console.log('导出的数据:', data)
      showSuccess('数据导出成功，请查看控制台')
    } else {
      showError('导出数据失败')
    }
  } else {
    showError('画布未准备就绪')
  }
}
</script>

<style scoped>
.test-workshop {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.test-section {
  margin-bottom: 2rem;
  padding: 1.5rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  background: white;
}

.test-section h2 {
  margin-top: 0;
  color: #2c3e50;
}

.test-btn {
  padding: 0.75rem 1.5rem;
  background: #3A7BD5;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  margin-right: 1rem;
  margin-bottom: 0.5rem;
}

.test-btn:hover {
  background: #2a62b9;
}

.pattern-test {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.pattern-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
}

.pattern-item:hover {
  border-color: #3A7BD5;
  background: #f8f9fa;
}

.pattern-item i {
  font-size: 1.5rem;
  color: #3A7BD5;
}

.drawing-test {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.drawing-controls {
  display: flex;
  gap: 1rem;
}
</style>
