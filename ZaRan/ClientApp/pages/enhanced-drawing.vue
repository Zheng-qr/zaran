<template>
  <div>
    <div class="header">
      <h1 class="text-3xl font-bold text-center py-4 text-white">增强版画板工具</h1>
      <p class="text-center text-white/90 pb-4">
        新增图片插入和图案笔刷功能，支持完整的数据导出导入
      </p>
    </div>
    
    <div class="feature-highlights">
      <div class="feature-grid">
        <div class="feature-card">
          <h3>🖼️ 图片插入</h3>
          <p>支持图片上传、缩放、旋转，导出时保留完整的 base64 数据</p>
        </div>
        <div class="feature-card">
          <h3>🎨 图案笔刷</h3>
          <p>可以加载任意图片作为画笔图案，创造独特的绘画效果</p>
        </div>
        <div class="feature-card">
          <h3>💾 完整导出</h3>
          <p>导出的数据包含所有图片的 base64 数据，确保完整恢复</p>
        </div>
        <div class="feature-card">
          <h3>🔄 智能导入</h3>
          <p>导入时自动恢复图片和图案，保持原有的视觉效果</p>
        </div>
      </div>
    </div>
    
    <WorkingDrawingBoard />
    
    <div class="usage-tips">
      <h2 class="text-xl font-bold mb-4">使用说明</h2>
      <div class="tips-grid">
        <div class="tip-item">
          <h4>📷 图片插入</h4>
          <p>点击"📷 图片"按钮，选择图片文件，图片会自动添加到画布中央</p>
        </div>
        <div class="tip-item">
          <h4>🎨 图案笔刷</h4>
          <p>选择画笔工具，将类型改为"图案画笔"，然后点击"选择图案"加载图片</p>
        </div>
        <div class="tip-item">
          <h4>💾 数据保存</h4>
          <p>导出的 JSON 文件包含完整的图片数据，可以在任何地方完整恢复</p>
        </div>
        <div class="tip-item">
          <h4>🔧 故障排除</h4>
          <p>如果图片无法显示，请检查图片格式是否支持（推荐 PNG、JPG）</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import WorkingDrawingBoard from '~/components/WorkingDrawingBoard.vue'

useHead({
  title: '增强版画板工具 - ZaRan',
  meta: [
    { name: 'description', content: '支持图片插入和图案笔刷的增强版画板工具' }
  ]
})
</script>

<style scoped>
.header {
  background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%);
}

.feature-highlights {
  max-width: 1200px;
  margin: 20px auto;
  padding: 20px;
}

.feature-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 20px;
}

.feature-card {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  text-align: center;
}

.feature-card h3 {
  margin: 0 0 10px 0;
  color: #7c3aed;
  font-size: 1.2rem;
}

.feature-card p {
  margin: 0;
  color: #666;
  line-height: 1.5;
}

.usage-tips {
  max-width: 1200px;
  margin: 40px auto;
  padding: 20px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.tips-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 20px;
}

.tip-item {
  padding: 15px;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  background: #f9fafb;
}

.tip-item h4 {
  margin: 0 0 8px 0;
  color: #7c3aed;
  font-size: 1rem;
}

.tip-item p {
  margin: 0;
  color: #666;
  font-size: 0.9rem;
  line-height: 1.4;
}

@media (max-width: 768px) {
  .feature-grid,
  .tips-grid {
    grid-template-columns: 1fr;
  }
  
  .feature-highlights,
  .usage-tips {
    margin: 10px;
    padding: 15px;
  }
}
</style>
