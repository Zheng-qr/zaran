<template>
  <div class="shop-page">
    <div class="container">
      <h1 class="page-title">文创商城</h1>
      <p class="page-description">精选扎染文创产品，传承文化之美</p>
      
      <!-- 商品分类 -->
      <div class="category-tabs">
        <button
          v-for="category in categories"
          :key="category.id"
          :class="['category-tab', { active: activeCategory === category.id }]"
          @click="setActiveCategory(category.id)"
        >
          <i :class="category.icon"></i>
          {{ category.name }}
        </button>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>加载商品中...</p>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="error-state">
        <i class="fas fa-exclamation-triangle"></i>
        <p>{{ error }}</p>
        <button @click="loadGoods" class="retry-btn">重试</button>
      </div>

      <!-- 商品列表 -->
      <div v-else-if="filteredGoods.length > 0" class="products-grid">
        <div class="product-card" v-for="product in filteredGoods" :key="product.id">
          <div class="product-image">
            <img :src="product.imageUrl || '/image/default-product.jpg'" :alt="product.name" loading="lazy">
            <div class="product-overlay">
              <button class="quick-view-btn" @click="viewProduct(product.id)">
                <i class="fas fa-eye"></i>
                快速查看
              </button>
            </div>
          </div>
          <div class="product-info">
            <h3 class="product-name">{{ product.name }}</h3>
            <p class="product-description">{{ product.publisherName || '扎染文创' }}</p>

            <!-- 版权信息 -->
            <div v-if="product.copyrightInfo" class="product-copyright">
              <i class="fas fa-copyright"></i>
              <span>{{ product.copyrightInfo }}</span>
            </div>

            <div class="product-price">
              <span class="current-price">{{ formatPrice(product.discountedPrice || product.price) }}</span>
              <span v-if="product.discountedPrice && product.discountedPrice < product.price" class="original-price">{{ formatPrice(product.price) }}</span>
              <span v-if="getDiscountPercentage(product.price, product.discountedPrice || product.price) > 0" class="discount-badge">
                -{{ getDiscountPercentage(product.price, product.discountedPrice || product.price) }}%
              </span>
            </div>
            <div class="product-stock">
              <span v-if="product.stoke > 0" class="in-stock">库存：{{ product.stoke }}</span>
              <span v-else class="out-of-stock">缺货</span>
            </div>
            <div class="product-actions">
              <button class="add-to-cart-btn" :disabled="product.stoke === 0" @click="addToCart(product)">
                <i class="fas fa-shopping-cart"></i>
                {{ product.stoke > 0 ? '加入购物车' : '缺货' }}
              </button>
              <button class="buy-now-btn">
                立即购买
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- 购物车侧边栏 -->
      <div class="cart-sidebar" :class="{ open: cartOpen }">
        <div class="cart-header">
          <h3>购物车</h3>
          <button class="close-cart" @click="cartOpen = false">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="cart-items">
          <div v-if="cartItems.length === 0" class="empty-cart">
            <i class="fas fa-shopping-cart"></i>
            <p>购物车为空</p>
          </div>
          <div v-else>
            <div class="cart-item" v-for="item in cartItems" :key="item.id">
              <img :src="item.image" :alt="item.name" class="item-image">
              <div class="item-info">
                <h4 class="item-name">{{ item.name }}</h4>
                <p class="item-price">¥{{ item.price }}</p>
                <div class="quantity-controls">
                  <button @click="decreaseQuantity(item.id)">-</button>
                  <span>{{ item.quantity }}</span>
                  <button @click="increaseQuantity(item.id)">+</button>
                </div>
              </div>
              <button class="remove-item" @click="removeFromCart(item.id)">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
        <div class="cart-footer" v-if="cartItems.length > 0">
          <div class="cart-total">
            <span>总计: ¥{{ cartTotal }}</span>
          </div>
          <button class="checkout-btn">去结算</button>
        </div>
      </div>

      <!-- 购物车按钮 -->
      <button class="cart-toggle" @click="cartOpen = !cartOpen">
        <i class="fas fa-shopping-cart"></i>
        <span v-if="cartItemCount > 0" class="cart-count">{{ cartItemCount }}</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { GoodDetailResponse } from '~/api'

// 页面元数据
useHead({
  title: '文创商城 - 檐下风铃',
  meta: [
    { name: 'description', content: '精选扎染文创产品商城' }
  ]
})

// 使用商品 composable
const { goods, loading, error, totalPages, fetchGoods, formatPrice, getDiscountPercentage } = useGoods()

// 商品分类
const categories = ref([
  { id: 'all', name: '全部', icon: 'fas fa-th' },
  { id: 'copyright', name: '版权商品', icon: 'fas fa-copyright' },
  { id: 'physical', name: '实体商品', icon: 'fas fa-box' },
  { id: 'clothing', name: '服饰', icon: 'fas fa-tshirt' },
  { id: 'accessories', name: '配饰', icon: 'fas fa-gem' },
  { id: 'home', name: '家居', icon: 'fas fa-home' },
  { id: 'art', name: '艺术品', icon: 'fas fa-palette' }
]);

