<template>
  <div class="drawing-page">
    <div class="page-header">
      <h1>画板工具</h1>
      <p>功能完整的在线画板，支持画笔、形状、文字、图片等多种工具</p>
    </div>
    
    <DrawingBoard ref="drawingBoardRef" />
    
    <!-- 功能说明 -->
    <div class="feature-info">
      <h2>功能说明</h2>
      <div class="features-grid">
        <div class="feature-item">
          <h3>🔍 选择工具</h3>
          <p>选择、移动、旋转和调整对象大小。支持多选，右键可复制或删除。</p>
        </div>
        <div class="feature-item">
          <h3>🖌️ 画笔工具</h3>
          <p>自由绘画，可调节粗细、颜色和晕染效果。</p>
        </div>
        <div class="feature-item">
          <h3>🧽 橡皮擦</h3>
          <p>真正的擦除功能，可调节橡皮擦大小。</p>
        </div>
        <div class="feature-item">
          <h3>🔷 形状工具</h3>
          <p>插入长方形、圆形、三角形、五角星等形状。</p>
        </div>
        <div class="feature-item">
          <h3>📝 文字工具</h3>
          <p>插入可编辑文字，支持颜色和大小调节。</p>
        </div>
        <div class="feature-item">
          <h3>📷 图片工具</h3>
          <p>上传并插入图片，可进行旋转和缩放。</p>
        </div>
        <div class="feature-item">
          <h3>🔍 缩放功能</h3>
          <p>放大缩小画布，方便精细操作。</p>
        </div>
        <div class="feature-item">
          <h3>💾 导出导入</h3>
          <p>支持导出为图片格式，或保存/加载画布数据。</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import DrawingBoard from '~/components/DrawingBoard.vue'

// 页面元数据
useHead({
  title: '画板工具 - ZaRan',
  meta: [
    { name: 'description', content: '功能完整的在线画板工具，支持画笔、形状、文字、图片等多种绘图功能' }
  ]
})

const drawingBoardRef = ref()

// 可以通过 ref 调用画板的方法
const exportImage = () => {
  drawingBoardRef.value?.exportAsImage('png')
}

const exportData = () => {
  drawingBoardRef.value?.exportData()
}

const clearBoard = () => {
  if (confirm('确定要清空画布吗？')) {
    drawingBoardRef.value?.clearCanvas()
  }
}
</script>

<style scoped>
.drawing-page {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.page-header {
  text-align: center;
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.page-header h1 {
  margin: 0 0 10px 0;
  font-size: 2.5rem;
  font-weight: bold;
}

.page-header p {
  margin: 0;
  font-size: 1.1rem;
  opacity: 0.9;
}

.feature-info {
  padding: 40px 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.feature-info h2 {
  text-align: center;
  margin-bottom: 30px;
  color: #333;
  font-size: 2rem;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 20px;
}

.feature-item {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  transition: transform 0.2s, box-shadow 0.2s;
}

.feature-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

.feature-item h3 {
  margin: 0 0 10px 0;
  color: #333;
  font-size: 1.2rem;
}

.feature-item p {
  margin: 0;
  color: #666;
  line-height: 1.5;
}

@media (max-width: 768px) {
  .page-header h1 {
    font-size: 2rem;
  }
  
  .page-header p {
    font-size: 1rem;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
  }
  
  .feature-info {
    padding: 20px 10px;
  }
}
</style>
