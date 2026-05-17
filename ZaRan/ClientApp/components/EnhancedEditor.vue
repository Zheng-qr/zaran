<template>
  <div class="enhanced-editor">
    <!-- Editor Toolbar -->
    <div class="editor-toolbar">
      <div class="toolbar-left">
        <button
          type="button"
          :class="['toolbar-btn', { active: viewMode === 'edit' }]"
          @click="viewMode = 'edit'"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
          </svg>
          富文本编辑
        </button>
        <button
          type="button"
          :class="['toolbar-btn', { active: viewMode === 'preview' }]"
          @click="viewMode = 'preview'"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
          </svg>
          Markdown 预览
        </button>
      </div>
      
      <div class="toolbar-right">
        <span class="text-sm text-gray-500">
          {{ viewMode === 'edit' ? '富文本编辑模式' : 'Markdown 预览模式' }}
        </span>
      </div>
    </div>

    <!-- Editor Content -->
    <div class="editor-content">
      <!-- Rich Text Editor -->
      <div v-show="viewMode === 'edit'" class="editor-pane">
        <ClientOnly>
          <QuillEditor
            v-model:content="content"
            content-type="html"
            :options="editorOptions"
            :class="editorClass"
          />
          <template #fallback>
            <div class="min-h-96 bg-gray-50 border border-gray-300 rounded p-4 flex items-center justify-center">
              <div class="text-gray-500">编辑器加载中...</div>
            </div>
          </template>
        </ClientOnly>
      </div>

      <!-- Markdown Preview -->
      <div v-show="viewMode === 'preview'" class="preview-pane">
        <div class="preview-header">
          <h3 class="text-lg font-medium text-gray-900">预览效果</h3>
          <p class="text-sm text-gray-500">以下是文章在前端的显示效果</p>
        </div>
        <div class="markdown-content" v-html="markdownPreview" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { QuillEditor } from '@vueup/vue-quill'
import '@vueup/vue-quill/dist/vue-quill.snow.css'
import { marked } from 'marked'

interface Props {
  modelValue: string
  placeholder?: string
  editorClass?: string
}

interface Emits {
  (e: 'update:modelValue', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '请输入内容...',
  editorClass: 'min-h-96'
})

const emit = defineEmits<Emits>()

// 响应式数据
const viewMode = ref<'edit' | 'preview'>('edit')

// 计算属性
const content = computed({
  get: () => props.modelValue,
  set: (value: string) => emit('update:modelValue', value)
})

const markdownPreview = computed(() => {
  if (!content.value) return '<p class="text-gray-500 italic">暂无内容</p>'
  
  // 如果内容是 HTML，尝试转换为 Markdown 预览
  // 这里我们直接使用 marked 来渲染，因为用户可能会输入一些 Markdown 语法
  try {
    return marked(content.value)
  } catch (error) {
    // 如果解析失败，直接显示 HTML 内容
    return content.value
  }
})

// Enhanced editor options
const editorOptions = {
  theme: 'snow',
  placeholder: props.placeholder,
  modules: {
    toolbar: [
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      ['bold', 'italic', 'underline', 'strike'],
      [{ 'color': [] }, { 'background': [] }],
      [{ 'script': 'sub'}, { 'script': 'super' }],
      ['blockquote', 'code-block'],
      [{ 'list': 'ordered'}, { 'list': 'bullet' }],
      [{ 'indent': '-1'}, { 'indent': '+1' }],
      [{ 'align': [] }],
      ['link', 'image', 'video'],
      [{ 'size': ['small', false, 'large', 'huge'] }],
      [{ 'font': [] }],
      ['clean']
    ],
    history: {
      delay: 1000,
      maxStack: 50,
      userOnly: true
    }
  }
}

// 配置 marked
marked.setOptions({
  breaks: true,
  gfm: true
})
</script>

<style scoped>
.enhanced-editor {
  @apply border border-gray-300 rounded-lg overflow-hidden bg-white;
}

.editor-toolbar {
  @apply flex items-center justify-between px-4 py-3 bg-gray-50 border-b border-gray-200;
}

.toolbar-left {
  @apply flex items-center space-x-2;
}

.toolbar-btn {
  @apply flex items-center space-x-2 px-3 py-2 text-sm font-medium text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-md transition-colors;
}

.toolbar-btn.active {
  @apply text-blue-600 bg-blue-50 border border-blue-200;
}

.toolbar-right {
  @apply flex items-center;
}

.editor-content {
  @apply relative;
}

.editor-pane {
  @apply relative;
}

.preview-pane {
  @apply p-6 min-h-96 bg-gray-50;
}

.preview-header {
  @apply mb-4 pb-3 border-b border-gray-200;
}

.markdown-content {
  @apply prose prose-sm max-w-none bg-white p-6 rounded-lg shadow-sm;
}

/* Markdown 内容样式 */
.markdown-content :deep(h1),
.markdown-content :deep(h2),
.markdown-content :deep(h3),
.markdown-content :deep(h4),
.markdown-content :deep(h5),
.markdown-content :deep(h6) {
  @apply text-gray-900 font-semibold mt-6 mb-3;
}

.markdown-content :deep(h1) { @apply text-2xl; }
.markdown-content :deep(h2) { @apply text-xl; }
.markdown-content :deep(h3) { @apply text-lg; }

.markdown-content :deep(p) {
  @apply mb-4 leading-relaxed text-gray-700;
}

.markdown-content :deep(blockquote) {
  @apply border-l-4 border-blue-500 pl-4 py-2 bg-blue-50 italic text-gray-600;
}

.markdown-content :deep(code) {
  @apply bg-gray-100 px-2 py-1 rounded text-sm font-mono text-gray-800;
}

.markdown-content :deep(pre) {
  @apply bg-gray-900 text-gray-100 p-4 rounded-lg overflow-x-auto my-4;
}

.markdown-content :deep(pre code) {
  @apply bg-transparent p-0 text-gray-100;
}

.markdown-content :deep(ul),
.markdown-content :deep(ol) {
  @apply mb-4 pl-6;
}

.markdown-content :deep(li) {
  @apply mb-2 text-gray-700;
}

.markdown-content :deep(a) {
  @apply text-blue-600 hover:text-blue-800 underline;
}

.markdown-content :deep(img) {
  @apply max-w-full h-auto rounded-lg shadow-sm my-4;
}

.markdown-content :deep(table) {
  @apply w-full border-collapse border border-gray-300 my-4;
}

.markdown-content :deep(th),
.markdown-content :deep(td) {
  @apply border border-gray-300 px-3 py-2 text-left;
}

.markdown-content :deep(th) {
  @apply bg-gray-50 font-semibold text-gray-900;
}

.markdown-content :deep(strong) {
  @apply font-semibold text-gray-900;
}

.markdown-content :deep(em) {
  @apply italic text-gray-700;
}
</style>
