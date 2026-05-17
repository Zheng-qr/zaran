<template>
  <nav class="navbar">
    <div class="container">
      <NuxtLink to="/" class="logo">檐下风铃</NuxtLink>
      <div class="desktop-nav">
        <ul class="nav-links">
          <li><NuxtLink to="/" :class="{ 'active-link': isActive('/') }">首页</NuxtLink></li>
          <li><NuxtLink to="/ip-story" :class="{ 'active-link': isActive('/ip-story') }">IP故事</NuxtLink></li>
          <li><NuxtLink to="/knowledge" :class="{ 'active-link': isActive('/knowledge') }">科普百科</NuxtLink></li>
          <li><NuxtLink to="/pattern-library" :class="{ 'active-link': isActive('/pattern-library') }">基因库</NuxtLink></li>
          <li><NuxtLink to="/workshop" :class="{ 'active-link': isActive('/workshop') }">创意工坊</NuxtLink></li>
          <li><NuxtLink to="/shop" :class="{ 'active-link': isActive('/shop') }">文创商城</NuxtLink></li>
          <li><NuxtLink to="/community" :class="{ 'active-link': isActive('/community') }">共创社区</NuxtLink></li>
          <li><NuxtLink to="/my" :class="{ 'active-link': isActive('/my') }">我的</NuxtLink></li>
        </ul>
      </div>
      <div class="user-actions">
        <template v-if="!userStore.isLoggedIn">
          <button @click="handleLogin" class="auth-btn login-btn">登录</button>
          <button @click="handleRegister" class="auth-btn register-btn">注册</button>
        </template>
        <template v-else>
          <NuxtLink to="/my" :class="{ 'active-link': isActive('/my') }">我的</NuxtLink>
          <NuxtLink to="/my" title="用户中心"><i class="fas fa-user"></i></NuxtLink>
          <NuxtLink to="/message" title="消息"><i class="fas fa-envelope"></i></NuxtLink>
          <NuxtLink v-if="isAdmin" to="/admin" title="管理员控制台"><i class="fas fa-cog"></i></NuxtLink>
          <span class="username">{{ userStore.user?.nickname || userStore.user?.username || '用户' }}</span>
          <button @click="handleLogout" class="auth-btn logout-btn">退出</button>
        </template>
        <NuxtLink to="/about" title="关于我们"><i class="fas fa-info-circle"></i></NuxtLink>
      </div>
      <button class="mobile-menu-btn" aria-label="Toggle menu" :aria-expanded="mobileMenuActive" @click="toggleMobileMenu">
        <i :class="['fas', mobileMenuActive ? 'fa-times' : 'fa-bars']"></i>
      </button>
    </div>
    <!-- 移动端菜单容器 -->
    <div class="mobile-nav-menu" :class="{ 'active': mobileMenuActive }">
      <ul class="nav-links">
        <li><NuxtLink to="/" @click="closeMobileMenu">首页</NuxtLink></li>
        <li><NuxtLink to="/ip-story" @click="closeMobileMenu">IP故事</NuxtLink></li>
        <li><NuxtLink to="/knowledge" @click="closeMobileMenu">科普百科</NuxtLink></li>
        <li><NuxtLink to="/pattern-library" @click="closeMobileMenu">基因库</NuxtLink></li>
        <li><NuxtLink to="/workshop" @click="closeMobileMenu">创意工坊</NuxtLink></li>
        <li><NuxtLink to="/shop" @click="closeMobileMenu">文创商城</NuxtLink></li>
        <li><NuxtLink to="/community" @click="closeMobileMenu">共创社区</NuxtLink></li>
      </ul>
      <div class="user-actions">
        <NuxtLink to="/my" title="用户中心" @click="closeMobileMenu"><i class="fas fa-user"></i></NuxtLink>
        <NuxtLink to="/message" title="消息" @click="closeMobileMenu"><i class="fas fa-envelope"></i></NuxtLink>
        <NuxtLink to="/about" title="关于我们" @click="closeMobileMenu"><i class="fas fa-info-circle"></i></NuxtLink>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
