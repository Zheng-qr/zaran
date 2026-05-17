<template>
  <div>
    <!-- Page header -->
    <div class="sm:flex sm:items-center sm:justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">文件管理</h1>
        <p class="mt-2 text-sm text-gray-700">管理系统中的所有上传文件</p>
      </div>
      <div class="mt-4 sm:mt-0 flex space-x-3">
        <button
          @click="refreshFiles"
          :disabled="loading"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          刷新
        </button>
        <button
          @click="showUploadModal = true"
          class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
        >
          <svg class="mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          上传文件
        </button>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white shadow rounded-lg mb-6">
      <div class="px-4 py-5 sm:p-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label for="search" class="block text-sm font-medium text-gray-700">搜索文件</label>
            <input
              id="search"
              v-model="filters.keyword"
              type="text"
              placeholder="文件名"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @input="debouncedSearch"
            >
          </div>
          
          <div>
            <label for="type" class="block text-sm font-medium text-gray-700">文件类型</label>
            <select
              id="type"
              v-model="filters.type"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadFiles"
            >
              <option value="">所有类型</option>
              <option value="image">图片</option>
              <option value="document">文档</option>
              <option value="video">视频</option>
              <option value="audio">音频</option>
              <option value="other">其他</option>
            </select>
          </div>
          
          <div>
            <label for="size" class="block text-sm font-medium text-gray-700">文件大小</label>
            <select
              id="size"
              v-model="filters.size"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadFiles"
            >
              <option value="">所有大小</option>
              <option value="small">小于 1MB</option>
              <option value="medium">1MB - 10MB</option>
              <option value="large">大于 10MB</option>
            </select>
          </div>
          
          <div>
            <label for="sort" class="block text-sm font-medium text-gray-700">排序方式</label>
            <select
              id="sort"
              v-model="filters.sort"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              @change="loadFiles"
            >
              <option value="newest">最新上传</option>
              <option value="oldest">最早上传</option>
              <option value="name">文件名</option>
              <option value="size">文件大小</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- View Toggle -->
    <div class="flex justify-between items-center mb-6">
      <div class="flex items-center space-x-2">
        <span class="text-sm text-gray-700">显示方式:</span>
        <button
          @click="viewMode = 'grid'"
          :class="[
            'p-2 rounded-md',
            viewMode === 'grid' ? 'bg-blue-100 text-blue-600' : 'text-gray-400 hover:text-gray-600'
          ]"
        >
          <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" />
          </svg>
        </button>
        <button
          @click="viewMode = 'list'"
          :class="[
            'p-2 rounded-md',
            viewMode === 'list' ? 'bg-blue-100 text-blue-600' : 'text-gray-400 hover:text-gray-600'
          ]"
        >
          <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 10h16M4 14h16M4 18h16" />
          </svg>
        </button>
      </div>
      
      <div class="text-sm text-gray-500">
        共 {{ totalFiles }} 个文件
      </div>
    </div>

    <!-- Files Display -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-4 py-5 sm:p-6">
        <LoadingOverlay v-if="loading" />
        
        <div v-else-if="files.length === 0" class="text-center py-12">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">没有找到文件</h3>
          <p class="mt-1 text-sm text-gray-500">尝试调整搜索条件或筛选器</p>
        </div>
        
        <!-- Grid View -->
        <div v-else-if="viewMode === 'grid'" class="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
          <div
            v-for="file in files"
            :key="file.id"
            class="file-card"
            @click="selectFile(file)"
            :class="{ 'file-card-selected': selectedFiles.includes(file.id) }"
          >
            <div class="file-preview">
              <img 
                v-if="isImage(file)" 
                :src="file.thumbnailUrl || file.url" 
                :alt="file.name"
                class="file-image"
              >
              <div v-else class="file-icon">
                <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
            </div>
            <div class="file-info">
              <p class="file-name">{{ file.name }}</p>
              <p class="file-size">{{ formatFileSize(file.size) }}</p>
            </div>
            <div class="file-actions">
              <button
                @click.stop="copyUrl(file)"
                class="action-button"
                title="复制链接"
              >
                <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
              </button>
              <button
                @click.stop="deleteFile(file)"
                class="action-button text-red-600 hover:text-red-700"
                title="删除文件"
              >
                <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
              </button>
            </div>
          </div>
        </div>
        
        <!-- List View -->
        <div v-else class="overflow-hidden">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  <input
                    type="checkbox"
                    :checked="selectedFiles.length === files.length"
                    @change="toggleSelectAll"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  >
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  文件
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  大小
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  类型
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  上传时间
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  操作
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="file in files" :key="file.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">
                  <input
                    type="checkbox"
                    :checked="selectedFiles.includes(file.id)"
                    @change="toggleSelectFile(file.id)"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  >
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10">
                      <img 
                        v-if="isImage(file)" 
                        :src="file.thumbnailUrl || file.url" 
                        :alt="file.name"
                        class="h-10 w-10 object-cover rounded"
                      >
                      <div v-else class="h-10 w-10 text-gray-400 flex items-center justify-center">
                        <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                        </svg>
                      </div>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900">{{ file.name }}</div>
                      <div class="text-sm text-gray-500">{{ file.mimeType }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatFileSize(file.size) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getTypeColorClass(file.mimeType)">
                    {{ getFileType(file.mimeType) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(file.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button
                      @click="copyUrl(file)"
                      class="text-blue-600 hover:text-blue-900"
                    >
                      复制链接
                    </button>
                    <button
                      @click="deleteFile(file)"
                      class="text-red-600 hover:text-red-900"
                    >
                      删除
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <AdminPagination
          v-if="files.length > 0"
          :current-page="currentPage"
          :total-items="totalFiles"
          :page-size="pageSize"
          @page-change="handlePageChange"
          class="mt-6"
        />
      </div>
    </div>

    <!-- Bulk Actions -->
    <div v-if="selectedFiles.length > 0" class="fixed bottom-6 left-1/2 transform -translate-x-1/2">
      <div class="bg-white shadow-lg rounded-lg border border-gray-200 px-4 py-3">
        <div class="flex items-center space-x-4">
          <span class="text-sm text-gray-700">已选择 {{ selectedFiles.length }} 个文件</span>
          <button
            @click="bulkDelete"
            class="inline-flex items-center px-3 py-1 border border-transparent text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700"
          >
            批量删除
          </button>
          <button
            @click="clearSelection"
            class="inline-flex items-center px-3 py-1 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50"
          >
            取消选择
          </button>
        </div>
      </div>
    </div>

    <!-- Upload Modal -->
    <div v-if="showUploadModal" class="fixed inset-0 z-50 overflow-y-auto">
      <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" @click="showUploadModal = false"></div>
        <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
          <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
            <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">上传文件</h3>
            <FileUpload
              :multiple="true"
              :max-files="10"
              accept="*"
              @success="handleUploadSuccess"
              @error="handleUploadError"
            />
          </div>
          <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
            <button
              @click="showUploadModal = false"
              class="w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 sm:mt-0 sm:w-auto sm:text-sm"
            >
              关闭
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Set layout and middleware
definePageMeta({
  layout: 'admin',
  middleware: 'admin'
})

// Mock file interface (replace with real API types)
interface FileItem {
  id: string
  name: string
  url: string
  thumbnailUrl?: string
  size: number
  mimeType: string
  createdAt: string
}

// Utility function for debouncing
const useDebounceFn = (fn: (...args: unknown[]) => void, delay: number) => {
  let timeoutId: NodeJS.Timeout
  return (...args: unknown[]) => {
    clearTimeout(timeoutId)
    timeoutId = setTimeout(() => fn(...args), delay)
  }
}

// Data
const files = ref<FileItem[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(24)
const totalFiles = ref(0)
const viewMode = ref<'grid' | 'list'>('grid')
const selectedFiles = ref<string[]>([])
const showUploadModal = ref(false)

// Filters
const filters = ref({
  keyword: '',
  type: '',
  size: '',
  sort: 'newest'
})

// Mock data (replace with real API calls)
const mockFiles: FileItem[] = [
  {
    id: '1',
    name: 'example-image.jpg',
    url: 'https://via.placeholder.com/400x300',
    thumbnailUrl: 'https://via.placeholder.com/150x150',
    size: 1024 * 500, // 500KB
    mimeType: 'image/jpeg',
    createdAt: new Date().toISOString()
  },
  {
    id: '2',
    name: 'document.pdf',
    url: 'https://example.com/document.pdf',
    size: 1024 * 1024 * 2, // 2MB
    mimeType: 'application/pdf',
    createdAt: new Date(Date.now() - 86400000).toISOString() // 1 day ago
  }
]

// Load files (mock implementation)
const loadFiles = async () => {
  loading.value = true
  
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 500))
    
    // Apply filters to mock data
    let filteredFiles = [...mockFiles]
    
    if (filters.value.keyword) {
      filteredFiles = filteredFiles.filter(file => 
        file.name.toLowerCase().includes(filters.value.keyword.toLowerCase())
      )
    }
    
    if (filters.value.type) {
      filteredFiles = filteredFiles.filter(file => {
        const type = getFileType(file.mimeType).toLowerCase()
        return type === filters.value.type
      })
    }
    
    files.value = filteredFiles
    totalFiles.value = filteredFiles.length
  } catch (error) {
    console.error('Failed to load files:', error)
  } finally {
    loading.value = false
  }
}

// Debounced search
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1
  loadFiles()
}, 500)

