<template>
  <div class="file-upload">
    <!-- Drop zone -->
    <div
      ref="dropZone"
      class="drop-zone"
      :class="{
        'drop-zone-active': isDragOver,
        'drop-zone-error': hasError,
        'drop-zone-disabled': disabled
      }"
      @click="openFileDialog"
      @dragover.prevent="handleDragOver"
      @dragleave.prevent="handleDragLeave"
      @drop.prevent="handleDrop"
    >
      <input
        ref="fileInput"
        type="file"
        :accept="accept"
        :multiple="multiple"
        class="hidden"
        @change="handleFileSelect"
      >

      <!-- Upload content -->
      <div v-if="!uploading && files.length === 0" class="upload-content">
        <svg class="upload-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
        </svg>
        <p class="upload-text">
          <span class="upload-link">点击上传</span>
          或拖拽文件到此处
        </p>
        <p class="upload-hint">
          {{ hint || `支持 ${accept || '*'} 格式，最大 ${formatFileSize(maxSize)}` }}
        </p>
      </div>

      <!-- Upload progress -->
      <div v-else-if="uploading" class="upload-progress">
        <svg class="animate-spin upload-spinner" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <p class="upload-text">上传中... {{ uploadProgress }}%</p>
        <div class="progress-bar">
          <div class="progress-fill" :style="{ width: `${uploadProgress}%` }"></div>
        </div>
      </div>

      <!-- File preview -->
      <div v-else class="file-preview">
        <div v-for="(file, index) in files" :key="index" class="file-item">
          <div class="file-info">
            <img 
              v-if="isImage(file) && file.preview" 
              :src="file.preview" 
              :alt="file.name"
              class="file-thumbnail"
            >
            <div v-else class="file-icon">
              <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <div class="file-details">
              <p class="file-name">{{ file.name }}</p>
              <p class="file-size">{{ formatFileSize(file.size) }}</p>
            </div>
          </div>
          <button
            @click.stop="removeFile(index)"
            class="file-remove"
            :disabled="uploading"
          >
            <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Error message -->
    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <!-- Upload button -->
    <div v-if="files.length > 0 && !uploading && !autoUpload" class="upload-actions">
      <button
        @click="uploadFiles"
        :disabled="uploading"
        class="upload-button"
      >
        上传文件
      </button>
      <button
        @click="clearFiles"
        :disabled="uploading"
        class="clear-button"
      >
        清空
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
interface FileWithPreview extends File {
  preview?: string
}

interface Props {
  accept?: string
  multiple?: boolean
  maxSize?: number // in bytes
  maxFiles?: number
  autoUpload?: boolean
  disabled?: boolean
  hint?: string
}

const props = withDefaults(defineProps<Props>(), {
  accept: 'image/*',
  multiple: false,
  maxSize: 10 * 1024 * 1024, // 10MB
  maxFiles: 1,
  autoUpload: true,
  disabled: false
})

const emit = defineEmits<{
  upload: [files: File[]]
  success: [urls: string[]]
  error: [error: string]
  change: [files: File[]]
}>()

// Refs
const dropZone = ref<HTMLElement>()
const fileInput = ref<HTMLInputElement>()

// State
const files = ref<FileWithPreview[]>([])
const isDragOver = ref(false)
const uploading = ref(false)
const uploadProgress = ref(0)
const error = ref('')
const hasError = ref(false)

// File handling
const openFileDialog = () => {
  if (props.disabled || uploading.value) return
  fileInput.value?.click()
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files) {
    addFiles(Array.from(target.files))
  }
}

const handleDragOver = (event: DragEvent) => {
  if (props.disabled || uploading.value) return
  isDragOver.value = true
}

const handleDragLeave = () => {
  isDragOver.value = false
}

const handleDrop = (event: DragEvent) => {
  if (props.disabled || uploading.value) return
  isDragOver.value = false
  
  const droppedFiles = Array.from(event.dataTransfer?.files || [])
  addFiles(droppedFiles)
}

const addFiles = async (newFiles: File[]) => {
  error.value = ''
  hasError.value = false

  // Validate file count
  const totalFiles = files.value.length + newFiles.length
  if (totalFiles > props.maxFiles) {
    error.value = `最多只能上传 ${props.maxFiles} 个文件`
    hasError.value = true
    return
  }

  // Validate and process files
  const validFiles: FileWithPreview[] = []
  
  for (const file of newFiles) {
    // Validate file size
    if (file.size > props.maxSize) {
      error.value = `文件 ${file.name} 超过最大大小限制 ${formatFileSize(props.maxSize)}`
      hasError.value = true
      continue
    }

    // Validate file type
    if (props.accept && props.accept !== '*') {
      const acceptedTypes = props.accept.split(',').map(type => type.trim())
      const isValidType = acceptedTypes.some(type => {
        if (type.startsWith('.')) {
          return file.name.toLowerCase().endsWith(type.toLowerCase())
        }
        return file.type.match(type.replace('*', '.*'))
      })
      
      if (!isValidType) {
        error.value = `文件 ${file.name} 格式不支持`
        hasError.value = true
        continue
      }
    }

    // Create preview for images
    const fileWithPreview: FileWithPreview = file
    if (isImage(file)) {
      fileWithPreview.preview = URL.createObjectURL(file)
    }

    validFiles.push(fileWithPreview)
  }

  // Add valid files
  if (props.multiple) {
    files.value.push(...validFiles)
  } else {
    // Clean up old previews
    files.value.forEach(file => {
      if (file.preview) {
        URL.revokeObjectURL(file.preview)
      }
    })
    files.value = validFiles
  }

  emit('change', files.value)

  // Auto upload if enabled
  if (props.autoUpload && validFiles.length > 0) {
    await uploadFiles()
  }
}

