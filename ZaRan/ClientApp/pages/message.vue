<template>
  <div class="message-page">
    <div class="container">
      <h1 class="page-title">消息中心</h1>
      
      <!-- 消息类型导航 -->
      <div class="message-nav">
        <button 
          v-for="type in messageTypes" 
          :key="type.id"
          :class="['nav-tab', { active: activeType === type.id }]"
          @click="setActiveType(type.id)"
        >
          <i :class="type.icon"></i>
          {{ type.name }}
          <span v-if="type.unread > 0" class="unread-badge">{{ type.unread }}</span>
        </button>
      </div>

      <!-- 消息列表 -->
      <div class="message-list">
        <!-- 加载状态 -->
        <div v-if="loading" class="loading-state">
          <i class="fas fa-spinner fa-spin"></i>
          <p>加载中...</p>
        </div>

        <!-- 错误状态 -->
        <div v-else-if="error" class="error-state">
          <i class="fas fa-exclamation-triangle"></i>
          <p>{{ error }}</p>
          <button @click="loadMessages()" class="retry-btn">重试</button>
        </div>

        <!-- 消息列表 -->
        <div v-else class="messages">
          <div class="message-item" v-for="message in currentMessages" :key="message.id">
            <div class="message-icon" :class="getMessageTypeColor(message.type!)">
              <i :class="getMessageTypeIcon(message.type!)"></i>
            </div>
            <div class="message-content">
              <h3 class="message-title">{{ message.title }}</h3>
              <p class="message-text">{{ message.content }}</p>
              <div class="message-meta">
                <span class="message-sender" v-if="message.sender">
                  来自: {{ message.sender.nickName }}
                </span>
                <span class="message-time">{{ formatMessageTime(message.createdAt!) }}</span>
              </div>
            </div>
            <div class="message-actions">
              <button
                v-if="!message.isRead"
                class="mark-read-btn"
                @click="handleMarkAsRead(message.id!)"
              >
                标记已读
              </button>
              <button
                v-if="message.type === MessageType._4"
                class="reply-btn"
                @click="replyToMessage(message)"
              >
                回复
              </button>
              <button class="delete-btn" @click="handleDeleteMessage(message.id!)">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-if="currentMessages.length === 0" class="empty-state">
        <i class="fas fa-inbox"></i>
        <p>暂无消息</p>
      </div>
    </div>

    <!-- 回复模态框 -->
    <div v-if="replyModal.show" class="reply-modal" @click="closeReplyModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>回复 {{ replyModal.message?.username }}</h3>
          <button class="close-btn" @click="closeReplyModal">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <div class="original-message">
            <p>{{ replyModal.message?.content }}</p>
          </div>
          <textarea 
            v-model="replyContent" 
            placeholder="输入回复内容..."
            class="reply-textarea"
          ></textarea>
        </div>
        <div class="modal-footer">
          <button class="cancel-btn" @click="closeReplyModal">取消</button>
          <button class="send-btn" @click="sendReply">发送</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { MessageType } from '~/api'

// 页面元数据
useHead({
  title: '消息中心 - 檐下风铃',
  meta: [
    { name: 'description', content: '查看和管理消息通知' }
  ]
})

// 使用消息功能
const {
  messages,
  loading,
  error,
  fetchMessages,
  markAsRead,
  deleteMessage,
  getMessageTypeIcon,
  getMessageTypeColor,
  formatMessageTime
} = useMessages()

// 消息类型映射
const messageTypeMap = {
  [MessageType._0]: 'system',
  [MessageType._1]: 'order',
  [MessageType._2]: 'comment',
  [MessageType._3]: 'follow',
  [MessageType._4]: 'private'
}

const reverseMessageTypeMap = {
  'system': MessageType._0,
  'order': MessageType._1,
  'comment': MessageType._2,
  'follow': MessageType._3,
  'private': MessageType._4
}

// 消息类型
const messageTypes = ref([
  { id: 'system', name: '系统通知', icon: 'fas fa-bell', unread: 0, type: MessageType._0 },
  { id: 'order', name: '订单消息', icon: 'fas fa-shopping-cart', unread: 0, type: MessageType._1 },
  { id: 'comment', name: '评论回复', icon: 'fas fa-comment', unread: 0, type: MessageType._2 },
  { id: 'follow', name: '关注通知', icon: 'fas fa-user-plus', unread: 0, type: MessageType._3 },
  { id: 'private', name: '私信', icon: 'fas fa-envelope', unread: 0, type: MessageType._4 }
]);

const activeType = ref('system');

// 当前页面状态
const currentPage = ref(1)
const pageSize = ref(20)

// 获取当前类型的消息
const currentMessages = computed(() => {
  const currentTypeEnum = reverseMessageTypeMap[activeType.value]
  return messages.value.filter(msg => msg.type === currentTypeEnum)
})

