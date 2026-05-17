<template>
  <div>
    <div class="header">
      <h1 class="text-3xl font-bold text-center py-4 text-white">图案笔刷调试</h1>
      <p class="text-center text-white/90 pb-4">
        调试图案笔刷功能，查看控制台输出
      </p>
    </div>
    
    <div class="debug-panel">
      <h2 class="text-xl font-bold mb-4">调试信息</h2>
      <div class="debug-info">
        <p><strong>当前工具:</strong> {{ currentTool }}</p>
        <p><strong>画笔类型:</strong> {{ brushType }}</p>
        <p><strong>图案已加载:</strong> {{ patternLoaded ? '是' : '否' }}</p>
        <p><strong>画布绘制模式:</strong> {{ isDrawingMode ? '是' : '否' }}</p>
        <p><strong>有图案事件:</strong> {{ hasPatternEvents ? '是' : '否' }}</p>
      </div>
      
      <div class="debug-actions">
        <button @click="debugInfo" class="debug-btn">打印调试信息</button>
        <button @click="forcePatternMode" class="debug-btn">强制图案模式</button>
        <button @click="testPatternEvents" class="debug-btn">测试图案事件</button>
      </div>
    </div>
    
    <WorkingDrawingBoard ref="drawingBoard" />
    
    <div class="instructions">
      <h3 class="font-bold mb-2">调试步骤：</h3>
      <ol class="list-decimal list-inside space-y-1">
        <li>打开浏览器开发者工具的控制台</li>
        <li>选择画笔工具，切换到"图案画笔"</li>
        <li>上传图案图片</li>
        <li>点击"打印调试信息"查看状态</li>
        <li>尝试在画布上绘制，观察控制台输出</li>
        <li>如果不工作，点击"强制图案模式"</li>
      </ol>
    </div>
  </div>
</template>

<script setup lang="ts">
import WorkingDrawingBoard from '~/components/WorkingDrawingBoard.vue'

const drawingBoard = ref()

// 调试状态
const currentTool = ref('unknown')
const brushType = ref('unknown')
const patternLoaded = ref(false)
const isDrawingMode = ref(false)
const hasPatternEvents = ref(false)

// 更新调试信息
const updateDebugInfo = () => {
  // 这里需要从 WorkingDrawingBoard 组件获取状态
  // 由于组件没有暴露这些状态，我们先用静态值
  console.log('Debug info updated')
}

// 打印调试信息
const debugInfo = () => {
  console.log('=== 图案笔刷调试信息 ===')
  console.log('当前工具:', currentTool.value)
  console.log('画笔类型:', brushType.value)
  console.log('图案已加载:', patternLoaded.value)
  console.log('画布绘制模式:', isDrawingMode.value)
  console.log('有图案事件:', hasPatternEvents.value)
  
  // 尝试访问画板组件的内部状态
  if (drawingBoard.value) {
    console.log('画板组件引用:', drawingBoard.value)
  }
}

// 强制图案模式
const forcePatternMode = () => {
  console.log('尝试强制启用图案模式...')
  // 这里可以添加强制启用图案模式的逻辑
}

// 测试图案事件
const testPatternEvents = () => {
  console.log('测试图案事件绑定...')
  // 这里可以添加测试事件绑定的逻辑
}

onMounted(() => {
  updateDebugInfo()
  
  // 定期更新调试信息
  setInterval(updateDebugInfo, 1000)
})

useHead({
  title: '图案笔刷调试 - ZaRan'
})
</script>

<style scoped>
.header {
  background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%);
}

.debug-panel {
  max-width: 800px;
  margin: 20px auto;
  padding: 20px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.debug-info {
  background: #f3f4f6;
  padding: 15px;
  border-radius: 6px;
  margin-bottom: 15px;
}

.debug-info p {
  margin: 5px 0;
  font-family: monospace;
}

.debug-actions {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.debug-btn {
  padding: 8px 16px;
  background: #dc2626;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.debug-btn:hover {
  background: #b91c1c;
}

.instructions {
  max-width: 800px;
  margin: 20px auto;
  padding: 20px;
  background: #fef3c7;
  border-radius: 8px;
  border-left: 4px solid #f59e0b;
}

.instructions ol {
  font-size: 14px;
}

@media (max-width: 768px) {
  .debug-panel,
  .instructions {
    margin: 10px;
    padding: 15px;
  }
  
  .debug-actions {
    flex-direction: column;
  }
}
</style>
