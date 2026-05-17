<template>
  <div class="article-detail-page" :style="pageBackgroundStyle">
    <div class="container">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>加载中...</p>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="error-state">
        <i class="fas fa-exclamation-triangle"></i>
        <p>{{ error }}</p>
        <button @click="loadArticle" class="retry-btn">重试</button>
      </div>

      <!-- 文章内容 -->
      <article v-else-if="article" class="article-detail">
        <!-- 文章头部 -->
        <header class="article-header">
          <div class="article-meta">
            <span class="article-date">{{ formatDate(article.createdAt) }}</span>
            <span class="article-author" v-if="article.author">
              作者：{{ article.author.nickname }}
            </span>
          </div>
          <h1 class="article-title">{{ article.title }}</h1>
          <p class="article-summary">{{ article.summary }}</p>
          <div class="article-tags" v-if="article.tags && article.tags.length > 0">
            <span v-for="tag in article.tags" :key="tag" class="tag">
              {{ tag }}
            </span>
          </div>
        </header>

        <!-- 文章图片 -->
        <div v-if="article.imageSmallUrl" class="article-image">
          <img :src="article.imageSmallUrl" :alt="article.title">
        </div>

        <!-- 文章正文 -->
        <div class="article-content">
          <div class="content-text markdown-content"
               v-html="formatContent(article.body)"
               :style="markdownStyle">
          </div>
        </div>

        <!-- 文章操作 -->
        <div class="article-actions">
          <LikeButton :target-id="articleId" />
          <button class="action-btn share-btn" @click="shareArticle">
            <i class="fas fa-share"></i>
            分享
          </button>
        </div>

        <!-- 相关文章 -->
        <section class="related-articles">
          <h3 class="section-title">相关文章</h3>
          <div class="related-grid">
            <div v-for="relatedArticle in relatedArticles" :key="relatedArticle.id" 
                 class="related-card" @click="goToArticle(relatedArticle.id)">
              <div class="related-image">
                <img :src="relatedArticle.imageSmallUrl" :alt="relatedArticle.title">
              </div>
              <div class="related-info">
                <h4 class="related-title">{{ relatedArticle.title }}</h4>
                <p class="related-summary">{{ relatedArticle.summary }}</p>
              </div>
            </div>
          </div>
        </section>

        <!-- 评论区 -->
        <CommentSection :target-id="articleId" />


      </article>

      <!-- 返回按钮 -->
      <div class="back-navigation">
        <button @click="goBack" class="back-btn">
          <i class="fas fa-arrow-left"></i>
          返回列表
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ArticleDetailResponse } from '~/api'
import { marked } from 'marked'

// 获取路由参数
const route = useRoute();
const articleId = route.params.id as string;

// 使用 composable
const { getArticleById } = useArticles();

// 响应式数据
const article = ref<ArticleDetailResponse | null>(null);
const loading = ref(false);
const error = ref('');
const liked = ref(false);
const newComment = ref('');
const comments = ref<any[]>([]);
const relatedArticles = ref<any[]>([]);

// 页面元数据
useHead({
  title: computed(() => article.value ? `${article.value.title} - 檐下风铃` : '文章详情 - 檐下风铃'),
  meta: [
    { name: 'description', content: computed(() => article.value?.summary || '文章详情页面') }
  ]
});

// 计算页面背景样式
const pageBackgroundStyle = computed(() => {
  if (!article.value?.color) {
    // 默认背景
    return {
      background: 'linear-gradient(135deg, #f4f7f6 0%, #e8f5f3 100%)',
      minHeight: '100vh'
    }
  }

  const baseColor = article.value.color
  const lighterColor = lightenColor(baseColor, 0.8)
  const darkerColor = darkenColor(baseColor, 0.1)

  return {
    background: `linear-gradient(135deg, ${lighterColor} 0%, ${baseColor} 50%, ${darkerColor} 100%)`,
    minHeight: '100vh'
  }
});

// 计算 Markdown 样式
const markdownStyle = computed(() => {
  if (!article.value?.color) {
    // 如果没有颜色，使用项目的主色调的实际颜色值
    const defaultColor = '#076cd1' // 项目主色调
    const defaultLightColor = 'rgba(7, 108, 209, 0.15)' // 使用 rgba 格式，更适合背景

    console.log('Using default colors:', defaultColor, 'Light:', defaultLightColor) // 调试信息

    return {
      '--article-color': defaultColor,
      '--article-color-light': defaultLightColor
    }
  }

  const baseColor = article.value.color
  // 将 hex 颜色转换为 rgba 格式的浅色
  const rgb = hexToRgb(baseColor)
  const lighterColor = rgb ? `rgba(${rgb.r}, ${rgb.g}, ${rgb.b}, 0.15)` : lightenColor(baseColor, 0.7)

  console.log('Article color:', baseColor, 'Lighter color:', lighterColor) // 调试信息

  return {
    '--article-color': baseColor,
    '--article-color-light': lighterColor
  }
});