const activeCategory = ref('all');
const currentPage = ref(1);
const searchKeyword = ref('');

// 加载商品数据
const loadGoods = () => {
  fetchGoods(currentPage.value, 12, searchKeyword.value);
};

// 查看商品详情
const viewProduct = (productId: string) => {
  navigateTo(`/shop/${productId}`);
};

// 购物车
const cartOpen = ref(false);
const cartItems = ref([]);

// 计算属性
const cartItemCount = computed(() => {
  return cartItems.value.reduce((total, item) => total + item.quantity, 0);
});

const cartTotal = computed(() => {
  return cartItems.value.reduce((total, item) => total + (item.price * item.quantity), 0);
});

// 方法
const setActiveCategory = (categoryId: string) => {
  activeCategory.value = categoryId;
  loadGoods(); // 重新加载商品
};

// 根据分类过滤商品
const filteredGoods = computed(() => {
  if (activeCategory.value === 'all') {
    return goods.value;
  } else if (activeCategory.value === 'copyright') {
    return goods.value.filter(good => good.copyrightInfo && good.copyrightInfo.trim() !== '');
  } else if (activeCategory.value === 'physical') {
    return goods.value.filter(good => !good.copyrightInfo || good.copyrightInfo.trim() === '');
  }
  // 其他分类可以根据需要添加更多过滤逻辑
  return goods.value;
});

const addToCart = (product: GoodDetailResponse) => {
  const existingItem = cartItems.value.find(item => item.id === product.id);
  if (existingItem) {
    existingItem.quantity++;
  } else {
    cartItems.value.push({ ...product, quantity: 1 });
  }
};

const removeFromCart = (productId: number) => {
  const index = cartItems.value.findIndex(item => item.id === productId);
  if (index > -1) {
    cartItems.value.splice(index, 1);
  }
};

const increaseQuantity = (productId: number) => {
  const item = cartItems.value.find(item => item.id === productId);
  if (item) {
    item.quantity++;
  }
};

const decreaseQuantity = (productId: number) => {
  const item = cartItems.value.find(item => item.id === productId);
  if (item && item.quantity > 1) {
    item.quantity--;
  }
};

// 页面加载时获取商品
onMounted(() => {
  loadGoods();
});
</script>

<style scoped>
/* 商城页面主容器 */
.shop-page {
  padding: 2rem 0;
  min-height: 100vh;
}

