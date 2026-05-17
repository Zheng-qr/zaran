<template>
  <nav class="flex" aria-label="Breadcrumb">
    <ol class="flex items-center space-x-4">
      <li>
        <div>
          <NuxtLink to="/admin" class="text-gray-400 hover:text-gray-500">
            <svg class="flex-shrink-0 h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
              <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 011 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z" />
            </svg>
            <span class="sr-only">首页</span>
          </NuxtLink>
        </div>
      </li>
      
      <li v-for="(breadcrumb, index) in breadcrumbs" :key="index">
        <div class="flex items-center">
          <svg class="flex-shrink-0 h-5 w-5 text-gray-300" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
          </svg>
          
          <NuxtLink
            v-if="breadcrumb.to && index < breadcrumbs.length - 1"
            :to="breadcrumb.to"
            class="ml-4 text-sm font-medium text-gray-500 hover:text-gray-700"
          >
            {{ breadcrumb.text }}
          </NuxtLink>
          
          <span
            v-else
            class="ml-4 text-sm font-medium text-gray-900"
            aria-current="page"
          >
            {{ breadcrumb.text }}
          </span>
        </div>
      </li>
    </ol>
  </nav>
</template>

<script setup lang="ts">
interface Breadcrumb {
  text: string
  to?: string
}

const route = useRoute()

const breadcrumbs = computed((): Breadcrumb[] => {
  const pathSegments = route.path.split('/').filter(Boolean)
  const crumbs: Breadcrumb[] = []
  
  // Skip 'admin' segment as it's the home icon
  const segments = pathSegments.slice(1)
  
  if (segments.length === 0) {
    return [{ text: '仪表板' }]
  }
  
  // Build breadcrumbs based on path segments
  let currentPath = '/admin'
  
  for (let i = 0; i < segments.length; i++) {
    const segment = segments[i]
    currentPath += `/${segment}`
    
    // Check if this is the last segment
    const isLast = i === segments.length - 1
    
    // Get breadcrumb text and link
    const breadcrumb = getBreadcrumbInfo(segment, currentPath, isLast, route.params)
    
    if (breadcrumb) {
      crumbs.push(breadcrumb)
    }
  }
  
  return crumbs
})

function getBreadcrumbInfo(segment: string, path: string, isLast: boolean, params: any): Breadcrumb | null {
  // Handle dynamic routes with IDs
  if (segment.match(/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i)) {
    // This is a UUID, show appropriate text based on parent
    const parentSegment = path.split('/').slice(-2, -1)[0]
    const actionMap: Record<string, string> = {
      'users': '用户详情',
      'articles': '文章详情',
      'goods': '商品详情',
      'collections': '集合详情',
      'transactions': '交易详情',
      'messages': '消息详情',
      'comments': '评论详情'
    }
    
    return {
      text: actionMap[parentSegment] || '详情',
      to: isLast ? undefined : path
    }
  }
  
  // Handle action segments
  if (segment === 'create') {
    return {
      text: '创建',
      to: isLast ? undefined : path
    }
  }
  
  if (segment === 'edit') {
    return {
      text: '编辑',
      to: isLast ? undefined : path
    }
  }
  
  // Handle main sections
  const sectionMap: Record<string, string> = {
    'users': '用户管理',
    'articles': '文章管理',
    'goods': '商品管理',
    'collections': '集合管理',
    'transactions': '交易管理',
    'collects': '收藏品管理',
    'messages': '消息管理',
    'comments': '评论管理',
    'files': '文件管理',
    'settings': '系统设置'
  }
  
  if (sectionMap[segment]) {
    return {
      text: sectionMap[segment],
      to: isLast ? undefined : path
    }
  }
  
  // Default case
  return {
    text: segment.charAt(0).toUpperCase() + segment.slice(1),
    to: isLast ? undefined : path
  }
}
</script>
