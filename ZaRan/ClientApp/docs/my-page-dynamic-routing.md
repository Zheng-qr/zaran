# 我的页面 - 动态路由功能实现

## 功能概述

成功将 `my.vue` 页面升级为支持动态路由的用户主页系统，现在支持：
- `/my` - 查看自己的个人中心
- `/my/[id]` - 查看指定用户的主页

## 核心功能

### 1. 路由参数处理
```typescript
// 获取路由参数
const route = useRoute()
const userId = route.params.id as string | undefined

// 判断是否为自己的资料
const isOwnProfile = computed(() => {
  if (!userId) return true // 没有ID参数，默认为自己的资料
  return userId === userStore.userId
})
```

### 2. 动态页面标题
```typescript
useHead({
  title: computed(() => 
    isOwnProfile.value 
      ? '我的 - 檐下风铃' 
      : `${displayUser.value?.nickname || '用户'}的主页 - 檐下风铃`
  ),
  meta: [
    { name: 'description', content: computed(() => 
      isOwnProfile.value ? '个人中心页面' : '用户主页'
    )}
  ]
})
```

### 3. 用户数据加载
```typescript
const loadUserInfo = async () => {
  if (isOwnProfile.value) {
    // 自己的资料，使用userStore中的数据
    displayUser.value = userStore.user
  } else if (userId) {
    // 他人的资料，从API获取
    const response = await UsersService.userDetailEndpoint(userId)
    displayUser.value = response
    
    // 检查是否已关注
    if (userStore.isLoggedIn) {
      isFollowing.value = await checkRelationship('follow', userId, 'user')
    }
  }
}
```

## 界面差异

### 自己的资料页面
- ✅ 显示所有标签页：作品、收藏、订单、关注、设置
- ✅ 显示编辑资料、分享主页、账户设置按钮
- ✅ 可以查看私人信息（收藏、订单）
- ✅ 显示完整的统计数据

### 他人的资料页面
- ✅ 只显示公开标签页：作品、关注
- ✅ 显示关注/取消关注、发消息、分享主页按钮
- ❌ 不显示私人信息（收藏、订单、设置）
- ✅ 显示公开的统计数据

## 页面头部变化

### 自己的资料
```
我的账户
管理您的个人信息和作品
```

### 他人的资料
```
[用户昵称]的主页
查看用户的作品和信息
```

## 操作按钮差异

### 自己的资料
```vue
<button class="btn btn-primary" @click="showEditProfile = true">
  <i class="fas fa-edit"></i>编辑资料
</button>
<button class="btn btn-outline">
  <i class="fas fa-share-alt"></i>分享主页
</button>
<button class="btn btn-outline" @click="setActiveTab('settings')">
  <i class="fas fa-cog"></i>账户设置
</button>
```

### 他人的资料
```vue
<button class="btn btn-primary" @click="toggleFollow">
  <i :class="isFollowing ? 'fas fa-user-check' : 'fas fa-user-plus'"></i>
  {{ isFollowing ? '已关注' : '关注' }}
</button>
<button class="btn btn-outline">
  <i class="fas fa-envelope"></i>发消息
</button>
<button class="btn btn-outline">
  <i class="fas fa-share-alt"></i>分享主页
</button>
```

## 关注功能

### 关注状态检查
```typescript
// 检查是否已关注
if (userStore.isLoggedIn) {
  isFollowing.value = await checkRelationship('follow', userId, 'user')
}
```

### 切换关注状态
```typescript
const toggleFollow = async () => {
  if (!userId || !userStore.isLoggedIn) return
  
  try {
    const success = await toggleRelationship('follow', userId, 'user')
    if (success) {
      isFollowing.value = !isFollowing.value
      // 更新关注数统计
      if (isFollowing.value) {
        stats.value.followingCount++
      } else {
        stats.value.followingCount--
      }
    }
  } catch (error) {
    console.error('关注操作失败:', error)
  }
}
```

## 数据加载策略

### 作品数据
- 自己的资料：加载自己创建的所有作品
- 他人的资料：加载该用户的公开作品

### 收藏数据
- 自己的资料：加载自己的收藏列表
- 他人的资料：不加载（隐私保护）

### 订单数据
- 自己的资料：加载自己的订单记录
- 他人的资料：不加载（隐私保护）

### 关注数据
- 自己的资料：加载自己关注的用户
- 他人的资料：加载该用户关注的用户（如果公开）

## 路由监听

```typescript
// 监听路由参数变化
watch(() => route.params.id, async () => {
  await loadUserInfo()
  loadMyWorks()
  if (isOwnProfile.value) {
    loadMyFavorites()
    loadMyOrders()
  }
  loadMyFollowing()
  // 重置到作品标签页
  activeTab.value = 'works'
})
```

## 使用示例

### 访问自己的资料
```
http://localhost:3000/my
```

### 访问他人的资料
```
http://localhost:3000/my/user-001
http://localhost:3000/my/zhaoshewen
```

## 安全考虑

1. **隐私保护**: 他人无法查看私人信息（收藏、订单）
2. **权限控制**: 只有登录用户才能进行关注操作
3. **数据验证**: 验证用户ID的有效性
4. **错误处理**: 处理用户不存在的情况

## 测试建议

1. **功能测试**
   - 访问 `/my` 查看自己的资料
   - 访问 `/my/test-user-id` 查看他人资料
   - 测试关注/取消关注功能
   - 验证标签页显示差异

2. **权限测试**
   - 未登录状态下访问他人资料
   - 验证私人信息不会泄露
   - 测试关注功能的权限控制

3. **响应式测试**
   - 不同设备尺寸下的显示效果
   - 按钮布局的适配性

## 后续优化

1. **性能优化**: 实现数据缓存机制
2. **功能增强**: 添加用户搜索功能
3. **社交功能**: 实现私信系统
4. **统计分析**: 添加访问统计功能
