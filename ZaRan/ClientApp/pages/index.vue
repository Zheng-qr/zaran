<template>
  <div>
    <!-- 轮播Banner区域 -->
    <section class="carousel-banner">
      <div class="carousel-slides" :style="{ transform: `translateX(-${currentIndex * 100}%)` }">
        <!-- 幻灯片循环 -->
        <div
          v-for="(slide, index) in slides"
          :key="index"
          class="carousel-slide"
          :style="{ backgroundImage: `url('${slide.image}')` }"
        >
          <div class="slide-content">
            <h1>{{ slide.title }}</h1>
            <p>{{ slide.description }}</p>
            <NuxtLink :to="slide.link" class="btn-slide">{{ slide.buttonText }}</NuxtLink>
          </div>
        </div>
      </div>
      <button class="carousel-prev" aria-label="上一张" @click="prevSlide"><i class="fas fa-chevron-left"></i></button>
      <button class="carousel-next" aria-label="下一张" @click="nextSlide"><i class="fas fa-chevron-right"></i></button>
      <div class="carousel-dots">
        <button
          v-for="(_, index) in slides"
          :key="index"
          class="carousel-dot"
          :class="{ 'active': index === currentIndex }"
          :aria-label="`跳转到幻灯片 ${index + 1}`"
          @click="goToSlide(index)"
        ></button>
      </div>
    </section>

    <section class="quick-nav">
      <div class="container">
        <h2>{{ quickNavTitle }}</h2>
        <div class="quick-nav-grid">
          <NuxtLink
            v-for="(item, index) in quickNavItems"
            :key="index"
            :to="item.link"
            class="quick-nav-item"
          >
            <i :class="item.icon"></i>
            <span>{{ item.text }}</span>
          </NuxtLink>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
// 页面元数据
useHead({
  title: '檐下风铃 - 扎染文化数字平台',
  meta: [
    { name: 'description', content: '探索传统扎染工艺的数字化平台' }
  ]
})

// 轮播图数据
const slides = ref([
  {
    image: '/image/IMG_8828.JPG',
    title: '探索扎染的无尽魅力',
    description: '千年传承的指尖艺术，绽放现代生活的新色彩。',
    link: '/knowledge',
    buttonText: '了解更多'
  },
  {
    image: '/image/IMG_8838.JPG',
    title: '"檐下风铃"IP故事',
    description: '跟随主人公的脚步，体验一段关于守护与创新的传奇。',
    link: '/ip-story',
    buttonText: '阅读故事'
  },
  {
    image: '/image/IMG_9053.JPG',
    title: '加入创意工坊',
    description: '释放你的灵感，参与扎染纹样的共创与设计。',
    link: '/workshop',
    buttonText: '开始创作'
  }
]);

// 快速导航数据
const quickNavTitle = ref('快速导航');
const quickNavItems = ref([
  { link: '/ip-story', icon: 'fas fa-book-reader', text: 'IP故事' },
  { link: '/knowledge', icon: 'fas fa-microscope', text: '扎染工艺' },
  { link: '/pattern-library', icon: 'fas fa-dna', text: '基因库' },
  { link: '/workshop', icon: 'fas fa-paint-brush', text: '创意工坊' },
  { link: '/shop', icon: 'fas fa-shopping-bag', text: '文创商城' },
  { link: '/community', icon: 'fas fa-users', text: '共创社区' },
]);

// 轮播逻辑
const currentIndex = ref(0);
let autoPlayInterval: number | null = null;

function goToSlide(index: number) {
  currentIndex.value = (index + slides.value.length) % slides.value.length;
}

function nextSlide() {
  goToSlide(currentIndex.value + 1);
}

function prevSlide() {
  goToSlide(currentIndex.value - 1);
}

function startAutoPlay(interval = 5000) {
  autoPlayInterval = window.setInterval(nextSlide, interval);
}

function stopAutoPlay() {
  if (autoPlayInterval) {
    clearInterval(autoPlayInterval);
    autoPlayInterval = null;
  }
}

function resetAutoPlay(interval = 5000) {
  stopAutoPlay();
  startAutoPlay(interval);
}

onMounted(() => {
  startAutoPlay();
});

