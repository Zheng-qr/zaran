<template>
  <div>
    <div class="header">
      <h1 class="text-3xl font-bold text-center py-4 text-white">图案笔刷测试</h1>
      <p class="text-center text-white/90 pb-4">
        测试修复后的图案笔刷功能
      </p>
    </div>
    
    <div class="test-instructions">
      <h2 class="text-xl font-bold mb-4">测试步骤</h2>
      <ol class="list-decimal list-inside space-y-2">
        <li>选择画笔工具（🖌️）</li>
        <li>将画笔类型改为"图案画笔"</li>
        <li>点击"选择图案"按钮，上传一张小图片（建议 100x100 像素以下）</li>
        <li>看到"✅ 图案已加载"提示后，在画布上绘制</li>
        <li>调整画笔粗细，观察图案大小变化</li>
        <li>切换回"普通画笔"测试正常绘制</li>
      </ol>
      
      <div class="tips">
        <h3 class="font-bold mb-2">提示：</h3>
        <ul class="list-disc list-inside space-y-1 text-sm">
          <li>图案会根据画笔粗细自动调整大小</li>
          <li>建议使用简单的图案或纹理图片</li>
          <li>如果图案不显示，请检查图片格式是否支持</li>
          <li>可以重新选择图案来更换</li>
        </ul>
      </div>
    </div>
    
    <WorkingDrawingBoard />
    
    <div class="debug-info">
      <h3 class="font-bold mb-2">调试信息：</h3>
      <p class="text-sm text-gray-600">
        打开浏览器开发者工具的控制台查看详细的调试信息
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import WorkingDrawingBoard from '~/components/WorkingDrawingBoard.vue'

useHead({
  title: '图案笔刷测试 - ZaRan',
  meta: [
    { name: 'description', content: '测试图案笔刷功能的专用页面' }
  ]
})
</script>

<style scoped>
.header {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
}

.test-instructions {
  max-width: 800px;
  margin: 20px auto;
  padding: 20px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.test-instructions ol {
  background: #fef3c7;
  padding: 15px;
  border-radius: 6px;
  border-left: 4px solid #f59e0b;
}

.tips {
  margin-top: 20px;
  padding: 15px;
  background: #ecfdf5;
  border-radius: 6px;
  border-left: 4px solid #10b981;
}

.debug-info {
  max-width: 800px;
  margin: 20px auto;
  padding: 15px;
  background: #f3f4f6;
  border-radius: 6px;
  text-align: center;
}

@media (max-width: 768px) {
  .test-instructions,
  .debug-info {
    margin: 10px;
    padding: 15px;
  }
}
</style>
