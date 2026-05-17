<template>
  <div v-if="hasAccess">
    <slot />
  </div>
  <div v-else-if="showFallback">
    <slot name="fallback">
      <div class="text-gray-500 text-sm">
        {{ fallbackMessage || '您没有权限查看此内容' }}
      </div>
    </slot>
  </div>
</template>

<script setup lang="ts">
import type { Permission } from '~/composables/usePermissions'

interface Props {
  // Permission object to check
  permission?: Permission
  // Required role (number)
  role?: number
  // Required status array
  status?: number[]
  // Whether to show fallback content when access is denied
  showFallback?: boolean
  // Custom fallback message
  fallbackMessage?: string
  // Require user to be logged in
  requireAuth?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showFallback: false,
  requireAuth: true
})

const userStore = useUserStore()
const { hasPermission, hasRole, hasStatus } = usePermissions()

const hasAccess = computed(() => {
  // Check if authentication is required
  if (props.requireAuth && !userStore.isLoggedIn) {
    return false
  }

  // Check specific permission if provided
  if (props.permission) {
    return hasPermission(props.permission)
  }

  // Check role if provided
  if (props.role !== undefined) {
    if (!hasRole(props.role)) {
      return false
    }
  }

  // Check status if provided
  if (props.status && props.status.length > 0) {
    if (!hasStatus(props.status)) {
      return false
    }
  }

  return true
})
</script>
