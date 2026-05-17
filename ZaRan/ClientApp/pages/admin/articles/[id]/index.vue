<template>
  <div>
    <LoadingOverlay v-if="loading" />
    
    <div v-else-if="article" class="space-y-6">
      <!-- Header -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-4 py-5 sm:p-6">
          <div class="flex items-center justify-between">
            <div class="flex-1">
              <h1 class="text-2xl font-bold text-gray-900">{{ article.title }}</h1>
              <div class="flex items-center space-x-4 mt-2">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="getTypeColorClass(article.type)">
                  {{ getTypeName(article.type) }}
                </span>
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="getStatusColorClass(article.status)">
                  {{ getStatusName(article.status) }}
                </span>
                <span class="text-sm text-gray-500">
                  发布于 {{ formatDate(article.createdAt) }}
                </span>
              </div>
            </div>
            <div class="flex space-x-3">
              <NuxtLink
                :to="`/admin/articles/${article.id}/edit`"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
              >
                编辑文章
              </NuxtLink>
              <NuxtLink
                to="/admin/articles"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
              >
                返回列表
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>

      <!-- Article Content -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Main Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Article Image -->
          <div v-if="article.imageUrl" class="bg-white shadow rounded-lg overflow-hidden">
            <img 
              :src="article.imageUrl" 
              :alt="article.title"
              class="w-full h-64 object-cover"
            >
          </div>

          <!-- Article Body -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">文章内容</h3>
              <div class="prose max-w-none">
                <div v-if="article.summary" class="text-gray-600 italic mb-4">
                  {{ article.summary }}
                </div>
                <div v-html="article.body || '暂无内容'" class="text-gray-900"></div>
              </div>
            </div>
          </div>

          <!-- Tags -->
          <div v-if="article.tags && article.tags.length > 0" class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">标签</h3>
              <div class="flex flex-wrap gap-2">
                <span 
                  v-for="tag in article.tags" 
                  :key="tag"
                  class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800"
                >
                  {{ tag }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="space-y-6">
          <!-- Author Info -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">作者信息</h3>
              <div class="flex items-center space-x-3">
                <img 
                  class="h-12 w-12 rounded-full" 
                  :src="article.author?.avatar || '/default-avatar.png'" 
                  :alt="article.author?.nickname"
                >
                <div>
                  <p class="text-sm font-medium text-gray-900">{{ article.author?.nickname }}</p>
                  <p class="text-sm text-gray-500">@{{ article.author?.username }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Article Meta -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">文章信息</h3>
              <dl class="space-y-3">
                <div>
                  <dt class="text-sm font-medium text-gray-500">创建时间</dt>
                  <dd class="text-sm text-gray-900">{{ formatDate(article.createdAt) }}</dd>
                </div>
                <div>
                  <dt class="text-sm font-medium text-gray-500">更新时间</dt>
                  <dd class="text-sm text-gray-900">{{ formatDate(article.updatedAt) }}</dd>
                </div>
                <div v-if="article.color">
                  <dt class="text-sm font-medium text-gray-500">主题色</dt>
                  <dd class="flex items-center space-x-2">
                    <div 
                      class="w-4 h-4 rounded border border-gray-300"
                      :style="{ backgroundColor: article.color }"
                    ></div>
                    <span class="text-sm text-gray-900">{{ article.color }}</span>
                  </dd>
                </div>
              </dl>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">快速操作</h3>
              <div class="space-y-3">
                <button
                  v-if="article.status === ARTICLE_STATUS.DRAFT"
                  @click="publishArticle"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-green-300 rounded-md shadow-sm text-sm font-medium text-green-700 bg-white hover:bg-green-50"
                >
                  发布文章
                </button>
                <button
                  v-else-if="article.status === ARTICLE_STATUS.PUBLISHED"
                  @click="archiveArticle"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-yellow-300 rounded-md shadow-sm text-sm font-medium text-yellow-700 bg-white hover:bg-yellow-50"
                >
                  归档文章
                </button>
                <button
                  @click="deleteArticle"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-red-300 rounded-md shadow-sm text-sm font-medium text-red-700 bg-white hover:bg-red-50"
                >
                  删除文章
                </button>
                <NuxtLink
                  :to="`/articles/${article.id}`"
                  target="_blank"
                  class="w-full inline-flex justify-center items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  前台预览
                </NuxtLink>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-12">
      <h3 class="mt-2 text-sm font-medium text-gray-900">文章不存在</h3>
      <p class="mt-1 text-sm text-gray-500">请检查文章ID是否正确</p>
      <div class="mt-6">
        <NuxtLink
          to="/admin/articles"
          class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
        >
          返回文章列表
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ArticlesService, AdminService } from '~/api'
import type { ArticleDetailResponse } from '~/api'

// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

const route = useRoute()
const router = useRouter()

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
  POST: 4
}

