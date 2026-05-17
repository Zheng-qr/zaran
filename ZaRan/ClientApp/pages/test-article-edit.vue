<template>
  <div class="container mx-auto px-4 py-8">
    <div class="max-w-4xl mx-auto">
      <h1 class="text-3xl font-bold text-gray-900 mb-8">文章编辑器测试</h1>
      
      <div class="bg-white shadow rounded-lg p-6">
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Title -->
          <div>
            <label for="title" class="block text-sm font-medium text-gray-700">文章标题 *</label>
            <input
              id="title"
              v-model="form.title"
              type="text"
              required
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="请输入文章标题"
            >
          </div>

          <!-- Summary -->
          <div>
            <label for="summary" class="block text-sm font-medium text-gray-700">文章摘要</label>
            <textarea
              id="summary"
              v-model="form.summary"
              rows="3"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="请输入文章摘要（可选）"
            />
          </div>

          <!-- Color -->
          <div>
            <label for="color" class="block text-sm font-medium text-gray-700">文章主题色</label>
            <div class="mt-1 flex items-center space-x-3">
              <input
                id="color"
                v-model="form.color"
                type="color"
                class="h-10 w-20 border border-gray-300 rounded-md cursor-pointer"
                title="选择文章主题色"
              >
              <input
                v-model="form.color"
                type="text"
                class="flex-1 border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                placeholder="#3B82F6"
                pattern="^#[0-9A-Fa-f]{6}$"
              >
              <button
                type="button"
                @click="form.color = null"
                class="px-3 py-2 text-sm text-gray-600 hover:text-gray-800 border border-gray-300 rounded-md hover:bg-gray-50"
              >
                清除
              </button>
            </div>
            <p class="mt-1 text-sm text-gray-500">设置文章的主题色，用于页面装饰和强调显示</p>
          </div>

          <!-- Content Editor -->
          <div>
            <label for="content" class="block text-sm font-medium text-gray-700">文章内容 *</label>
            <div class="mt-1">
              <ClientOnly>
                <QuillEditor
                  v-model:content="form.body"
                  content-type="html"
                  :options="editorOptions"
                  class="min-h-96"
                />
                <template #fallback>
                  <div class="min-h-96 bg-gray-50 border border-gray-300 rounded p-4 flex items-center justify-center">
                    <div class="text-gray-500">编辑器加载中...</div>
                  </div>
                </template>
              </ClientOnly>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex justify-end space-x-3 pt-6 border-t border-gray-200">
            <button
              type="button"
              @click="clearForm"
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
            >
              清空
            </button>
            <button
              type="submit"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
            >
              保存测试
            </button>
          </div>
        </form>

        <!-- Preview -->
        <div class="mt-8 border-t pt-6">
          <h3 class="text-lg font-medium text-gray-900 mb-3">表单数据预览：</h3>
          <pre class="bg-gray-100 p-4 rounded text-sm overflow-auto">{{ JSON.stringify(form, null, 2) }}</pre>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { QuillEditor } from '@vueup/vue-quill'
import '@vueup/vue-quill/dist/vue-quill.snow.css'

// 页面元数据
useHead({
  title: '文章编辑器测试 - 檐下风铃',
  meta: [
    { name: 'description', content: '测试文章编辑器功能' }
  ]
})

// 表单数据
const form = ref({
  title: '测试文章标题',
  summary: '这是一个测试文章的摘要',
  body: '<h1>欢迎使用富文本编辑器</h1><p>这是一个测试内容，您可以在这里编辑文章。</p><ul><li>支持富文本编辑</li><li>支持多种格式</li><li>易于使用</li></ul>',
  color: '#3B82F6'
})

// 编辑器配置
const editorOptions = {
  theme: 'snow',
  placeholder: '请输入文章内容，支持富文本编辑...',
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

// 方法
const handleSubmit = () => {
  console.log('表单提交:', form.value)
  alert('表单数据已输出到控制台')
}

const clearForm = () => {
  form.value = {
    title: '',
    summary: '',
    body: '',
    color: null
  }
}
</script>

<style scoped>
.container {
  min-height: 100vh;
  background-color: #f9fafb;
}
</style>