const route = useRoute();
const router = useRouter();
const userStore = useUserStore();
const mobileMenuActive = ref(false);

// 判断当前用户是否为管理员
const isAdmin = computed(() => {
  return userStore.user?.userRole === 4; // 4 是 Admin 角色
});

onMounted(() => {
  userStore.fetchUserInfo();
});

function toggleMobileMenu() {
  mobileMenuActive.value = !mobileMenuActive.value;
}

function closeMobileMenu() {
  mobileMenuActive.value = false;
}

function isActive(path: string): boolean {
  return route.path === path;
}

function handleLogin() {
  router.push('/login');
}

function handleRegister() {
  router.push('/register');
}

async function handleLogout() {
  await userStore.logout();
  router.push('/');
}
</script>

<style scoped>
/* 导航栏样式 */
.navbar {
  background-color: var(--dark-color);
  padding: 15px 0;
  position: sticky;
  top: 0;
  z-index: 1000;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.navbar .container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  color: var(--accent-color-1);
  font-size: 1.6rem;
  font-weight: bold;
  text-decoration: none;
}

.desktop-nav {
  flex: 1;
  display: flex;
  justify-content: center;
}

.nav-links {
  display: flex;
  list-style: none;
  align-items: center;
}

.nav-links li {
  margin: 0 15px;
}

.nav-links a {
  color: var(--text-light-color);
  text-decoration: none;
  font-weight: 500;
  transition: color 0.3s;
  padding: 5px 0;
}

.nav-links a:hover,
.nav-links a.active-link {
  color: var(--accent-color-1);
}

.user-actions {
  display: flex;
  align-items: center;
}

.user-actions a {
  color: var(--text-light-color);
  margin-left: 20px;
  font-size: 1.2rem;
  transition: color 0.3s;
}

.user-actions a:hover {
  color: var(--accent-color-1);
}

.auth-btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.login-btn {
  background: transparent;
  color: var(--accent-color-1);
  border: 1px solid var(--accent-color-1);
}

.login-btn:hover {
  background: var(--accent-color-1);
  color: var(--dark-color);
}

.register-btn {
  background: var(--accent-color-1);
  color: var(--dark-color);
}

.register-btn:hover {
  background: var(--accent-color-2);
}

.logout-btn {
  background: var(--danger-color);
  color: var(--text-light-color);
  padding: 0.3rem 0.8rem;
  font-size: 0.8rem;
}

.logout-btn:hover {
  background: var(--danger-hover-color);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.username {
  color: var(--text-light-color);
  font-weight: 500;
  margin-right: 0.5rem;
}

.mobile-menu-btn {
  display: none;
  background: none;
  border: none;
  color: var(--text-light-color);
  font-size: 1.6rem;
  cursor: pointer;
}

/* 移动端展开的导航菜单样式 */
.mobile-nav-menu {
  display: none;
  flex-direction: column;
  width: 100%;
  background-color: var(--dark-color);
  position: absolute;
  top: 100%;
  left: 0;
  padding: 10px 0;
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
  z-index: 999;
}

.mobile-nav-menu.active {
  display: flex;
}

.mobile-nav-menu .nav-links {
  flex-direction: column;
  width: 100%;
}

.mobile-nav-menu .nav-links li {
  margin: 10px 0;
  text-align: center;
}

.mobile-nav-menu .user-actions {
  flex-direction: row;
  justify-content: center;
  padding-top: 10px;
  margin-top: 10px;
  border-top: 1px solid var(--dark-gray-color);
}

.mobile-nav-menu .user-actions a {
  margin: 0 15px;
}

/* Header 响应式设计 */
@media (max-width: 768px) {
  .desktop-nav, .navbar .user-actions {
    display: none;
  }

  .mobile-menu-btn {
    display: block;
  }

  .auth-btn {
    padding: 0.4rem 0.8rem;
    font-size: 0.8rem;
  }

  .user-info {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.25rem;
  }
}
</style>
