<template>
  <footer>
    <div class="container">
      <div class="footer-content">
        <div class="footer-section">
          <h3>关于我们</h3>
          <p>{{ aboutText }}</p>
        </div>
        <div class="footer-section">
          <h3>快速链接</h3>
          <ul>
            <li v-for="(link, index) in quickLinks" :key="index">
              <NuxtLink :to="link.path">{{ link.text }}</NuxtLink>
            </li>
          </ul>
        </div>
        <div class="footer-section">
          <h3>联系我们</h3>
          <p><i class="fas fa-phone"></i> 电话: {{ contactPhone }}</p>
          <div class="social-icons">
            <a v-for="(social, index) in socialLinks" :key="index" :href="social.url" :title="social.title">
              <i :class="social.icon"></i>
            </a>
          </div>
        </div>
      </div>
      <div class="copyright">
        <p>&copy; {{ copyright }}</p>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
// 页脚数据
const aboutText = ref("\"檐下风铃\"致力于保护和传承中国传统扎染工艺，融合现代设计理念，打造一个充满活力的数字文化平台。");

const quickLinks = ref([
  { path: '/ip-story', text: 'IP故事' },
  { path: '/knowledge', text: '扎染工艺' },
  { path: '/pattern-library', text: '纹样基因库' },
  { path: '/shop', text: '文创商城' },
  { path: '/community', text: '共创社区' },
  { path: '/community', text: '关于平台' }
]);

const contactPhone = ref("+86 13924843248");

const socialLinks = ref([
  { url: "#", title: "微信", icon: "fab fa-weixin" },
  { url: "#", title: "微博", icon: "fab fa-weibo" },
  { url: "#", title: "Bilibili", icon: "fab fa-bilibili" },
]);

const copyright = ref("檐下风铃. 保留所有权利.");
</script>

<style scoped>
/* 页脚样式 */
footer {
  background: var(--dark-color);
  color: var(--medium-gray-color);
  padding: 3rem 0 1.5rem;
}

.footer-content {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 30px;
  margin-bottom: 2.5rem;
}

.footer-section h3 {
  margin-bottom: 1rem;
  color: var(--accent-color-1);
  font-size: 1.2rem;
}

.footer-section ul {
  list-style: none;
}

.footer-section ul li {
  margin-bottom: 10px;
}

.footer-section a {
  color: var(--medium-gray-color);
  text-decoration: none;
  transition: color 0.3s;
}

.footer-section a:hover {
  color: var(--accent-color-1);
}

.footer-section p {
  font-size: 0.9rem;
  line-height: 1.6;
}

.social-icons {
  margin-top: 15px;
}

.social-icons a {
  display: inline-block;
  margin-right: 12px;
  font-size: 1.3rem;
  color: var(--text-light-color);
  transition: color 0.3s;
}

.social-icons a:hover {
  color: var(--accent-color-1);
}

.copyright {
  text-align: center;
  padding-top: 20px;
  border-top: 1px solid #4a4a4a;
  font-size: 0.85rem;
}

/* Footer 响应式设计 */
@media (max-width: 768px) {
  .footer-content {
    grid-template-columns: 1fr;
    text-align: center;
  }

  .footer-section ul {
    padding-left: 0;
  }

  .social-icons {
    text-align: center;
  }
}
</style>
