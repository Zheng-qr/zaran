<template>
  <div>
    <div class="header">
      <h1 class="text-3xl font-bold text-center py-4 text-white">Fabric.js 画板工具</h1>
      <p class="text-center text-white/90 pb-4">功能完整的在线画板，支持画笔、橡皮擦、形状、文字、图片等多种工具</p>
    </div>
    <FabricDrawingBoard />
  </div>
</template>

<script setup lang="ts">
import FabricDrawingBoard from '~/components/FabricDrawingBoard.vue'

useHead({
  title: 'Fabric.js 画板工具 - ZaRan'
})
</script>

<style scoped>
.header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}
</style>