// Data
const article = ref<ArticleDetailResponse | null>(null)
const loading = ref(false)

// Available options
const articleTypes = [
  { value: ARTICLE_TYPE.STORY, name: '故事' },
  { value: ARTICLE_TYPE.CHARACTER, name: '角色介绍' },
  { value: ARTICLE_TYPE.GENE, name: '基因库' },
  { value: ARTICLE_TYPE.WIKI, name: '百科' },
  { value: ARTICLE_TYPE.POST, name: '社区帖子' }
]

const articleStatuses = [
  { value: ARTICLE_STATUS.DRAFT, name: '草稿' },
  { value: ARTICLE_STATUS.PUBLISHED, name: '已发布' },
  { value: ARTICLE_STATUS.BANNED, name: '已封禁' },
  { value: ARTICLE_STATUS.ARCHIVED, name: '已归档' }
]

// Load article data
const loadArticle = async () => {
  const articleId = route.params.id as string
  if (!articleId) return

  loading.value = true

  try {
    article.value = await ArticlesService.articleGetEndpoint(articleId)
  } catch (error) {
    console.error('Failed to load article:', error)
    article.value = null
  } finally {
    loading.value = false
  }
}

// Article actions
const publishArticle = async () => {
  if (!article.value?.id) return
  if (!confirm(`确定要发布文章《${article.value.title}》吗？`)) return

  try {
    await AdminService.adminUpdateArticleStatusEndpoint(article.value.id, { status: ARTICLE_STATUS.PUBLISHED })
    await loadArticle()
  } catch (error) {
    console.error('Failed to publish article:', error)
  }
}

const archiveArticle = async () => {
  if (!article.value?.id) return
  if (!confirm(`确定要归档文章《${article.value.title}》吗？`)) return

  try {
    await AdminService.adminUpdateArticleStatusEndpoint(article.value.id, { status: ARTICLE_STATUS.ARCHIVED })
    await loadArticle()
  } catch (error) {
    console.error('Failed to archive article:', error)
  }
}

const deleteArticle = async () => {
  if (!article.value?.id) return
  if (!confirm(`确定要删除文章《${article.value.title}》吗？此操作不可恢复！`)) return

  try {
    await AdminService.adminDeleteArticleEndpoint(article.value.id)
    await router.push('/admin/articles')
  } catch (error) {
    console.error('Failed to delete article:', error)
  }
}

// Utility functions
const getTypeName = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const typeObj = articleTypes.find(t => t.value === typeValue)
  return typeObj?.name || '未知'
}

const getStatusName = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const statusObj = articleStatuses.find(s => s.value === statusValue)
  return statusObj?.name || '未知'
}

const getTypeColorClass = (type: number | undefined) => {
  const typeValue = typeof type === 'number' ? type : 0
  const colorMap = {
    0: 'bg-blue-100 text-blue-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-purple-100 text-purple-800',
    3: 'bg-yellow-100 text-yellow-800',
    4: 'bg-pink-100 text-pink-800'
  }
  return colorMap[typeValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const getStatusColorClass = (status: number | undefined) => {
  const statusValue = typeof status === 'number' ? status : 0
  const colorMap = {
    0: 'bg-gray-100 text-gray-800',
    1: 'bg-green-100 text-green-800',
    2: 'bg-red-100 text-red-800',
    3: 'bg-yellow-100 text-yellow-800'
  }
  return colorMap[statusValue as keyof typeof colorMap] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '-'
  return new Intl.DateTimeFormat('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(dateString))
}

// Load data on mount
onMounted(() => {
  loadArticle()
})
</script>

<style scoped>
.prose {
  max-width: none;
}

.prose h1, .prose h2, .prose h3, .prose h4, .prose h5, .prose h6 {
  margin-top: 1.5em;
  margin-bottom: 0.5em;
  font-weight: 600;
}

.prose p {
  margin-bottom: 1em;
  line-height: 1.6;
}

.prose ul, .prose ol {
  margin-bottom: 1em;
  padding-left: 1.5em;
}

.prose blockquote {
  border-left: 4px solid #e5e7eb;
  padding-left: 1em;
  margin: 1em 0;
  font-style: italic;
  color: #6b7280;
}
</style>
