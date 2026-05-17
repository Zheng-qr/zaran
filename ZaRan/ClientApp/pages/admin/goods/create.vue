<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">创建商品</h1>
        <p class="mt-2 text-sm text-gray-700">创建新的商品信息</p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <NuxtLink
          to="/admin/goods"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          返回列表
        </NuxtLink>
      </div>
    </div>

    <!-- Multi-step form -->
    <div class="bg-white shadow rounded-lg">
      <!-- Step indicator -->
      <div class="px-4 py-5 sm:p-6 border-b border-gray-200">
        <nav aria-label="Progress">
          <ol class="flex items-center">
            <li class="relative">
              <div class="flex items-center">
                <div class="flex items-center justify-center w-8 h-8 bg-blue-600 rounded-full">
                  <span class="text-white text-sm font-medium">1</span>
                </div>
                <span class="ml-3 text-sm font-medium text-gray-900">商品信息</span>
              </div>
            </li>
            <li class="relative ml-8">
              <div class="flex items-center">
                <div class="flex items-center justify-center w-8 h-8 border-2 border-gray-300 rounded-full">
                  <span class="text-gray-500 text-sm font-medium">2</span>
                </div>
                <span class="ml-3 text-sm font-medium text-gray-500">商品详情</span>
              </div>
            </li>
          </ol>
        </nav>
      </div>

      <!-- Step 1: Basic Information -->
      <div v-if="currentStep === 1" class="px-4 py-5 sm:p-6">
        <form @submit.prevent="nextStep" class="space-y-6">
          <!-- Basic Information -->
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <!-- Name -->
            <div class="lg:col-span-2">
              <label for="name" class="block text-sm font-medium text-gray-700">商品名称 *</label>
              <input
                id="name"
                v-model="goodForm.name"
                type="text"
                required
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                placeholder="请输入商品名称"
              >
            </div>

            <!-- Price -->
            <div>
              <label for="price" class="block text-sm font-medium text-gray-700">商品价格 *</label>
              <div class="mt-1 relative rounded-md shadow-sm">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-gray-500 sm:text-sm">¥</span>
                </div>
                <input
                  id="price"
                  v-model.number="goodForm.price"
                  type="number"
                  step="0.01"
                  min="0"
                  required
                  class="pl-7 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  placeholder="0.00"
                >
              </div>
            </div>

            <!-- Stock -->
            <div>
              <label for="stock" class="block text-sm font-medium text-gray-700">库存数量</label>
              <input
                id="stock"
                v-model.number="goodForm.stoke"
                type="number"
                min="0"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                placeholder="请输入库存数量"
              >
            </div>

            <!-- Discounted Price -->
            <div>
              <label for="discountedPrice" class="block text-sm font-medium text-gray-700">折扣价格</label>
              <div class="mt-1 relative rounded-md shadow-sm">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-gray-500 sm:text-sm">¥</span>
                </div>
                <input
                  id="discountedPrice"
                  v-model.number="goodForm.discountedPrice"
                  type="number"
                  step="0.01"
                  min="0"
                  class="pl-7 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  placeholder="0.00"
                >
              </div>
              <p class="mt-1 text-sm text-gray-500">留空表示无折扣</p>
            </div>

            <!-- Copyright Info -->
            <div class="lg:col-span-2">
              <label for="copyrightInfo" class="block text-sm font-medium text-gray-700">版权信息</label>
              <textarea
                id="copyrightInfo"
                v-model="goodForm.copyrightInfo"
                rows="2"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                placeholder="请输入版权信息，如：版权所有、使用权限、授权范围等"
              />
              <p class="mt-1 text-sm text-gray-500">
                填写版权信息有助于区分版权商品和实体商品，明确商品的使用权限
              </p>
            </div>
          </div>

          <!-- Image Upload -->
          <div>
            <label class="block text-sm font-medium text-gray-700">商品图片</label>
            <FileUpload
              v-model="goodForm.imageUrl"
              accept="image/*"
              :max-size="5 * 1024 * 1024"
              class="mt-1"
            />
          </div>

          <!-- Form Actions -->
          <div class="flex justify-end space-x-3 pt-6 border-t border-gray-200">
            <NuxtLink
              to="/admin/goods"
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
            >
              取消
            </NuxtLink>
            <button
              type="submit"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
            >
              下一步：创建详情
            </button>
          </div>
        </form>
      </div>

      <!-- Step 2: Article Creation -->
      <div v-if="currentStep === 2" class="px-4 py-5 sm:p-6">
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Article Information -->
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <!-- Article Title -->
            <div class="lg:col-span-2">
              <label for="articleTitle" class="block text-sm font-medium text-gray-700">详情文章标题 *</label>
              <input
                id="articleTitle"
                v-model="articleForm.title"
                type="text"
                required
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                placeholder="请输入详情文章标题"
              >
            </div>

            <!-- Article Summary -->
            <div class="lg:col-span-2">
              <label for="articleSummary" class="block text-sm font-medium text-gray-700">文章摘要</label>
              <textarea
                id="articleSummary"
                v-model="articleForm.summary"
                rows="3"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                placeholder="请输入文章摘要（可选）"
              />
            </div>

            <!-- Tags -->
            <div class="lg:col-span-2">
              <label for="tags" class="block text-sm font-medium text-gray-700">标签</label>
              <input
                id="tags"
                v-model="tagsInput"
                type="text"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                placeholder="请输入标签，用逗号分隔"
              >
              <p class="mt-1 text-sm text-gray-500">多个标签请用逗号分隔，例如：扎染,传统工艺,文化</p>
            </div>
          </div>

          <!-- Article Image Upload -->
          <div>
            <label class="block text-sm font-medium text-gray-700">文章封面</label>
            <FileUpload
              v-model="articleForm.imageUrl"
              accept="image/*"
              :max-size="5 * 1024 * 1024"
              class="mt-1"
            />
          </div>

          <!-- Content Editor -->
          <div>
            <label for="content" class="block text-sm font-medium text-gray-700">详情内容 *</label>
            <div class="mt-1">
              <QuillEditor
                v-model:content="articleForm.body"
                content-type="html"
                :options="editorOptions"
                class="min-h-96"
              />
            </div>
          </div>

          <!-- Form Actions -->
          <div class="flex justify-between pt-6 border-t border-gray-200">
            <button
              type="button"
              @click="previousStep"
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
            >
              上一步
            </button>
            <div class="flex space-x-3">
              <NuxtLink
                to="/admin/goods"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
              >
                取消
              </NuxtLink>
              <button
                type="submit"
                :disabled="loading"
                class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50"
              >
                <svg v-if="loading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                创建商品
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { QuillEditor } from '@vueup/vue-quill'
import '@vueup/vue-quill/dist/vue-quill.snow.css'
import { GoodsService, ArticlesService } from '~/api'
import type { GoodPostEndpointRequest, ArticlePostEndpointRequest } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