onBeforeUnmount(() => {
  stopAutoPlay();

  // 移除事件监听器
  const slidesContainer = document.querySelector('.carousel-slides');
  if (slidesContainer) {
    slidesContainer.removeEventListener('mouseenter', stopAutoPlay);
    slidesContainer.removeEventListener('mouseleave', resetAutoPlay);
  }
});
</script>

<style scoped>
/* 轮播Banner样式 */
.carousel-banner {
  position: relative;
  width: 100%;
  height: 65vh;
  min-height: 400px;
  overflow: hidden;
  background-color: var(--dark-gray-color);
}

.carousel-slides {
  display: flex;
  width: 100%;
  height: 100%;
  transition: transform 0.5s ease-in-out;
}

.carousel-slide {
  min-width: 100%;
  height: 100%;
  position: relative;
  background-size: cover;
  background-position: center;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  color: var(--text-light-color);
}

.carousel-slide::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.4);
  z-index: 1;
}

.slide-content {
  position: relative;
  z-index: 2;
  padding: 20px;
  max-width: 700px;
}

.slide-content h1 {
  font-size: 2.8rem;
  margin-bottom: 1rem;
  font-weight: 700;
  color: var(--text-light-color);
}

.slide-content p {
  font-size: 1.1rem;
  margin-bottom: 1.5rem;
  opacity: 0.9;
}

.slide-content .btn-slide {
  padding: 12px 25px;
  background-color: var(--accent-color-1);
  color: var(--dark-color);
  text-decoration: none;
  border-radius: var(--border-radius);
  font-weight: bold;
  transition: background-color 0.3s, transform 0.3s;
  display: inline-block;
}

.slide-content .btn-slide:hover {
  background-color: #e6a823;
  transform: translateY(-2px);
}

/* 导航按钮 */
.carousel-prev,
.carousel-next {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background-color: rgba(0, 0, 0, 0.5);
  color: white;
  border: none;
  padding: 15px;
  cursor: pointer;
  z-index: 10;
  font-size: 1.5rem;
  border-radius: var(--border-radius);
  transition: background-color 0.3s;
}

.carousel-prev:hover,
.carousel-next:hover {
  background-color: rgba(0, 0, 0, 0.8);
}

.carousel-prev {
  left: 15px;
}

.carousel-next {
  right: 15px;
}

/* 指示点 */
.carousel-dots {
  position: absolute;
  bottom: 20px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  z-index: 10;
}

.carousel-dot {
  width: 12px;
  height: 12px;
  background-color: rgba(255, 255, 255, 0.5);
  border-radius: 50%;
  margin: 0 5px;
  cursor: pointer;
  transition: background-color 0.3s, transform 0.3s;
  border: none;
}

.carousel-dot.active {
  background-color: var(--text-light-color);
  transform: scale(1.2);
}

/* 快速导航 */
.quick-nav {
  padding: 3rem 0;
}

.quick-nav h2 {
  text-align: center;
  margin-bottom: 2.5rem;
  font-size: 2rem;
  color: var(--dark-color);
}

.quick-nav-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 25px;
}

.quick-nav-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  text-decoration: none;
  color: var(--text-color);
  transition: transform 0.3s, box-shadow 0.3s;
  padding: 25px 15px;
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--box-shadow);
}

.quick-nav-item:hover {
  transform: translateY(-6px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
}

.quick-nav-item i {
  font-size: 2.2rem;
  margin-bottom: 12px;
  color: var(--primary-color);
  width: 60px;
  height: 60px;
  line-height: 60px;
  border-radius: 50%;
  background-color: #e7f0ff;
  transition: background-color 0.3s, color 0.3s;
}

.quick-nav-item:hover i {
  background-color: var(--primary-color);
  color: var(--text-light-color);
}

.quick-nav-item span {
  font-weight: 500;
  font-size: 0.95rem;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .carousel-banner {
    height: 50vh;
    min-height: 300px;
  }

  .slide-content h1 {
    font-size: 2rem;
  }

  .slide-content p {
    font-size: 1rem;
  }

  .quick-nav-grid {
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    gap: 15px;
  }

  .quick-nav-item {
    padding: 20px 10px;
  }

  .quick-nav-item i {
    font-size: 1.8rem;
    width: 50px;
    height: 50px;
    line-height: 50px;
  }
}
</style>
