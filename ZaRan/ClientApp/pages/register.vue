<template>
  <div class="register-page">
    <div class="register-container">
      <div class="register-card">
        <div class="register-header">
          <h1 class="register-title">注册</h1>
          <p class="register-subtitle">加入檐下风铃扎染社区</p>
        </div>
        
        <form @submit.prevent="handleRegister" class="register-form">
          <!-- 错误信息显示 -->
          <div v-if="errorMessage" class="error-message">
            <i class="fas fa-exclamation-circle"></i>
            {{ errorMessage }}
          </div>

          <div class="form-group">
            <label for="username" class="form-label">用户名</label>
            <input
              id="username"
              v-model="registerForm.username"
              type="text"
              class="form-input"
              placeholder="请输入用户名"
              required
            >
          </div>

          <div class="form-group">
            <label for="nickname" class="form-label">昵称</label>
            <input
              id="nickname"
              v-model="registerForm.nickname"
              type="text"
              class="form-input"
              placeholder="请输入昵称"
              required
            >
          </div>
          
          <div class="form-group">
            <label for="email" class="form-label">邮箱</label>
            <input 
              id="email"
              v-model="registerForm.email" 
              type="email" 
              class="form-input"
              placeholder="请输入邮箱地址"
              required
            >
          </div>
          
          <div class="form-group">
            <label for="password" class="form-label">密码</label>
            <div class="password-input-wrapper">
              <input 
                id="password"
                v-model="registerForm.password" 
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
            <div class="password-strength">
              <div class="strength-bar" :class="passwordStrength.class">
                <div class="strength-fill" :style="{ width: passwordStrength.width }"></div>
              </div>
              <span class="strength-text">{{ passwordStrength.text }}</span>
            </div>
          </div>
          
          <div class="form-group">
            <label for="confirmPassword" class="form-label">确认密码</label>
            <input 
              id="confirmPassword"
              v-model="registerForm.confirmPassword" 
              type="password" 
              class="form-input"
              placeholder="请再次输入密码"
              required
            >
            <div v-if="registerForm.confirmPassword && !passwordsMatch" class="error-message">
              密码不匹配
            </div>
          </div>
          
          <div class="form-group">
            <label class="agreement">
              <input type="checkbox" v-model="registerForm.agreeToTerms" required>
              <span class="checkmark"></span>
              我已阅读并同意 
              <NuxtLink to="/terms" class="terms-link">用户协议</NuxtLink> 
              和 
              <NuxtLink to="/privacy" class="privacy-link">隐私政策</NuxtLink>
            </label>
          </div>
          
          <button type="submit" class="register-btn" :disabled="userStore.loading || !canSubmit">
            <span v-if="userStore.loading" class="loading-spinner"></span>
            {{ userStore.loading ? '注册中...' : '注册' }}
          </button>
        </form>
        
        <div class="register-footer">
          <p>已有账号？ <NuxtLink to="/login" class="login-link">立即登录</NuxtLink></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// 页面元数据
useHead({
  title: '注册 - 檐下风铃',
  meta: [
    { name: 'description', content: '注册檐下风铃扎染平台账号' }
  ]
})

// 表单数据
const registerForm = ref({
  username: '',
  nickname: '',
  email: '',
  password: '',
  confirmPassword: '',
  agreeToTerms: false
});

const showPassword = ref(false);
const userStore = useUserStore();
const errorMessage = ref('');

// 计算属性
const passwordsMatch = computed(() => {
  return registerForm.value.password === registerForm.value.confirmPassword;
});

const passwordStrength = computed(() => {
  const password = registerForm.value.password;
  if (!password) return { class: '', width: '0%', text: '' };
  
  let score = 0;
  if (password.length >= 8) score++;
  if (/[a-z]/.test(password)) score++;
  if (/[A-Z]/.test(password)) score++;
  if (/[0-9]/.test(password)) score++;
  if (/[^A-Za-z0-9]/.test(password)) score++;
  
  if (score < 2) return { class: 'weak', width: '20%', text: '弱' };
  if (score < 4) return { class: 'medium', width: '60%', text: '中等' };
  return { class: 'strong', width: '100%', text: '强' };
});

const canSubmit = computed(() => {
  return registerForm.value.username &&
         registerForm.value.nickname &&
         registerForm.value.email &&
         registerForm.value.password &&
         passwordsMatch.value &&
         registerForm.value.agreeToTerms;
});

// 注册处理
const handleRegister = async () => {
  if (!canSubmit.value) return;

  errorMessage.value = '';

  try {
    const success = await userStore.register({
      username: registerForm.value.username,
      nickname: registerForm.value.nickname,
      email: registerForm.value.email,
      password: registerForm.value.password
    });

    if (success) {
      // 注册成功后跳转到首页
      await navigateTo('/');
    } else {
      errorMessage.value = userStore.error || '注册失败，请检查输入信息';
    }
  } catch (error) {
    console.error('注册失败:', error);
    errorMessage.value = '注册过程中发生错误，请稍后重试';
  }
};
</script>

<style scoped>
.register-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.register-container {
  width: 100%;
  max-width: 450px;
}

.register-card {
  background: white;
  border-radius: 12px;
  padding: 40px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
}

.register-header {
  text-align: center;
  margin-bottom: 30px;
}

.register-title {
  font-size: 2rem;
  font-weight: 700;
  color: #333;
  margin-bottom: 8px;
}

.register-subtitle {
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

.password-strength {
  margin-top: 8px;
}

.strength-bar {
  height: 4px;
  background: #e1e5e9;
  border-radius: 2px;
  overflow: hidden;
  margin-bottom: 4px;
}

.strength-fill {
  height: 100%;
  transition: width 0.3s;
}

.strength-bar.weak .strength-fill { background: #ff4757; }
.strength-bar.medium .strength-fill { background: #ffa502; }
.strength-bar.strong .strength-fill { background: #2ed573; }

.strength-text {
  font-size: 0.8rem;
  color: #666;
}

.error-message {
  color: #ff4757;
  font-size: 0.8rem;
  margin-top: 4px;
}

.agreement {
  display: flex;
  align-items: flex-start;
  cursor: pointer;
  font-size: 0.9rem;
  line-height: 1.4;
}

.agreement input {
  margin-right: 8px;
  margin-top: 2px;
}

.terms-link, .privacy-link {
  color: #667eea;
  text-decoration: none;
}

.register-btn {
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
  margin-top: 10px;
}

.register-btn:hover:not(:disabled) {
  background: #5a6fd8;
}

.register-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.register-footer {
  text-align: center;
  margin-top: 25px;
}

.login-link {
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
