<template>
  <div>
    <!-- Loading state -->
    <LoadingOverlay v-if="loading" />

    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">编辑商品</h1>
        <p class="mt-2 text-sm text-gray-700">编辑商品信息和详情</p>
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
        <NuxtLink
          :to="`/admin/goods/${route.params.id}`"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
          </svg>
          预览
        </NuxtLink>
      </div>
    </div>

    <!-- Edit form -->
    <div v-if="!loading" class="space-y-6">
      <!-- Good Information -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">商品信息</h3>
          <form @submit.prevent="saveGood" class="space-y-6">
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
                  v-model.number="goodForm.stock"
                  type="number"
                  min="0"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  placeholder="请输入库存数量"
                >
              </div>

              <!-- Status -->
              <div>
                <label for="status" class="block text-sm font-medium text-gray-700">商品状态</label>
                <select
                  id="status"
                  v-model="goodForm.status"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                >
                  <option v-for="status in goodStatuses" :key="status.value" :value="status.value">
                    {{ status.name }}
                  </option>
                </select>
              </div>

              <!-- Category -->
              <div>
                <label for="category" class="block text-sm font-medium text-gray-700">商品分类</label>
                <input
                  id="category"
                  v-model="goodForm.category"
                  type="text"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  placeholder="请输入商品分类"
                >
              </div>

              <!-- Summary -->
              <div class="lg:col-span-2">
                <label for="summary" class="block text-sm font-medium text-gray-700">商品简介</label>
                <textarea
                  id="summary"
                  v-model="goodForm.summary"
                  rows="3"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  placeholder="请输入商品简介"
                />
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
              <button
                type="submit"
                :disabled="savingGood"
                class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50"
              >
                <svg v-if="savingGood" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                保存商品信息
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Article Information -->
      <div v-if="articleForm.title" class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">商品详情文章</h3>
          <form @submit.prevent="saveArticle" class="space-y-6">
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
            <div class="flex justify-end space-x-3 pt-6 border-t border-gray-200">
              <button
                type="submit"
                :disabled="savingArticle"
                class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-green-600 hover:bg-green-700 disabled:opacity-50"
              >
                <svg v-if="savingArticle" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                保存详情文章
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { QuillEditor } from '@vueup/vue-quill'
import '@vueup/vue-quill/dist/vue-quill.snow.css'
import { GoodsService, ArticlesService } from '~/api'
import type { GoodUpdateRequest, ArticleUpdateRequest } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const route = useRoute()
const { isAdmin } = usePermissions()
const userStore = useUserStore()

// Constants
const GOOD_STATUS = {
  DRAFT: 0,
  AVAILABLE: 1,
  UNAVAILABLE: 2
}

// Available options
const goodStatuses = [
  { value: GOOD_STATUS.DRAFT, name: '草稿' },
  { value: GOOD_STATUS.AVAILABLE, name: '可购买' },
  { value: GOOD_STATUS.UNAVAILABLE, name: '不可购买' }
]

// Form data
const loading = ref(true)
const savingGood = ref(false)
const savingArticle = ref(false)

const goodForm = ref<GoodUpdateRequest>({
  name: '',
  summary: '',
  price: 0,
  stock: 0,
  status: GOOD_STATUS.DRAFT,
  imageUrl: '',
  category: ''
})

const articleForm = ref<ArticleUpdateRequest>({
  title: '',
  summary: '',
  body: '',
  imageUrl: '',
  tags: []
})

const tagsInput = ref('')
const originalGood = ref(null)
const articleId = ref<string | null>(null)

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

// Load good and article data
const loadData = async () => {
  try {
    const goodId = route.params.id as string
    const goodResponse = await GoodsService.goodGetEndpoint(goodId)
    
    // Check if user has permission to edit this good
    if (!isAdmin() && goodResponse.authorId !== userStore.userId) {
      throw createError({
        statusCode: 403,
        statusMessage: '您没有权限编辑此商品'
      })
    }
    
    originalGood.value = goodResponse
    
    // Populate good form
    goodForm.value = {
      name: goodResponse.name || '',
      summary: goodResponse.summary || '',
      price: goodResponse.price || 0,
      stock: goodResponse.stock || 0,
      status: goodResponse.status || GOOD_STATUS.DRAFT,
      imageUrl: goodResponse.imageUrl || '',
      category: goodResponse.category || ''
    }
    
    // Load associated article if exists
    if (goodResponse.articleId) {
      articleId.value = goodResponse.articleId
      const articleResponse = await ArticlesService.articleGetEndpoint(goodResponse.articleId)
      
      articleForm.value = {
        title: articleResponse.title || '',
        summary: articleResponse.summary || '',
        body: articleResponse.body || '',
        imageUrl: articleResponse.imageUrl || '',
        tags: articleResponse.tags || []
      }
      
      // Set tags input
      tagsInput.value = (articleResponse.tags || []).join(', ')
    }
    
  } catch (error) {
    console.error('Failed to load data:', error)
    const { error: showError } = useGlobalNotifications()
    showError('加载失败', '无法加载商品数据')
    await navigateTo('/admin/goods')
  } finally {
    loading.value = false
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

// Save good
const saveGood = async () => {
  if (!goodForm.value.name || goodForm.value.price <= 0) {
    const { error } = useGlobalNotifications()
    error('请填写必填字段', '商品名称和价格不能为空')
    return
  }

  savingGood.value = true

  try {
    const goodId = route.params.id as string
    await GoodsService.goodPatchEndpoint(goodId, goodForm.value)
    
    const { success } = useGlobalNotifications()
    success('保存成功', '商品信息已保存')
  } catch (error) {
    console.error('Failed to save good:', error)
    const { error: showError } = useGlobalNotifications()
    showError('保存失败', '商品信息保存失败，请重试')
  } finally {
    savingGood.value = false
  }
}

// Save article
const saveArticle = async () => {
  if (!articleForm.value.title || !articleForm.value.body) {
    const { error } = useGlobalNotifications()
    error('请填写必填字段', '文章标题和内容不能为空')
    return
  }

  savingArticle.value = true

  try {
    if (articleId.value) {
      await ArticlesService.articlePatchEndpoint(articleId.value, articleForm.value)
    }
    
    const { success } = useGlobalNotifications()
    success('保存成功', '详情文章已保存')
  } catch (error) {
    console.error('Failed to save article:', error)
    const { error: showError } = useGlobalNotifications()
    showError('保存失败', '详情文章保存失败，请重试')
  } finally {
    savingArticle.value = false
  }
}

// Load data on mount
onMounted(() => {
  loadData()
})
</script>