/* 分类标签 */
.category-tabs {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.category-tab {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: white;
  border: 2px solid var(--light-color);
  border-radius: var(--border-radius);
  color: var(--dark-gray-color);
  cursor: pointer;
  transition: all 0.3s ease;
  font-weight: 500;
}

.category-tab:hover {
  border-color: var(--primary-color);
  color: var(--primary-color);
  transform: translateY(-2px);
}

.category-tab.active {
  background: var(--primary-color);
  border-color: var(--primary-color);
  color: white;
}

/* 商品网格 */
.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

/* 商品卡片 */
.product-card {
  background: white;
  border-radius: var(--border-radius);
  overflow: hidden;
  box-shadow: var(--box-shadow);
  transition: all 0.3s ease;
  position: relative;
}

.product-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

/* 商品图片 */
.product-image {
  position: relative;
  width: 100%;
  height: 220px;
  overflow: hidden;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.product-card:hover .product-image img {
  transform: scale(1.05);
}

.product-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.product-card:hover .product-overlay {
  opacity: 1;
}

.quick-view-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: white;
  color: var(--dark-color);
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  font-weight: 500;
  transition: all 0.3s ease;
}

.quick-view-btn:hover {
  background: var(--primary-color);
  color: white;
}

/* 商品信息 */
.product-info {
  padding: 1.5rem;
}

.product-name {
  font-size: 1.1rem;
  font-weight: 600;
  color: var(--dark-color);
  margin-bottom: 0.5rem;
  line-height: 1.4;
}

.product-description {
  color: var(--medium-gray-color);
  font-size: 0.9rem;
  margin-bottom: 0.75rem;
}

/* 版权信息 */
.product-copyright {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
  padding: 0.5rem;
  background: linear-gradient(135deg, #f8f9fa, #e9ecef);
  border-left: 3px solid var(--primary-color);
  border-radius: 4px;
  font-size: 0.85rem;
  color: var(--dark-gray-color);
}

.product-copyright i {
  color: var(--primary-color);
  font-size: 0.9rem;
}

/* 价格区域 */
.product-price {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.current-price {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--accent-color-2);
}

.original-price {
  font-size: 1rem;
  color: var(--medium-gray-color);
  text-decoration: line-through;
}

.discount-badge {
  background: var(--accent-color-2);
  color: white;
  font-size: 0.75rem;
  padding: 0.25rem 0.5rem;
  border-radius: var(--border-radius);
  font-weight: 600;
}

/* 库存信息 */
.product-stock {
  margin-bottom: 1rem;
  font-size: 0.9rem;
}

.in-stock {
  color: var(--success-color);
  font-weight: 500;
}

.out-of-stock {
  color: var(--danger-color);
  font-weight: 500;
}

/* 商品操作按钮 */
.product-actions {
  display: flex;
  gap: 0.75rem;
}

.add-to-cart-btn,
.buy-now-btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem;
  border: none;
  border-radius: var(--border-radius);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.add-to-cart-btn {
  background: var(--secondary-color);
  color: white;
}

.add-to-cart-btn:hover:not(:disabled) {
  background: #008a7a;
  transform: translateY(-1px);
}

.add-to-cart-btn:disabled {
  background: var(--medium-gray-color);
  cursor: not-allowed;
  transform: none;
}

.buy-now-btn {
  background: var(--primary-color);
  color: white;
}

.buy-now-btn:hover {
  background: #065bb5;
  transform: translateY(-1px);
}

/* 加载和错误状态 */
.loading-state {
  text-align: center;
  padding: 4rem 0;
}

.spinner {
  display: inline-block;
  width: 40px;
  height: 40px;
  border: 4px solid var(--light-color);
  border-radius: 50%;
  border-top-color: var(--primary-color);
  animation: spin 1s ease-in-out infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.error-state {
  text-align: center;
  padding: 4rem 0;
  color: var(--danger-color);
}

.retry-btn {
  background: var(--primary-color);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: var(--border-radius);
  cursor: pointer;
  margin-top: 1rem;
  font-weight: 500;
  transition: all 0.3s ease;
}

.retry-btn:hover {
  background: #065bb5;
  transform: translateY(-1px);
}

/* 购物车侧边栏 */
.cart-sidebar {
  position: fixed;
  top: 0;
  right: -400px;
  width: 400px;
  height: 100vh;
  background: white;
  box-shadow: -4px 0 20px rgba(0, 0, 0, 0.1);
  transition: right 0.3s ease;
  z-index: 1000;
  display: flex;
  flex-direction: column;
}

.cart-sidebar.open {
  right: 0;
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid var(--light-color);
}

.cart-header h3 {
  margin: 0;
  color: var(--dark-color);
}

.close-cart {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: var(--medium-gray-color);
  cursor: pointer;
  padding: 0.5rem;
  border-radius: var(--border-radius);
  transition: all 0.3s ease;
}

.close-cart:hover {
  background: var(--light-color);
  color: var(--dark-color);
}

.cart-items {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
}

.empty-cart {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--medium-gray-color);
}

.empty-cart i {
  font-size: 3rem;
  margin-bottom: 1rem;
  display: block;
}

.cart-item {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  border-bottom: 1px solid var(--light-color);
}

.item-image {
  width: 60px;
  height: 60px;
  object-fit: cover;
  border-radius: var(--border-radius);
}

.item-info {
  flex: 1;
}

.item-name {
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: var(--dark-color);
}

.item-price {
  color: var(--accent-color-2);
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.quantity-controls button {
  width: 30px;
  height: 30px;
  border: 1px solid var(--light-color);
  background: white;
  border-radius: var(--border-radius);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.quantity-controls button:hover {
  background: var(--primary-color);
  color: white;
  border-color: var(--primary-color);
}

.remove-item {
  background: none;
  border: none;
  color: var(--danger-color);
  cursor: pointer;
  padding: 0.5rem;
  border-radius: var(--border-radius);
  transition: all 0.3s ease;
}

.remove-item:hover {
  background: var(--danger-color);
  color: white;
}

.cart-footer {
  padding: 1.5rem;
  border-top: 1px solid var(--light-color);
}

.cart-total {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--dark-color);
  margin-bottom: 1rem;
  text-align: center;
}

.checkout-btn {
  width: 100%;
  padding: 1rem;
  background: var(--primary-color);
  color: white;
  border: none;
  border-radius: var(--border-radius);
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.checkout-btn:hover {
  background: #065bb5;
  transform: translateY(-1px);
}

/* 购物车切换按钮 */
.cart-toggle {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  width: 60px;
  height: 60px;
  background: var(--primary-color);
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 1.5rem;
  cursor: pointer;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
  transition: all 0.3s ease;
  z-index: 999;
}

.cart-toggle:hover {
  background: #065bb5;
  transform: scale(1.1);
}

.cart-count {
  position: absolute;
  top: -8px;
  right: -8px;
  background: var(--accent-color-2);
  color: white;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 600;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .shop-page {
    padding: 1rem 0;
  }

  .category-tabs {
    gap: 0.5rem;
    margin-bottom: 1.5rem;
  }

  .category-tab {
    padding: 0.5rem 1rem;
    font-size: 0.9rem;
  }

  .products-grid {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 1.5rem;
  }

  .product-info {
    padding: 1rem;
  }

  .product-actions {
    flex-direction: column;
    gap: 0.5rem;
  }

  .cart-sidebar {
    width: 100vw;
    right: -100vw;
  }

  .cart-toggle {
    bottom: 1rem;
    right: 1rem;
    width: 50px;
    height: 50px;
    font-size: 1.25rem;
  }
}

@media (max-width: 480px) {
  .products-grid {
    grid-template-columns: 1fr;
  }

  .category-tabs {
    justify-content: flex-start;
    overflow-x: auto;
    padding-bottom: 0.5rem;
  }

  .category-tab {
    flex-shrink: 0;
  }
}
</style>