// 更新未读计数
const updateUnreadCounts = () => {
  messageTypes.value.forEach(type => {
    const typeMessages = messages.value.filter(msg => msg.type === type.type)
    type.unread = typeMessages.filter(msg => !msg.isRead).length
  })
}

// 回复模态框
const replyModal = ref({
  show: false,
  message: null as any
});

const replyContent = ref('');

// 方法
const setActiveType = async (type: string) => {
  activeType.value = type;
  await loadMessages();
};

const loadMessages = async () => {
  const currentTypeEnum = reverseMessageTypeMap[activeType.value]
  await fetchMessages(currentPage.value, pageSize.value, currentTypeEnum)
  updateUnreadCounts()
}

const handleMarkAsRead = async (messageId: string) => {
  await markAsRead(messageId)
  updateUnreadCounts()
};

const handleDeleteMessage = async (messageId: string) => {
  await deleteMessage(messageId)
  updateUnreadCounts()
};

const replyToMessage = (message: any) => {
  replyModal.value.show = true;
  replyModal.value.message = message;
  replyContent.value = '';
};

const closeReplyModal = () => {
  replyModal.value.show = false;
  replyModal.value.message = null;
  replyContent.value = '';
};

const sendReply = () => {
  if (replyContent.value.trim()) {
    console.log('发送回复:', replyContent.value);
    closeReplyModal();
  }
};

const joinActivity = (activityId: string) => {
  console.log('参加活动:', activityId);
};

// 页面加载时获取消息
onMounted(async () => {
  await loadMessages()
})
</script>

<style scoped>
.message-page {
  min-height: 100vh;
  background-color: #f8fafc;
  padding: 2rem 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.page-title {
  font-size: 2rem;
  font-weight: bold;
  color: #1f2937;
  margin-bottom: 2rem;
  text-align: center;
}

.message-nav {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  border-bottom: 1px solid #e5e7eb;
  overflow-x: auto;
}

.nav-tab {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem 1.5rem;
  background: none;
  border: none;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
  white-space: nowrap;
  position: relative;
}

.nav-tab:hover {
  color: #3b82f6;
}

.nav-tab.active {
  color: #3b82f6;
  border-bottom: 2px solid #3b82f6;
}

.unread-badge {
  background-color: #ef4444;
  color: white;
  font-size: 0.75rem;
  padding: 0.25rem 0.5rem;
  border-radius: 9999px;
  min-width: 1.25rem;
  text-align: center;
}

.message-list {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.loading-state, .error-state {
  padding: 3rem;
  text-align: center;
  color: #6b7280;
}

.error-state {
  color: #ef4444;
}

.retry-btn {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
}

.message-item {
  display: flex;
  gap: 1rem;
  padding: 1.5rem;
  border-bottom: 1px solid #f3f4f6;
  transition: background-color 0.2s;
}

.message-item:hover {
  background-color: #f9fafb;
}

.message-item:last-child {
  border-bottom: none;
}

.message-icon {
  width: 3rem;
  height: 3rem;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  flex-shrink: 0;
}

.message-content {
  flex: 1;
  min-width: 0;
}

.message-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.message-text {
  color: #6b7280;
  line-height: 1.5;
  margin-bottom: 0.75rem;
}

.message-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.875rem;
  color: #9ca3af;
}

.message-actions {
  display: flex;
  gap: 0.5rem;
  align-items: flex-start;
  flex-shrink: 0;
}

.mark-read-btn, .reply-btn {
  padding: 0.5rem 1rem;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  font-size: 0.875rem;
  transition: background-color 0.2s;
}

.mark-read-btn:hover, .reply-btn:hover {
  background-color: #2563eb;
}

.delete-btn {
  padding: 0.5rem;
  background-color: #ef4444;
  color: white;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.delete-btn:hover {
  background-color: #dc2626;
}

.empty-state {
  padding: 3rem;
  text-align: center;
  color: #6b7280;
}

.empty-state i {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.reply-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 0.5rem;
  width: 90%;
  max-width: 500px;
  max-height: 80vh;
  overflow: hidden;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.modal-header h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #6b7280;
  cursor: pointer;
}

.modal-body {
  padding: 1.5rem;
}

.original-message {
  background-color: #f3f4f6;
  padding: 1rem;
  border-radius: 0.25rem;
  margin-bottom: 1rem;
  color: #6b7280;
}

.reply-textarea {
  width: 100%;
  min-height: 120px;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.25rem;
  resize: vertical;
}

.modal-footer {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  padding: 1.5rem;
  border-top: 1px solid #e5e7eb;
}

.cancel-btn {
  padding: 0.5rem 1rem;
  background-color: #6b7280;
  color: white;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
}

.send-btn {
  padding: 0.5rem 1rem;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
}
</style>