// Refresh files
const refreshFiles = () => {
  loadFiles()
}

// Handle page change
const handlePageChange = (page: number) => {
  currentPage.value = page
  loadFiles()
}

// File selection
const selectFile = (file: FileItem) => {
  const index = selectedFiles.value.indexOf(file.id)
  if (index > -1) {
    selectedFiles.value.splice(index, 1)
  } else {
    selectedFiles.value.push(file.id)
  }
}

const toggleSelectFile = (fileId: string) => {
  const index = selectedFiles.value.indexOf(fileId)
  if (index > -1) {
    selectedFiles.value.splice(index, 1)
  } else {
    selectedFiles.value.push(fileId)
  }
}

const toggleSelectAll = () => {
  if (selectedFiles.value.length === files.value.length) {
    selectedFiles.value = []
  } else {
    selectedFiles.value = files.value.map(file => file.id)
  }
}

const clearSelection = () => {
  selectedFiles.value = []
}

// File actions
const copyUrl = async (file: FileItem) => {
  try {
    await navigator.clipboard.writeText(file.url)
    // Show success notification
    const { success } = useGlobalNotifications()
    success('链接已复制到剪贴板')
  } catch (error) {
    console.error('Failed to copy URL:', error)
  }
}

const deleteFile = async (file: FileItem) => {
  if (!confirm(`确定要删除文件 ${file.name} 吗？此操作不可恢复！`)) return
  
  try {
    // Mock delete API call
    await new Promise(resolve => setTimeout(resolve, 500))
    
    // Remove from local list
    const index = files.value.findIndex(f => f.id === file.id)
    if (index > -1) {
      files.value.splice(index, 1)
      totalFiles.value--
    }
    
    // Remove from selection
    const selectionIndex = selectedFiles.value.indexOf(file.id)
    if (selectionIndex > -1) {
      selectedFiles.value.splice(selectionIndex, 1)
    }
  } catch (error) {
    console.error('Failed to delete file:', error)
  }
}

