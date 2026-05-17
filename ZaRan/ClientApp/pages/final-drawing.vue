<template>
  <div>
    <div class="header">
      <h1 class="text-3xl font-bold text-center py-4 text-white">完整功能画板工具</h1>
      <p class="text-center text-white/90 pb-4">
        支持画笔、橡皮擦、形状、文字、图片等多种工具的专业画板
      </p>
    </div>
    <FinalDrawingBoard />
  </div>
</template>

<script setup lang="ts">
import FinalDrawingBoard from '~/components/FinalDrawingBoard.vue'

useHead({
  title: '完整功能画板工具 - ZaRan',
  meta: [
    { name: 'description', content: '功能完整的在线画板工具，支持画笔、橡皮擦、形状、文字、图片等多种绘图功能' }
  ]
})
</script>

<style scoped>
.header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}
</style>
