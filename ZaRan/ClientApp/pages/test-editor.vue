<template>
  <div class="container mx-auto px-4 py-8">
    <div class="max-w-4xl mx-auto">
      <h1 class="text-3xl font-bold text-gray-900 mb-8">编辑器测试页面</h1>
      
      <div class="bg-white shadow rounded-lg p-6">
        <h2 class="text-xl font-semibold text-gray-900 mb-4">增强编辑器测试</h2>
        
        <div class="mb-6">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            文章内容
          </label>
          <EnhancedEditor
            v-model="content"
            placeholder="请输入测试内容..."
            editor-class="min-h-64"
          />
        </div>
        
        <div class="border-t pt-6">
          <h3 class="text-lg font-medium text-gray-900 mb-3">当前内容（原始数据）：</h3>
          <pre class="bg-gray-100 p-4 rounded text-sm overflow-auto max-h-40">{{ content }}</pre>
        </div>
        
        <div class="border-t pt-6 mt-6">
          <h3 class="text-lg font-medium text-gray-900 mb-3">Markdown 渲染效果：</h3>
          <div class="prose prose-sm max-w-none bg-gray-50 p-4 rounded" v-html="renderedContent"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { marked } from 'marked'

// 页面元数据
useHead({
  title: '编辑器测试 - 檐下风铃',
  meta: [
    { name: 'description', content: '测试增强编辑器功能' }
  ]
})

// 响应式数据
const content = ref(`# 欢迎使用增强编辑器

这是一个测试内容，展示编辑器的功能：

## 功能特点

1. **富文本编辑** - 支持完整的富文本编辑功能
2. **Markdown 预览** - 可以实时预览 Markdown 渲染效果
3. **双模式切换** - 在编辑和预览模式之间自由切换

### 代码示例

\`\`\`javascript
function hello() {
  console.log("Hello, World!");
}
\`\`\`

### 引用

> 这是一个引用示例，展示引用的样式效果。

### 列表

- 项目一
- 项目二
- 项目三

### 链接和图片

[访问官网](https://example.com)

![示例图片](https://via.placeholder.com/300x200)

---

**粗体文本** 和 *斜体文本* 的示例。`)

// 计算属性
const renderedContent = computed(() => {
  if (!content.value) return ''
  try {
    return marked(content.value)
  } catch (error) {
    return content.value
  }
})

// 配置 marked
marked.setOptions({
  breaks: true,
  gfm: true
})
</script>

<style scoped>
.container {
  min-height: 100vh;
  background-color: #f9fafb;
}
</style>