const removeFile = (index: number) => {
  const file = files.value[index]
  if (file.preview) {
    URL.revokeObjectURL(file.preview)
  }
  files.value.splice(index, 1)
  emit('change', files.value)
}

const clearFiles = () => {
  files.value.forEach(file => {
    if (file.preview) {
      URL.revokeObjectURL(file.preview)
    }
  })
  files.value = []
  emit('change', files.value)
}

const uploadFiles = async () => {
  if (files.value.length === 0) return

  uploading.value = true
  uploadProgress.value = 0
  error.value = ''
  hasError.value = false

  try {
    emit('upload', files.value)

    // Use the real upload API
    const { uploadFile } = useApi()
    const uploadPromises = files.value.map(async (file, index) => {
      const result = await uploadFile(file)
      if (result.error) {
        throw new Error(result.error)
      }

      // Update progress
      uploadProgress.value = Math.round(((index + 1) / files.value.length) * 100)

      return result.data
    })

    const urls = await Promise.all(uploadPromises)
    emit('success', urls.filter(Boolean) as string[])

    // Clear files after successful upload
    clearFiles()
  } catch (err) {
    const errorMessage = err instanceof Error ? err.message : '上传失败'
    error.value = errorMessage
    hasError.value = true
    emit('error', errorMessage)
  } finally {
    uploading.value = false
    uploadProgress.value = 0
  }
}

// Utility functions
const isImage = (file: File) => {
  return file.type.startsWith('image/')
}

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return `${parseFloat((bytes / k ** i).toFixed(2))} ${sizes[i]}`
}

// Cleanup on unmount
onUnmounted(() => {
  files.value.forEach(file => {
    if (file.preview) {
      URL.revokeObjectURL(file.preview)
    }
  })
})
</script>

<style scoped>
.file-upload {
  @apply w-full;
}

.drop-zone {
  @apply border-2 border-dashed border-gray-300 rounded-lg p-6 text-center cursor-pointer transition-colors duration-200 hover:border-gray-400;
}

.drop-zone-active {
  @apply border-blue-500 bg-blue-50;
}

.drop-zone-error {
  @apply border-red-500 bg-red-50;
}

.drop-zone-disabled {
  @apply cursor-not-allowed opacity-50;
}

.upload-content {
  @apply py-8;
}

.upload-icon {
  @apply mx-auto h-12 w-12 text-gray-400;
}

.upload-text {
  @apply mt-2 text-sm text-gray-600;
}

.upload-link {
  @apply font-medium text-blue-600 hover:text-blue-500;
}

.upload-hint {
  @apply mt-1 text-xs text-gray-500;
}

.upload-progress {
  @apply py-8;
}

.upload-spinner {
  @apply mx-auto h-8 w-8 text-blue-600;
}

.progress-bar {
  @apply mt-4 w-full bg-gray-200 rounded-full h-2;
}

.progress-fill {
  @apply bg-blue-600 h-2 rounded-full transition-all duration-300;
}

.file-preview {
  @apply space-y-3;
}

.file-item {
  @apply flex items-center justify-between p-3 bg-gray-50 rounded-lg;
}

.file-info {
  @apply flex items-center space-x-3 flex-1;
}

.file-thumbnail {
  @apply h-12 w-12 object-cover rounded;
}

.file-icon {
  @apply h-12 w-12 text-gray-400 flex items-center justify-center;
}

.file-details {
  @apply flex-1 min-w-0;
}

.file-name {
  @apply text-sm font-medium text-gray-900 truncate;
}

.file-size {
  @apply text-xs text-gray-500;
}

.file-remove {
  @apply p-1 text-gray-400 hover:text-red-500 disabled:opacity-50;
}

.error-message {
  @apply mt-2 text-sm text-red-600;
}

.upload-actions {
  @apply mt-4 flex space-x-3;
}

.upload-button {
  @apply px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700 disabled:opacity-50;
}

.clear-button {
  @apply px-4 py-2 bg-gray-300 text-gray-700 text-sm font-medium rounded-md hover:bg-gray-400 disabled:opacity-50;
}
</style>
