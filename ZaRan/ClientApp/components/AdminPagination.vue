<template>
  <nav class="flex items-center justify-between">
    <!-- Results info -->
    <div class="flex-1 flex justify-between sm:hidden">
      <button
        @click="goToPrevious"
        :disabled="currentPage <= 1"
        class="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
      >
        上一页
      </button>
      <button
        @click="goToNext"
        :disabled="currentPage >= totalPages"
        class="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
      >
        下一页
      </button>
    </div>
    
    <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
      <div>
        <p class="text-sm text-gray-700">
          显示第
          <span class="font-medium">{{ startItem }}</span>
          到
          <span class="font-medium">{{ endItem }}</span>
          项，共
          <span class="font-medium">{{ totalItems }}</span>
          项结果
        </p>
      </div>
      
      <div>
        <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
          <!-- Previous button -->
          <button
            @click="goToPrevious"
            :disabled="currentPage <= 1"
            class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span class="sr-only">上一页</span>
            <svg class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M12.707 5.293a1 1 0 010 1.414L9.414 10l3.293 3.293a1 1 0 01-1.414 1.414l-4-4a1 1 0 010-1.414l4-4a1 1 0 011.414 0z" clip-rule="evenodd" />
            </svg>
          </button>
          
          <!-- Page numbers -->
          <template v-for="page in visiblePages" :key="page">
            <button
              v-if="page !== '...'"
              @click="goToPage(page as number)"
              :class="[
                'relative inline-flex items-center px-4 py-2 border text-sm font-medium',
                page === currentPage
                  ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                  : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
              ]"
            >
              {{ page }}
            </button>
            <span
              v-else
              class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700"
            >
              ...
            </span>
          </template>
          
          <!-- Next button -->
          <button
            @click="goToNext"
            :disabled="currentPage >= totalPages"
            class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span class="sr-only">下一页</span>
            <svg class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
            </svg>
          </button>
        </nav>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
interface Props {
  currentPage: number
  totalItems: number
  pageSize: number
  maxVisiblePages?: number
}

const props = withDefaults(defineProps<Props>(), {
  maxVisiblePages: 7
})

const emit = defineEmits<{
  pageChange: [page: number]
}>()

// Computed properties
const totalPages = computed(() => Math.ceil(props.totalItems / props.pageSize))

const startItem = computed(() => {
  if (props.totalItems === 0) return 0
  return (props.currentPage - 1) * props.pageSize + 1
})

const endItem = computed(() => {
  const end = props.currentPage * props.pageSize
  return Math.min(end, props.totalItems)
})

const visiblePages = computed(() => {
  const pages: (number | string)[] = []
  const total = totalPages.value
  const current = props.currentPage
  const max = props.maxVisiblePages

  if (total <= max) {
    // Show all pages if total is less than max
    for (let i = 1; i <= total; i++) {
      pages.push(i)
    }
  } else {
    // Always show first page
    pages.push(1)

    // Calculate start and end of middle section
    const start = Math.max(2, current - Math.floor((max - 4) / 2))
    const end = Math.min(total - 1, start + max - 4)

    // Add ellipsis if needed before middle section
    if (start > 2) {
      pages.push('...')
    }

    // Add middle section
    for (let i = start; i <= end; i++) {
      pages.push(i)
    }

    // Add ellipsis if needed after middle section
    if (end < total - 1) {
      pages.push('...')
    }

    // Always show last page if more than 1 page
    if (total > 1) {
      pages.push(total)
    }
  }

  return pages
})

// Navigation methods
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value && page !== props.currentPage) {
    emit('pageChange', page)
  }
}

const goToPrevious = () => {
  if (props.currentPage > 1) {
    goToPage(props.currentPage - 1)
  }
}

const goToNext = () => {
  if (props.currentPage < totalPages.value) {
    goToPage(props.currentPage + 1)
  }
}
</script>