// 颜色处理工具函数
const hexToRgb = (hex: string) => {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
  return result ? {
    r: parseInt(result[1], 16),
    g: parseInt(result[2], 16),
    b: parseInt(result[3], 16)
  } : null;
};

const rgbToHex = (r: number, g: number, b: number) => {
  return `#${((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1)}`;
};

const lightenColor = (color: string, amount: number) => {
  const rgb = hexToRgb(color);
  if (!rgb) return color;

  const r = Math.min(255, Math.round(rgb.r + (255 - rgb.r) * amount));
  const g = Math.min(255, Math.round(rgb.g + (255 - rgb.g) * amount));
  const b = Math.min(255, Math.round(rgb.b + (255 - rgb.b) * amount));

  return rgbToHex(r, g, b);
};

const darkenColor = (color: string, amount: number) => {
  const rgb = hexToRgb(color);
  if (!rgb) return color;

  const r = Math.max(0, Math.round(rgb.r * (1 - amount)));
  const g = Math.max(0, Math.round(rgb.g * (1 - amount)));
  const b = Math.max(0, Math.round(rgb.b * (1 - amount)));

  return rgbToHex(r, g, b);
};



// 加载文章
const loadArticle = async () => {
  const result = await getArticleById(articleId);
  if (result) {
    article.value = result;
  }
};

// 方法
const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
};

const formatContent = (content: string | null | undefined) => {
  if (!content) return '';
  // 使用 marked 解析 Markdown
  return marked(content);
};

const formatTime = (timeString: string) => {
  const date = new Date(timeString);
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const hours = Math.floor(diff / (1000 * 60 * 60));
  
  if (hours < 1) {
    return '刚刚';
  } else if (hours < 24) {
    return `${hours}小时前`;
  } else {
    return date.toLocaleDateString('zh-CN');
  }
};

const toggleLike = () => {
  liked.value = !liked.value;
};

const shareArticle = () => {
  if (navigator.share) {
    navigator.share({
      title: article.value?.title,
      text: article.value?.summary,
      url: window.location.href
    });
  } else {
    // 复制链接到剪贴板
    navigator.clipboard.writeText(window.location.href);
    alert('链接已复制到剪贴板');
  }
};

const scrollToComments = () => {
  document.getElementById('comments')?.scrollIntoView({ behavior: 'smooth' });
};

const submitComment = () => {
  if (newComment.value.trim()) {
    const comment = {
      id: comments.value.length + 1,
      author: '当前用户',
      avatar: '/image/avatar.jpg',
      content: newComment.value,
      time: new Date().toISOString(),
      likes: 0
    };
    comments.value.unshift(comment);
    newComment.value = '';
  }
};

const likeComment = (commentId: number) => {
  const comment = comments.value.find(c => c.id === commentId);
  if (comment) {
    comment.likes++;
  }
};

const replyToComment = (commentId: number) => {
  // 实现回复功能
  console.log('回复评论:', commentId);
};

const goToArticle = (id: number) => {
  navigateTo(`/articles/${id}`);
};

const goBack = () => {
  history.back();
};

// 页面加载时获取文章
onMounted(() => {
  loadArticle();
});
</script>