const bulkDelete = async () => {
  if (!confirm(`确定要删除选中的 ${selectedFiles.value.length} 个文件吗？此操作不可恢复！`)) return
  
  try {
    // Mock bulk delete API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Remove from local list
    files.value = files.value.filter(file => !selectedFiles.value.includes(file.id))
    totalFiles.value = files.value.length
    
    clearSelection()
  } catch (error) {
    console.error('Failed to delete files:', error)
  }
}

// Upload handlers
const handleUploadSuccess = (urls: string[]) => {
  showUploadModal.value = false
  loadFiles() // Refresh file list
  
  const { success } = useGlobalNotifications()
  success(`成功上传 ${urls.length} 个文件`)
}

const handleUploadError = (error: string) => {
  const { error: showError } = useGlobalNotifications()
  showError('上传失败', error)
}

// Utility functions
const isImage = (file: FileItem) => {
  return file.mimeType.startsWith('image/')
}

const getFileType = (mimeType: string) => {
  if (mimeType.startsWith('image/')) return '图片'
  if (mimeType.startsWith('video/')) return '视频'
  if (mimeType.startsWith('audio/')) return '音频'
  if (mimeType.includes('pdf')) return 'PDF'
  if (mimeType.includes('word') || mimeType.includes('document')) return '文档'
  if (mimeType.includes('spreadsheet') || mimeType.includes('excel')) return '表格'
  return '其他'
}

const getTypeColorClass = (mimeType: string) => {
  const type = getFileType(mimeType)
  const colorMap: Record<string, string> = {
    '图片': 'bg-green-100 text-green-800',
    '视频': 'bg-purple-100 text-purple-800',
    '音频': 'bg-blue-100 text-blue-800',
    'PDF': 'bg-red-100 text-red-800',
    '文档': 'bg-yellow-100 text-yellow-800',
    '表格': 'bg-indigo-100 text-indigo-800',
    '其他': 'bg-gray-100 text-gray-800'
  }
  return colorMap[type] || 'bg-gray-100 text-gray-800'
}

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return `${parseFloat((bytes / k ** i).toFixed(2))} ${sizes[i]}`
}

const formatDate = (dateString: string) => {
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
  loadFiles()
})
</script>

<style scoped>
.file-card {
  @apply relative bg-white border border-gray-200 rounded-lg p-4 cursor-pointer hover:shadow-md transition-shadow duration-200;
}

.file-card-selected {
  @apply border-blue-500 bg-blue-50;
}

.file-preview {
  @apply w-full h-24 mb-3 flex items-center justify-center bg-gray-50 rounded;
}

.file-image {
  @apply w-full h-full object-cover rounded;
}

.file-icon {
  @apply h-12 w-12 text-gray-400;
}

.file-info {
  @apply mb-2;
}

.file-name {
  @apply text-sm font-medium text-gray-900 truncate;
}

.file-size {
  @apply text-xs text-gray-500;
}

.file-actions {
  @apply flex justify-end space-x-1;
}

.action-button {
  @apply p-1 text-gray-400 hover:text-gray-600;
}
</style>
