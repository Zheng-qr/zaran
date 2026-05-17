<template>
  <button 
    @click="handleToggleLike" 
    :disabled="loading"
    class="like-btn"
    :class="{ 'liked': isLiked }"
  >
    <span v-if="loading" class="loading-spinner"></span>
    <i v-else :class="getIcon()"></i>
    {{ getText() }}
  </button>
</template>

<script setup lang="ts">
interface Props {
  targetId: string
  context?: 'article' | 'good'
}

const props = withDefaults(defineProps<Props>(), {
  context: 'article'
})
const { isLiked, loading, toggleLike } = useLikes(props.targetId, props.context)

const handleToggleLike = async () => {
  await toggleLike()
}

const getIcon = () => {
  return isLiked.value ? 'fas fa-heart' : 'far fa-heart'
}

const getText = () => {
  return isLiked.value ? '已收藏' : '收藏'
}
</script>

<style scoped>
.like-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  background: none;
  border: 1px solid #e2e8f0;
  padding: 8px 16px;
  border-radius: 20px;
  cursor: pointer;
  color: #718096;
  font-size: 0.9rem;
  transition: all 0.2s;
}

.like-btn:hover {
  border-color: #e53e3e;
  color: #e53e3e;
}

.like-btn.liked {
  border-color: #e53e3e;
  color: #e53e3e;
  background: #fed7d7;
}

.like-btn:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.loading-spinner {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid #f3f3f3;
  border-radius: 50%;
  border-top-color: #e53e3e;
  animation: spin 1s ease-in-out infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
