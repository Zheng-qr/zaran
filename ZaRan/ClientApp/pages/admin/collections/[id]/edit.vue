<template>
  <div>
    <!-- Loading state -->
    <LoadingOverlay v-if="loading" />

    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">编辑集合</h1>
        <p class="mt-2 text-sm text-gray-700">编辑集合信息和内容</p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <NuxtLink
          to="/admin/collections"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          返回列表
        </NuxtLink>
        <NuxtLink
          :to="`/admin/collections/${route.params.id}`"
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

    <!-- Collection form -->
    <div v-if="!loading" class="bg-white shadow rounded-lg">
      <form @submit.prevent="handleSubmit" class="space-y-6 p-6">
        <!-- Basic Information -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Name -->
          <div class="lg:col-span-2">
            <label for="name" class="block text-sm font-medium text-gray-700">集合名称 *</label>
            <input
              id="name"
              v-model="form.name"
              type="text"
              required
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="请输入集合名称"
            >
          </div>

          <!-- Type -->
          <div>
            <label for="type" class="block text-sm font-medium text-gray-700">集合类型 *</label>
            <select
              id="type"
              v-model="form.type"
              required
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="">请选择集合类型</option>
              <option v-for="type in collectionTypes" :key="type.value" :value="type.value">
                {{ type.name }}
              </option>
            </select>
          </div>

          <!-- Visibility -->
          <div>
            <label for="visibility" class="block text-sm font-medium text-gray-700">可见性</label>
            <select
              id="visibility"
              v-model="form.isPublic"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option :value="true">公开</option>
              <option :value="false">私有</option>
            </select>
          </div>

          <!-- Summary -->
          <div class="lg:col-span-2">
            <label for="summary" class="block text-sm font-medium text-gray-700">集合简介</label>
            <textarea
              id="summary"
              v-model="form.summary"
              rows="3"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              placeholder="请输入集合简介（可选）"
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

        <!-- Image Upload -->
        <div>
          <label class="block text-sm font-medium text-gray-700">集合封面</label>
          <FileUpload
            v-model="form.imageUrl"
            accept="image/*"
            :max-size="5 * 1024 * 1024"
            class="mt-1"
          />
        </div>

        <!-- Description Editor -->
        <div>
          <label for="description" class="block text-sm font-medium text-gray-700">详细描述</label>
          <div class="mt-1">
            <QuillEditor
              v-model:content="form.description"
              content-type="html"
              :options="editorOptions"
              class="min-h-64"
            />
          </div>
        </div>

        <!-- Form Actions -->
        <div class="flex justify-end space-x-3 pt-6 border-t border-gray-200">
          <NuxtLink
            to="/admin/collections"
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
            保存集合
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { QuillEditor } from '@vueup/vue-quill'
import '@vueup/vue-quill/dist/vue-quill.snow.css'
import { CollectionsService } from '~/api'
import type { CollectionPatchEndpointRequest } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const route = useRoute()
const { isAdmin } = usePermissions()
const userStore = useUserStore()

// Collection constants
const COLLECTION_TYPE = {
  UNSPECIFIED: 0,
  ARTICLE: 1,
  GOOD: 2,
  USER: 3
}

// Available options
const collectionTypes = [
  { value: COLLECTION_TYPE.ARTICLE, name: '文章集合' },
  { value: COLLECTION_TYPE.GOOD, name: '商品集合' },
  { value: COLLECTION_TYPE.USER, name: '用户集合' }
]

// Form data
const form = ref<CollectionPatchEndpointRequest>({
  name: '',
  summary: '',
  description: '',
  type: COLLECTION_TYPE.ARTICLE,
  isPublic: true,
  imageUrl: '',
  tags: []
})

const tagsInput = ref('')
const loading = ref(true)
const saving = ref(false)
const originalCollection = ref(null)

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

// Load collection data
const loadCollection = async () => {
  try {
    const collectionId = route.params.id as string
    const response = await CollectionsService.collectionGetEndpoint(collectionId)
    
    // Check if user has permission to edit this collection
    if (!isAdmin() && response.creatorId !== userStore.userId) {
      throw createError({
        statusCode: 403,
        statusMessage: '您没有权限编辑此集合'
      })
    }
    
    originalCollection.value = response
    
    // Populate form
    form.value = {
      name: response.name || '',
      summary: response.summary || '',
      description: response.description || '',
      type: response.type || COLLECTION_TYPE.ARTICLE,
      isPublic: response.isPublic ?? true,
      imageUrl: response.imageUrl || '',
      tags: response.tags || []
    }
    
    // Set tags input
    tagsInput.value = (response.tags || []).join(', ')
    
  } catch (error) {
    console.error('Failed to load collection:', error)
    const { error: showError } = useGlobalNotifications()
    showError('加载失败', '无法加载集合数据')
    await navigateTo('/admin/collections')
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
  await saveCollection(form.value.isPublic)
}

// Handle save draft
const handleSaveDraft = async () => {
  await saveCollection(false)
}

// Save collection
const saveCollection = async (isPublic: boolean) => {
  if (!form.value.name) {
    const { error } = useGlobalNotifications()
    error('请填写必填字段', '集合名称不能为空')
    return
  }

  saving.value = true

  try {
    const collectionId = route.params.id as string
    const collectionData = {
      ...form.value,
      isPublic
    }

    await CollectionsService.collectionPatchEndpoint(collectionId, collectionData)
    
    const { success } = useGlobalNotifications()
    success(
      isPublic ? '集合发布成功' : '集合保存成功',
      isPublic ? '集合已成功发布' : '集合已保存'
    )

    // Redirect to collection list
    await navigateTo('/admin/collections')
  } catch (error) {
    console.error('Failed to save collection:', error)
    const { error: showError } = useGlobalNotifications()
    showError('保存失败', '集合保存失败，请重试')
  } finally {
    saving.value = false
  }
}

// Load collection on mount
onMounted(() => {
  loadCollection()
})
</script>
