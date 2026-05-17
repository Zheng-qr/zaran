<template>
  <Teleport to="body">
    <div class="notification-container">
      <TransitionGroup name="notification" tag="div">
        <div
          v-for="notification in notifications"
          :key="notification.id"
          class="notification"
          :class="getNotificationClasses(notification.type)"
        >
          <div class="notification-content">
            <div class="notification-icon">
              <i :class="getNotificationIcon(notification.type)"></i>
            </div>
            <div class="notification-text">
              <div class="notification-title">{{ notification.title }}</div>
              <div v-if="notification.message" class="notification-message">
                {{ notification.message }}
              </div>
            </div>
            <button
              @click="removeNotification(notification.id)"
              class="notification-close"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
const { 
  notifications, 
  removeNotification, 
  getNotificationIcon 
} = useGlobalNotifications()

const getNotificationClasses = (type: string) => {
  const baseClasses = 'notification-item'
  const typeClasses = {
    success: 'notification-success',
    error: 'notification-error',
    warning: 'notification-warning',
    info: 'notification-info'
  }
  return `${baseClasses} ${typeClasses[type as keyof typeof typeClasses] || typeClasses.info}`
}
</script>

<style scoped>
.notification-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 9999;
  pointer-events: none;
}

.notification {
  pointer-events: auto;
  margin-bottom: 12px;
  max-width: 400px;
  min-width: 300px;
}

.notification-item {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  overflow: hidden;
  border-left: 4px solid;
}

.notification-success {
  border-left-color: #10b981;
}

.notification-error {
  border-left-color: #ef4444;
}

.notification-warning {
  border-left-color: #f59e0b;
}

.notification-info {
  border-left-color: #3b82f6;
}

.notification-content {
  display: flex;
  align-items: flex-start;
  padding: 16px;
  gap: 12px;
}

.notification-icon {
  flex-shrink: 0;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: 2px;
}

.notification-success .notification-icon {
  color: #10b981;
}

.notification-error .notification-icon {
  color: #ef4444;
}

.notification-warning .notification-icon {
  color: #f59e0b;
}

.notification-info .notification-icon {
  color: #3b82f6;
}

.notification-text {
  flex: 1;
  min-width: 0;
}

.notification-title {
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 4px;
  line-height: 1.4;
}

.notification-message {
  color: #6b7280;
  font-size: 0.9rem;
  line-height: 1.4;
}

.notification-close {
  flex-shrink: 0;
  background: none;
  border: none;
  color: #9ca3af;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: color 0.2s;
}

.notification-close:hover {
  color: #6b7280;
}

/* 动画 */
.notification-enter-active {
  transition: all 0.3s ease-out;
}

.notification-leave-active {
  transition: all 0.3s ease-in;
}

.notification-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.notification-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

.notification-move {
  transition: transform 0.3s ease;
}

/* 响应式设计 */
@media (max-width: 640px) {
  .notification-container {
    top: 10px;
    right: 10px;
    left: 10px;
  }

  .notification {
    max-width: none;
    min-width: 0;
  }

  .notification-content {
    padding: 12px;
  }

  .notification-title {
    font-size: 0.9rem;
  }

  .notification-message {
    font-size: 0.8rem;
  }
}
</style>