// Constants
const GOOD_STATUS = {
  DRAFT: 0,
  AVAILABLE: 1,
  UNAVAILABLE: 2
}

const ARTICLE_TYPE = {
  STORY: 0,
  CHARACTER: 1,
  GENE: 2,
  WIKI: 3,
  POST: 4,
  PATTERN: 5
}

const ARTICLE_STATUS = {
  DRAFT: 0,
  PUBLISHED: 1,
  BANNED: 2,
  ARCHIVED: 3
}

// Available options
const goodStatuses = [
  { value: GOOD_STATUS.DRAFT, name: '草稿' },
  { value: GOOD_STATUS.AVAILABLE, name: '可购买' }
]

// Form data
const currentStep = ref(1)
const loading = ref(false)

const goodForm = ref<GoodPostEndpointRequest>({
  name: '',
  price: 0,
  stoke: 0,
  discountedPrice: 0,
  publisherName: '',
  imageUrl: '',
  copyrightInfo: '',
  relatedArticleId: ''
})

const articleForm = ref<ArticlePostEndpointRequest>({
  title: '',
  summary: '',
  body: '',
  type: ARTICLE_TYPE.WIKI, // 商品详情通常是百科类型
  status: ARTICLE_STATUS.PUBLISHED,
  imageUrl: '',
  tags: []
})

const tagsInput = ref('')

// Editor options
const editorOptions = {
  theme: 'snow',
  modules: {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'],
      ['blockquote', 'code-block'],
      [{ 'header': 1 }, { 'header': 2 }],
      [{ 'list': 'ordered'}, { 'list': 'bullet' }],
      [{ 'script': 'sub'}, { 'script': 'super' }],
      [{ 'indent': '-1'}, { 'indent': '+1' }],
      [{ 'direction': 'rtl' }],
      [{ 'size': ['small', false, 'large', 'huge'] }],
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      [{ 'color': [] }, { 'background': [] }],
      [{ 'font': [] }],
      [{ 'align': [] }],
      ['clean'],
      ['link', 'image']
    ]
  }
}

// Watch tags input
watch(tagsInput, (newValue) => {
  if (newValue) {
    articleForm.value.tags = newValue.split(',').map(tag => tag.trim()).filter(tag => tag.length > 0)
  } else {
    articleForm.value.tags = []
  }
})

// Step navigation
const nextStep = () => {
  if (!goodForm.value.name || goodForm.value.price <= 0) {
    const { error } = useGlobalNotifications()
    error('请填写必填字段', '商品名称和价格不能为空')
    return
  }
  
  // Auto-fill article title if empty
  if (!articleForm.value.title) {
    articleForm.value.title = `${goodForm.value.name} - 详细介绍`
  }
  
  currentStep.value = 2
}

const previousStep = () => {
  currentStep.value = 1
}

// Handle form submission
const handleSubmit = async () => {
  if (!articleForm.value.title || !articleForm.value.body) {
    const { error } = useGlobalNotifications()
    error('请填写必填字段', '文章标题和内容不能为空')
    return
  }

  loading.value = true

  try {
    // First create the article
    const articleResponse = await ArticlesService.articlePostEndpoint(articleForm.value)

    // Then create the good with the article ID
    const goodData = {
      ...goodForm.value,
      articleId: articleResponse.id
    }

    await GoodsService.goodPostEndpoint(goodData)
    
    const { success } = useGlobalNotifications()
    success('商品创建成功', '商品和详情文章已成功创建')

    // Redirect to goods list
    await navigateTo('/admin/goods')
  } catch (error) {
    console.error('Failed to create good:', error)
    const { error: showError } = useGlobalNotifications()
    showError('创建失败', '商品创建失败，请重试')
  } finally {
    loading.value = false
  }
}
</script>