<style scoped>
/* 文章详情页样式 */
.article-detail-page {
  padding: 2rem 0;
  min-height: 100vh;
  transition: background 0.5s ease;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.article-detail {
  max-width: 800px;
  margin: 0 auto;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: var(--border-radius);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.article-header {
  padding: 30px;
  border-bottom: 1px solid #eee;
}

.article-meta {
  display: flex;
  gap: 20px;
  margin-bottom: 15px;
  font-size: 0.9rem;
  color: var(--medium-gray-color);
}

.article-title {
  font-size: 2rem;
  margin-bottom: 15px;
  color: var(--dark-color);
  line-height: 1.3;
}

.article-summary {
  font-size: 1.1rem;
  color: var(--dark-gray-color);
  margin-bottom: 20px;
  line-height: 1.6;
}

.article-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 15px;
}

.tag {
  background-color: var(--light-color);
  color: var(--dark-color);
  padding: 0.3rem 0.8rem;
  border-radius: 1rem;
  font-size: 0.85rem;
  font-weight: 500;
  transition: all 0.3s ease;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.article-content {
  padding: 30px;
}

.content-text {
  font-size: 1.1rem;
  line-height: 1.8;
  color: var(--text-color);
}

/* Markdown 样式 */

.markdown-content :deep(strong) {
  color: var(--article-color, var(--primary-color)) !important;
  font-weight: 700;
}

.markdown-content :deep(em) {
  color: var(--article-color, var(--primary-color)) !important;
  font-style: italic;
}

.markdown-content :deep(mark) {
  background-color: var(--article-color-light, rgba(7, 108, 209, 0.2)) !important;
  color: var(--article-color, var(--primary-color)) !important;
  padding: 0.1em 0.2em;
  border-radius: 3px;
}

/* 有序列表样式 */
.markdown-content :deep(ol) {
  counter-reset: list-counter;
  padding-left: 0;
  margin-left: 1.5em;
}

.markdown-content :deep(ol li) {
  counter-increment: list-counter;
  position: relative;
  padding-left: 0.5em;
  margin-bottom: 0.5em;
}

.markdown-content :deep(ol li::before) {
  content: counter(list-counter) ".";
  position: absolute;
  left: -1.5em;
  color: var(--article-color, var(--primary-color)) !important;
  font-weight: bold;
  width: 1.2em;
  text-align: right;
}

/* 无序列表样式 */
.markdown-content :deep(ul li::before) {
  content: "•";
  color: var(--article-color, var(--primary-color)) !important;
  font-weight: bold;
  position: absolute;
  left: -1em;
}

.markdown-content :deep(ul) {
  padding-left: 0;
  margin-left: 1.5em;
}

.markdown-content :deep(ul li) {
  position: relative;
  padding-left: 0.5em;
  margin-bottom: 0.5em;
  list-style: none;
}

/* 链接样式 */
.markdown-content :deep(a) {
  color: var(--article-color, var(--primary-color)) !important;
  text-decoration: none;
  border-bottom: 1px solid var(--article-color-light, rgba(7, 108, 209, 0.3));
  transition: all 0.3s ease;
}

.markdown-content :deep(a:hover) {
  color: var(--article-color, var(--primary-color)) !important;
  border-bottom-color: var(--article-color, var(--primary-color)) !important;
}

/* 引用样式 */
.markdown-content :deep(blockquote) {
  border-left: 4px solid var(--article-color, var(--primary-color)) !important;
  margin: 1em 0;
  padding: 0.5em 1em;
  background-color: var(--article-color-light, rgba(7, 108, 209, 0.05)) !important;
  font-style: italic;
}

/* 标题样式 */
.markdown-content :deep(h1),
.markdown-content :deep(h2),
.markdown-content :deep(h3),
.markdown-content :deep(h4),
.markdown-content :deep(h5),
.markdown-content :deep(h6) {
  color: var(--article-color, var(--primary-color)) !important;
  margin-top: 1.5em;
  margin-bottom: 0.5em;
}

/* 代码样式 */
.markdown-content :deep(code) {
  background-color: var(--article-color-light, rgba(7, 108, 209, 0.1)) !important;
  color: var(--article-color, var(--primary-color)) !important;
  padding: 0.2em 0.4em;
  border-radius: 3px;
  font-family: 'Courier New', monospace;
  font-size: 0.9em;
}

.markdown-content :deep(pre) {
  background-color: #f8f9fa;
  border: 1px solid var(--article-color-light, rgba(7, 108, 209, 0.2)) !important;
  border-radius: 5px;
  padding: 1em;
  overflow-x: auto;
  margin: 1em 0;
}

.markdown-content :deep(pre code) {
  background: none !important;
  padding: 0;
  color: inherit;
}

.article-actions {
  display: flex;
  gap: 15px;
  padding: 20px 30px;
  border-top: 1px solid #eee;
  border-bottom: 1px solid #eee;
}

.action-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 15px;
  background: rgba(255, 255, 255, 0.8);
  color: var(--dark-gray-color);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: all 0.3s;
  backdrop-filter: blur(5px);
}

.action-btn:hover {
  background: var(--primary-color);
  color: white;
  transform: translateY(-1px);
}

.like-btn.liked {
  background: var(--accent-color-2);
  color: white;
}

/* 相关文章 */
.related-articles {
  padding: 30px;
  border-bottom: 1px solid #eee;
}

.section-title {
  font-size: 1.3rem;
  margin-bottom: 20px;
  color: var(--dark-color);
}

.related-grid {
  display: grid;
  gap: 15px;
}

.related-card {
  display: flex;
  gap: 15px;
  padding: 15px;
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(5px);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: all 0.3s;
}

.related-card:hover {
  transform: translateY(-2px);
  background: rgba(255, 255, 255, 0.9);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

.related-image {
  width: 100px;
  height: 80px;
  border-radius: var(--border-radius);
  overflow: hidden;
  flex-shrink: 0;
}

.related-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.related-info {
  flex: 1;
}

.related-title {
  font-size: 1rem;
  margin-bottom: 5px;
  color: var(--dark-color);
}

.related-summary {
  font-size: 0.9rem;
  color: var(--medium-gray-color);
  line-height: 1.4;
}

/* 返回导航 */
.back-navigation {
  margin-top: 30px;
  text-align: center;
}

.back-btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  background: var(--secondary-color);
  color: white;
  border: none;
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: background-color 0.3s;
}

.back-btn:hover {
  background: #008a7a;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .article-detail-page {
    padding: 1rem 0;
  }

  .article-header {
    padding: 20px;
  }

  .article-title {
    font-size: 1.5rem;
  }

  .article-content {
    padding: 20px;
  }

  .article-actions {
    padding: 15px 20px;
    flex-wrap: wrap;
  }

  .related-articles {
    padding: 20px;
  }

  .related-card {
    flex-direction: column;
    gap: 10px;
  }

  .related-image {
    width: 100%;
    height: 150px;
  }
}
</style>
