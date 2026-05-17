<template>
  <Teleport to="body">
    <Transition name="loading-overlay">
      <div v-if="isLoading" class="loading-overlay">
        <div class="loading-content">
          <div class="loading-spinner">
            <div class="spinner-ring"></div>
            <div class="spinner-ring"></div>
            <div class="spinner-ring"></div>
            <div class="spinner-ring"></div>
          </div>
          <div class="loading-text">
            <div class="loading-message">{{ currentLoadingMessage }}</div>
            <div v-if="currentProgress > 0" class="loading-progress">
              <div class="progress-bar">
                <div 
                  class="progress-fill" 
                  :style="{ width: `${currentProgress}%` }"
                ></div>
              </div>
              <div class="progress-text">{{ currentProgress }}%</div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
const { 
  isLoading, 
  currentLoadingMessage, 
  currentProgress 
} = useGlobalLoading()
</script>

<style scoped>
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  z-index: 9998;
  display: flex;
  align-items: center;
  justify-content: center;
}

.loading-content {
  background: white;
  border-radius: 12px;
  padding: 32px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  text-align: center;
  max-width: 300px;
  width: 90%;
}

.loading-spinner {
  position: relative;
  width: 60px;
  height: 60px;
  margin: 0 auto 24px;
}

.spinner-ring {
  position: absolute;
  width: 100%;
  height: 100%;
  border: 3px solid transparent;
  border-top-color: #667eea;
  border-radius: 50%;
  animation: spin 1.2s linear infinite;
}

.spinner-ring:nth-child(1) {
  animation-delay: 0s;
}

.spinner-ring:nth-child(2) {
  animation-delay: 0.1s;
  width: 80%;
  height: 80%;
  top: 10%;
  left: 10%;
  border-top-color: #764ba2;
}

.spinner-ring:nth-child(3) {
  animation-delay: 0.2s;
  width: 60%;
  height: 60%;
  top: 20%;
  left: 20%;
  border-top-color: #f093fb;
}

.spinner-ring:nth-child(4) {
  animation-delay: 0.3s;
  width: 40%;
  height: 40%;
  top: 30%;
  left: 30%;
  border-top-color: #f5576c;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.loading-text {
  color: #374151;
}

.loading-message {
  font-size: 1rem;
  font-weight: 500;
  margin-bottom: 16px;
}

.loading-progress {
  margin-top: 16px;
}

.progress-bar {
  width: 100%;
  height: 6px;
  background: #e5e7eb;
  border-radius: 3px;
  overflow: hidden;
  margin-bottom: 8px;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
  border-radius: 3px;
  transition: width 0.3s ease;
}

.progress-text {
  font-size: 0.8rem;
  color: #6b7280;
}

/* 动画 */
.loading-overlay-enter-active {
  transition: all 0.3s ease-out;
}

.loading-overlay-leave-active {
  transition: all 0.3s ease-in;
}

.loading-overlay-enter-from {
  opacity: 0;
}

.loading-overlay-leave-to {
  opacity: 0;
}

.loading-overlay-enter-from .loading-content {
  transform: scale(0.9);
}

.loading-overlay-leave-to .loading-content {
  transform: scale(0.9);
}

.loading-content {
  transition: transform 0.3s ease;
}

/* 响应式设计 */
@media (max-width: 640px) {
  .loading-content {
    padding: 24px;
    max-width: 280px;
  }

  .loading-spinner {
    width: 50px;
    height: 50px;
    margin-bottom: 20px;
  }

  .loading-message {
    font-size: 0.9rem;
  }
}
</style>
