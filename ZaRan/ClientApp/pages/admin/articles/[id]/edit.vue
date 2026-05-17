<template>
  <div>
    <!-- Loading state -->
    <LoadingOverlay v-if="loading" />

    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">编辑文章</h1>
        <p class="mt-2 text-sm text-gray-700">编辑文章内容</p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <NuxtLink
          to="/admin/articles"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          返回列表
        </NuxtLink>
        <NuxtLink
          :to="`/admin/articles/${route.params.id}`"
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

    <!-- Article form -->
    <div v-if="!loading" class="bg-white shadow rounded-lg">
      <form @submit.prevent="handleSubmit" class="space-y-6 p-6">
        <!-- Basic Information -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Title -->
          <div class="lg:col-span-2">
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

          <!-- Type -->
          <div>
            <label for="type" class="block text-sm font-medium text-gray-700">文章类型 *</label>
            <select
              id="type"
              v-model="form.type"
              required
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">请选择文章类型</option>
              <option v-for="type in articleTypes" :key="type.value" :value="type.value">
                {{ type.name }}
              </option>
            </select>
          </div>

          <!-- Status -->
          <div>
            <label for="status" class="block text-sm font-medium text-gray-700">发布状态</label>
            <select
              id="status"
              v-model="form.status"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option v-for="status in articleStatuses" :key="status.value" :value="status.value">
                {{ status.name }}
              </option>
            </select>
          </div>

          <!-- Summary -->
          <div class="lg:col-span-2">
            <label for="summary" class="block text-sm font-medium text-gray-700">文章摘要</label>
            <textarea
              id="summary"
              v-model="form.summary"
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

          <!-- Article Color -->
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
        </div>

        <!-- Image Upload -->
        <div>
          <label class="block text-sm font-medium text-gray-700">文章封面</label>
          <FileUpload
            v-model="form.imageUrl"
            accept="image/*"
            :max-size="5 * 1024 * 1024"
            class="mt-1"
          />
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

        <!-- Form Actions -->
        <div class="flex justify-end space-x-3 pt-6 border-t border-gray-200">
          <NuxtLink
            to="/admin/articles"
            class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
          >
            取消
          </NuxtLink>
          <button
            type="button"
            @click="handleSaveDraft"
            :disabled="saving"
            class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
          >
            保存草稿
          </button>
          <button
            type="submit"
            :disabled="saving"
            class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50"
          >
            <svg v-if="saving" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            {{ form.status === ARTICLE_STATUS.PUBLISHED ? '发布文章' : '保存文章' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { QuillEditor } from '@vueup/vue-quill'
import '@vueup/vue-quill/dist/vue-quill.snow.css'
import { ArticlesService } from '~/api'
import type { ArticleUpdateRequest } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const route = useRoute()
const { isAdmin } = usePermissions()
const userStore = useUserStore()

// Article constants
const ARTICLE_STATUS = {
  DRAFT: 0,
  PUBLISHED: 1,
  BANNED: 2,
  ARCHIVED: 3
}

const ARTICLE_TYPE = {
  STORY: 0,
  CHARACTER: 1,
  GENE: 2,
  WIKI: 3,
  POST: 4,
  PATTERN: 5
}

// Available options
const articleTypes = [
  { value: ARTICLE_TYPE.STORY, name: '故事' },
  { value: ARTICLE_TYPE.CHARACTER, name: '角色介绍' },
  { value: ARTICLE_TYPE.GENE, name: '基因库' },
  { value: ARTICLE_TYPE.WIKI, name: '百科' },
  { value: ARTICLE_TYPE.POST, name: '社区帖子' },
  { value: ARTICLE_TYPE.PATTERN, name: '纹样' }
]

const articleStatuses = [
  { value: ARTICLE_STATUS.DRAFT, name: '草稿' },
  { value: ARTICLE_STATUS.PUBLISHED, name: '发布' },
  { value: ARTICLE_STATUS.ARCHIVED, name: '归档' }
]

// Form data
const form = ref<ArticleUpdateRequest>({
  title: '',
  summary: '',
  body: '',
  type: ARTICLE_TYPE.STORY,
  status: ARTICLE_STATUS.DRAFT,
  imageUrl: '',
  color: null,
  tags: []
})

const tagsInput = ref('')
const loading = ref(true)
const saving = ref(false)
const originalArticle = ref(null)

// Enhanced editor options for better content editing
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





// Load article data
const loadArticle = async () => {
  try {
    const articleId = route.params.id as string
    const response = await ArticlesService.articleGetEndpoint(articleId)
    
    // Check if user has permission to edit this article
    if (!isAdmin() && response.authorId !== userStore.userId) {
      throw createError({
        statusCode: 403,
        statusMessage: '您没有权限编辑此文章'
      })
    }
    
    originalArticle.value = response
    
    // Populate form
    form.value = {
      title: response.title || '',
      summary: response.summary || '',
      body: response.body || '',
      type: response.type || ARTICLE_TYPE.STORY,
      status: response.status || ARTICLE_STATUS.DRAFT,
      imageUrl: response.imageUrl || '',
      color: response.color || null,
      tags: response.tags || []
    }
    
    // Set tags input
    tagsInput.value = (response.tags || []).join(', ')
    
  } catch (error) {
    console.error('Failed to load article:', error)
    const { error: showError } = useGlobalNotifications()
    showError('加载失败', '无法加载文章数据')
    await navigateTo('/admin/articles')
  } finally {
    loading.value = false
  }
}

// Watch tags input
watch(tagsInput, (newValue) => {
  if (newValue) {
    form.value.tags = newValue.split(',').map(tag => tag.trim()).filter(tag => tag.length > 0)
  } else {
    form.value.tags = []
  }
})

// Handle form submission
const handleSubmit = async () => {
  await saveArticle(form.value.status)
}

// Handle save draft
const handleSaveDraft = async () => {
  await saveArticle(ARTICLE_STATUS.DRAFT)
}

// Save article
const saveArticle = async (status: number) => {
  if (!form.value.title || !form.value.body) {
    const { error } = useGlobalNotifications()
    error('请填写必填字段', '标题和内容不能为空')
    return
  }

  saving.value = true

  try {
    const articleId = route.params.id as string
    const articleData = {
      ...form.value,
      status
    }

    await ArticlesService.articlePatchEndpoint(articleId, articleData)
    
    const { success } = useGlobalNotifications()
    success(
      status === ARTICLE_STATUS.PUBLISHED ? '文章发布成功' : '文章保存成功',
      status === ARTICLE_STATUS.PUBLISHED ? '文章已成功发布' : '文章已保存'
    )

    // Redirect to article list
    await navigateTo('/admin/articles')
  } catch (error) {
    console.error('Failed to save article:', error)
    const { error: showError } = useGlobalNotifications()
    showError('保存失败', '文章保存失败，请重试')
  } finally {
    saving.value = false
  }
}

// Load article on mount
onMounted(() => {
  loadArticle()
})
</script>
