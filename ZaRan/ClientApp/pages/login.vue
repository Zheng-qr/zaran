<template>
  <div class="login-page">
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <h1 class="login-title">登录</h1>
          <p class="login-subtitle">欢迎回到檐下风铃</p>
        </div>
        
        <form @submit.prevent="handleLogin" class="login-form">
          <!-- 错误信息显示 -->
          <div v-if="errorMessage" class="error-message">
            <i class="fas fa-exclamation-circle"></i>
            {{ errorMessage }}
          </div>

          <div class="form-group">
            <label for="username" class="form-label">用户名/邮箱</label>
            <input
              id="username"
              v-model="loginForm.username"
              type="text"
              class="form-input"
              placeholder="请输入用户名或邮箱"
              required
            >
          </div>
          
          <div class="form-group">
            <label for="password" class="form-label">密码</label>
            <div class="password-input-wrapper">
              <input 
                id="password"
                v-model="loginForm.password" 
                :type="showPassword ? 'text' : 'password'" 
                class="form-input"
                placeholder="请输入密码"
                required
              >
              <button 
                type="button" 
                class="password-toggle"
                @click="showPassword = !showPassword"
              >
                <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
              </button>
            </div>
          </div>
          
          <div class="form-options">
            <label class="remember-me">
              <input type="checkbox" v-model="loginForm.rememberMe">
              <span class="checkmark"></span>
              记住我
            </label>
            <NuxtLink to="/forgot-password" class="forgot-password">
              忘记密码？
            </NuxtLink>
          </div>
          
          <button type="submit" class="login-btn" :disabled="userStore.loading">
            <span v-if="userStore.loading" class="loading-spinner"></span>
            {{ userStore.loading ? '登录中...' : '登录' }}
          </button>
        </form>
        
        <div class="divider">
          <span>或</span>
        </div>
        
        <div class="social-login">
          <button class="social-btn wechat-btn">
            <i class="fab fa-weixin"></i>
            微信登录
          </button>
          <button class="social-btn qq-btn">
            <i class="fab fa-qq"></i>
            QQ登录
          </button>
        </div>
        
        <div class="login-footer">
          <p>还没有账号？ <NuxtLink to="/register" class="register-link">立即注册</NuxtLink></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// 页面元数据
useHead({
  title: '登录 - 檐下风铃',
  meta: [
    { name: 'description', content: '登录檐下风铃扎染平台' }
  ]
})

// 表单数据
const loginForm = ref({
  username: '',
  password: '',
  rememberMe: false
});

const showPassword = ref(false);
const userStore = useUserStore();
const errorMessage = ref('');
const route = useRoute();

// 登录处理
const handleLogin = async () => {
  errorMessage.value = '';

  try {
    const success = await userStore.login({
      account: loginForm.value.username,
      password: loginForm.value.password
    });

    if (success) {
      // 获取重定向路径，如果没有则跳转到首页
      const redirectPath = route.query.redirect as string || '/';
      await navigateTo(redirectPath);
    } else {
      errorMessage.value = userStore.error || '登录失败，请检查用户名和密码';
    }
  } catch (error) {
    console.error('登录失败:', error);
    errorMessage.value = '登录过程中发生错误，请稍后重试';
  }
};

// 社交登录
const handleSocialLogin = (provider: string) => {
  console.log(`${provider} 登录`);
  // 实现社交登录逻辑
};
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-container {
  width: 100%;
  max-width: 400px;
}

.login-card {
  background: white;
  border-radius: 12px;
  padding: 40px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-title {
  font-size: 2rem;
  font-weight: 700;
  color: #333;
  margin-bottom: 8px;
}

.login-subtitle {
  color: #666;
  font-size: 0.9rem;
}

.error-message {
  background: #fee;
  border: 1px solid #fcc;
  color: #c33;
  padding: 12px;
  border-radius: 8px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.9rem;
}

.form-group {
  margin-bottom: 20px;
}

.form-label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #333;
}

.form-input {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e1e5e9;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
}

.password-input-wrapper {
  position: relative;
}

.password-toggle {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: #666;
  cursor: pointer;
}

.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
}

.remember-me {
  display: flex;
  align-items: center;
  cursor: pointer;
  font-size: 0.9rem;
}

.forgot-password {
  color: #667eea;
  text-decoration: none;
  font-size: 0.9rem;
}

.login-btn {
  width: 100%;
  padding: 12px;
  background: #667eea;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s;
}

.login-btn:hover:not(:disabled) {
  background: #5a6fd8;
}

.login-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.divider {
  text-align: center;
  margin: 25px 0;
  position: relative;
}

.divider::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background: #e1e5e9;
}

.divider span {
  background: white;
  padding: 0 15px;
  color: #666;
  font-size: 0.9rem;
}

.social-login {
  display: flex;
  gap: 10px;
  margin-bottom: 25px;
}

.social-btn {
  flex: 1;
  padding: 10px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  transition: opacity 0.3s;
}

.wechat-btn {
  background: #1aad19;
  color: white;
}

.qq-btn {
  background: #12b7f5;
  color: white;
}

.social-btn:hover {
  opacity: 0.9;
}

.login-footer {
  text-align: center;
}

.register-link {
  color: #667eea;
  text-decoration: none;
  font-weight: 500;
}

.loading-spinner {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid #ffffff;
  border-radius: 50%;
  border-top-color: transparent;
  animation: spin 1s ease-in-out infinite;
  margin-right: 8px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
