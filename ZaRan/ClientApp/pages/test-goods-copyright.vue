<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-3xl font-bold mb-8">商品版权信息测试页面</h1>
    
    <!-- 测试商品创建表单 -->
    <div class="bg-white shadow rounded-lg p-6 mb-8">
      <h2 class="text-xl font-semibold mb-4">创建测试商品</h2>
      <form @submit.prevent="createTestGood" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700">商品名称</label>
          <input
            v-model="testGood.name"
            type="text"
            required
            class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            placeholder="请输入商品名称"
          >
        </div>
        
        <div>
          <label class="block text-sm font-medium text-gray-700">价格</label>
          <input
            v-model.number="testGood.price"
            type="number"
            step="0.01"
            min="0"
            required
            class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            placeholder="0.00"
          >
        </div>
        
        <div>
          <label class="block text-sm font-medium text-gray-700">版权信息</label>
          <textarea
            v-model="testGood.copyrightInfo"
            rows="3"
            class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
            placeholder="请输入版权信息，如：版权所有、使用权限、授权范围等"
          />
          <p class="mt-1 text-sm text-gray-500">
            填写版权信息将标记为版权商品，留空则为实体商品
          </p>
        </div>
        
        <button
          type="submit"
          :disabled="loading"
          class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50"
        >
          <svg v-if="loading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          {{ loading ? '创建中...' : '创建测试商品' }}
        </button>
      </form>
    </div>
    
    <!-- 商品分类过滤 -->
    <div class="bg-white shadow rounded-lg p-6 mb-8">
      <h2 class="text-xl font-semibold mb-4">商品分类过滤测试</h2>
      <div class="flex space-x-4 mb-4">
        <button
          v-for="category in categories"
          :key="category.id"
          :class="[
            'px-4 py-2 rounded-md text-sm font-medium',
            activeCategory === category.id
              ? 'bg-blue-600 text-white'
              : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
          ]"
          @click="setActiveCategory(category.id)"
        >
          <i :class="category.icon" class="mr-2"></i>
          {{ category.name }}
        </button>
      </div>
      
      <div class="text-sm text-gray-600">
        当前分类：{{ getCurrentCategoryName() }} | 
        显示商品数量：{{ filteredGoods.length }}
      </div>
    </div>
    
    <!-- 商品列表 -->
    <div class="bg-white shadow rounded-lg p-6">
      <h2 class="text-xl font-semibold mb-4">商品列表</h2>
      
      <div v-if="filteredGoods.length === 0" class="text-center py-8 text-gray-500">
        暂无商品
      </div>
      
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="product in filteredGoods"
          :key="product.id"
          class="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow"
        >
          <h3 class="font-semibold text-lg mb-2">{{ product.name }}</h3>
          <p class="text-gray-600 mb-2">价格：¥{{ product.price?.toFixed(2) || '0.00' }}</p>
          
          <!-- 版权信息显示 -->
          <div v-if="product.copyrightInfo" class="mb-2">
            <div class="flex items-start space-x-2 p-2 bg-blue-50 border-l-4 border-blue-400 rounded">
              <i class="fas fa-copyright text-blue-600 mt-0.5"></i>
              <div>
                <p class="text-sm font-medium text-blue-800">版权商品</p>
                <p class="text-xs text-blue-600">{{ product.copyrightInfo }}</p>
              </div>
            </div>
          </div>
          
          <div v-else class="mb-2">
            <div class="flex items-center space-x-2 p-2 bg-green-50 border-l-4 border-green-400 rounded">
              <i class="fas fa-box text-green-600"></i>
              <p class="text-sm font-medium text-green-800">实体商品</p>
            </div>
          </div>
          
          <div class="text-xs text-gray-500">
            发布者：{{ product.publisherName || '未知' }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { GoodDetailResponse, GoodPostEndpointRequest } from '~/api'

// 页面元数据
useHead({
  title: '商品版权信息测试 - 檐下风铃',
  meta: [
    { name: 'description', content: '测试商品版权信息功能' }
  ]
})

// 使用商品 composable
const { goods, loading, fetchGoods, createGood } = useGoods()

// 测试商品表单
const testGood = ref<GoodPostEndpointRequest>({
  name: '',
  price: 0,
  stoke: 10,
  discountedPrice: 0,
  publisherName: '',
  imageUrl: '',
  copyrightInfo: '',
  relatedArticleId: ''
})

// 商品分类
const categories = ref([
  { id: 'all', name: '全部', icon: 'fas fa-th' },
  { id: 'copyright', name: '版权商品', icon: 'fas fa-copyright' },
  { id: 'physical', name: '实体商品', icon: 'fas fa-box' }
])

const activeCategory = ref('all')

// 创建测试商品
const createTestGood = async () => {
  try {
    await createGood(testGood.value)
    
    // 重置表单
    testGood.value = {
      name: '',
      price: 0,
      stoke: 10,
      discountedPrice: 0,
      publisherName: '',
      imageUrl: '',
      copyrightInfo: '',
      relatedArticleId: ''
    }
    
    // 重新加载商品列表
    await fetchGoods()
    
    alert('测试商品创建成功！')
  } catch (error) {
    console.error('创建测试商品失败:', error)
    alert('创建测试商品失败，请检查控制台错误信息')
  }
}

// 设置活动分类
const setActiveCategory = (categoryId: string) => {
  activeCategory.value = categoryId
}

// 获取当前分类名称
const getCurrentCategoryName = () => {
  const category = categories.value.find(c => c.id === activeCategory.value)
  return category?.name || '未知'
}

// 根据分类过滤商品
const filteredGoods = computed(() => {
  if (activeCategory.value === 'all') {
    return goods.value
  } else if (activeCategory.value === 'copyright') {
    return goods.value.filter(good => good.copyrightInfo && good.copyrightInfo.trim() !== '')
  } else if (activeCategory.value === 'physical') {
    return goods.value.filter(good => !good.copyrightInfo || good.copyrightInfo.trim() === '')
  }
  return goods.value
})

// 页面加载时获取商品
onMounted(() => {
  fetchGoods()
})
</script>

<style scoped>
.container {
  max-width: 1200px;
}
</style>
