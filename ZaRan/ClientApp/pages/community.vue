<template>
    <div class="community-page">
        <!-- 页面头部 -->
        <div class="page-header">
            <div class="container">
                <h1>共创社区</h1>
                <p>与扎染爱好者交流创意，共同创作</p>
            </div>
        </div>

        <div class="page-content container">
            <div class="section">
                <!-- 标签导航 -->
                <div class="community-tabs">
                    <div v-for="tab in tabs" :key="tab.key" class="tab" :class="{ active: activeTab === tab.key }"
                        @click="switchTab(tab.key)">
                        {{ tab.label }}
                    </div>
                </div>

                <!-- 发帖区域 -->
                <div class="create-post">
                    <div class="post-editor">
                        <textarea v-model="newPostContent" class="post-input" placeholder="分享你的创意、问题或作品..."></textarea>
                        <div class="post-toolbar">
                            <div class="toolbar-left">
                                <button class="toolbar-button" title="添加图片">
                                    <i class="fas fa-image"></i>
                                </button>
                                <button class="toolbar-button" title="添加标签">
                                    <i class="fas fa-tag"></i>
                                </button>
                                <button class="toolbar-button" title="添加链接">
                                    <i class="fas fa-link"></i>
                                </button>
                                <button class="toolbar-button" title="添加代码">
                                    <i class="fas fa-code"></i>
                                </button>
                            </div>
                            <button class="post-submit" @click="submitPost">发布</button>
                        </div>
                    </div>
                </div>

                <!-- 加载状态 -->
                <div v-if="loading" class="loading-state">
                    <div class="spinner"></div>
                    <p>加载社区内容中...</p>
                </div>

                <!-- 错误状态 -->
                <div v-else-if="error" class="error-state">
                    <i class="fas fa-exclamation-triangle"></i>
                    <p>{{ error }}</p>
                    <button @click="loadCommunityData" class="retry-btn">重试</button>
                </div>

                <!-- 帖子列表 -->
                <div v-else class="posts-list">
                    <!-- 帖子1 - 作品展示 -->
                    <div class="post-card">
                        <div class="post-header">
                            <div class="post-avatar" style="background-image: url('/image/default-avatar.jpg')"></div>
                            <div class="post-user-info">
                                <div class="post-user">染艺达人</div>
                                <div class="post-meta">
                                    <span class="post-time">2小时前</span>
                                    <span class="post-tag">作品展示</span>
                                </div>
                            </div>
                        </div>
                        <div class="post-content">
                            刚刚完成了一组新的扎染作品，灵感来自传统云纹和现代几何的结合。使用了自贡扎染的捆扎技法和天然植物染料，整个制作过程耗时一周。大家觉得怎么样？欢迎交流创作心得！
                        </div>
                        <div class="post-images">
                            <div class="post-image" style="background-image: url('/image/IMG_9034.JPG')"></div>
                            <div class="post-image" style="background-image: url('/image/IMG_9055.JPG')"></div>
                            <div class="post-image" style="background-image: url('/image/IMG_9056.JPG')"></div>
                        </div>
                        <div class="post-actions">
                            <div class="post-action">
                                <i class="far fa-heart"></i>
                                <span>24</span>
                            </div>
                            <div class="post-action">
                                <i class="far fa-comment"></i>
                                <span>8</span>
                            </div>
                            <div class="post-action">
                                <i class="fas fa-share"></i>
                                <span>分享</span>
                            </div>
                            <div class="post-action">
                                <i class="far fa-bookmark"></i>
                                <span>收藏</span>
                            </div>
                        </div>
                    </div>

                    <!-- 帖子2 - 版权交易 -->
                    <div class="post-card">
                        <div class="post-header">
                            <div class="post-avatar" style="background-image: url('/image/default-avatar.jpg')"></div>
                            <div class="post-user-info">
                                <div class="post-user">纹样设计师</div>
                                <div class="post-meta">
                                    <span class="post-time">昨天</span>
                                    <span class="post-tag">版权交易</span>
                                </div>
                            </div>
                        </div>
                        <div class="post-content">
                            我正在寻找合作伙伴，共同开发一系列扎染纹样。这些纹样融合了传统元素和现代设计语言，适合应用于服装、家居等领域。有意者请联系我！
                        </div>
                        <div class="copyright-section">
                            <div class="copyright-title">版权交易信息</div>
                            <p>授权类型: 商用授权</p>
                            <p>授权范围: 服装、家居用品</p>
                            <p>授权期限: 2年</p>
                            <div class="copyright-buttons">
                                <button class="btn btn-outline">我要版权</button>
                                <button class="btn btn-primary">我卖版权</button>
                            </div>
                        </div>
                        <div class="post-actions">
                            <div class="post-action">
                                <i class="far fa-heart"></i>
                                <span>12</span>
                            </div>
                            <div class="post-action">
                                <i class="far fa-comment"></i>
                                <span>5</span>
                            </div>
                            <div class="post-action">
                                <i class="fas fa-share"></i>
                                <span>分享</span>
                            </div>
                        </div>
                    </div>

                    <!-- 热门话题 -->
                    <div class="topics-section">
                        <h3>热门话题</h3>
                        <div class="topics-grid">
                            <div class="topic-card" v-for="topic in hotTopics" :key="topic.name"
                                @click="searchTopic(topic.name)">
                                <div class="topic-name">{{ topic.name }}</div>
                                <div class="topic-posts">{{ topic.posts }}篇讨论</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
// 页面元数据
useHead({
    title: '共创社区 - 檐下风铃',
    meta: [
        { name: 'description', content: '扎染爱好者交流社区' }
    ]
})

// 响应式数据
const loading = ref(false)
const error = ref('')
const newPostContent = ref('')
const activeTab = ref('all')

// 标签配置
const tabs = [
    { key: 'all', label: '全部' },
    { key: 'hot', label: '热门' },
    { key: 'latest', label: '最新' },
    { key: 'following', label: '关注' },
    { key: 'qa', label: '问答' },
    { key: 'tutorial', label: '教程' },
    { key: 'showcase', label: '作品展示' },
    { key: 'copyright', label: '版权交易' },
    { key: 'activity', label: '活动' }
]

// 热门话题数据
const hotTopics = ref([
    { name: '#自贡扎染技法', posts: 128 },
    { name: '#植物染料配方', posts: 76 },
    { name: '#纹样设计大赛', posts: 53 },
    { name: '#扎染作品展示', posts: 210 }
])

// 切换标签
const switchTab = (tabKey: string) => {
    activeTab.value = tabKey
    // 这里可以根据不同标签加载不同内容
    console.log('切换到标签:', tabKey)
}

// 发布帖子
const submitPost = () => {
    if (!newPostContent.value.trim()) {
        alert('请输入帖子内容')
        return
    }

    // 这里应该调用API发布帖子
    console.log('发布帖子:', newPostContent.value)
    newPostContent.value = ''
    alert('帖子发布成功！')
}

// 搜索话题
const searchTopic = (topicName: string) => {
    console.log('搜索话题:', topicName)
    // 这里可以跳转到话题页面或筛选相关帖子
}

// 加载社区数据
const loadCommunityData = async () => {
    loading.value = true
    try {
        // 这里应该调用实际的API
        await new Promise(resolve => setTimeout(resolve, 1000))
        loading.value = false
    } catch (err) {
        error.value = '加载失败，请重试'
        loading.value = false
    }
}

// 页面加载时获取数据
onMounted(() => {
    loadCommunityData()
})
</script>

<style scoped>
/* 页面头部 */
.page-header {
    background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
    color: white;
    padding: 4rem 0;
    text-align: center;
    margin-bottom: 2rem;
}

.page-header h1 {
    color: white;
    font-size: 2.5rem;
    margin-bottom: 0.5rem;
}

/* 内容区域 */
.page-content {
    padding: 2rem 0;
}

.section {
    margin-bottom: 3rem;
    background: white;
    border-radius: var(--border-radius);
    box-shadow: var(--box-shadow);
    padding: 1.5rem;
}

/* 标签导航 */
.community-tabs {
    display: flex;
    border-bottom: 1px solid var(--medium-gray-color);
    margin-bottom: 1.5rem;
    overflow-x: auto;
    scrollbar-width: none;
}

.community-tabs::-webkit-scrollbar {
    display: none;
}

.tab {
    padding: 0.75rem 1.5rem;
    cursor: pointer;
    border-bottom: 3px solid transparent;
    white-space: nowrap;
    font-size: 0.95rem;
    transition: all 0.3s ease;
}

.tab:hover {
    color: var(--primary-color);
}

.tab.active {
    border-bottom-color: var(--primary-color);
    font-weight: 600;
    color: var(--primary-color);
}

/* 发帖区域 */
.create-post {
    margin-bottom: 2rem;
}

.post-editor {
    border: 1px solid var(--medium-gray-color);
    border-radius: var(--border-radius);
    overflow: hidden;
}

.post-input {
    width: 100%;
    padding: 1rem;
    border: none;
    resize: vertical;
    min-height: 100px;
    font-family: inherit;
    font-size: 1rem;
}

.post-input:focus {
    outline: none;
    box-shadow: 0 0 0 2px rgba(58, 123, 213, 0.2);
}

.post-input::placeholder {
    color: var(--dark-gray-color);
    opacity: 0.7;
}

.post-toolbar {
    display: flex;
    justify-content: space-between;
    padding: 0.75rem 1rem;
    border-top: 1px solid var(--medium-gray-color);
    background-color: var(--light-color);
}

.toolbar-left {
    display: flex;
    gap: 0.75rem;
}

.toolbar-button {
    background: none;
    border: none;
    color: var(--dark-gray-color);
    cursor: pointer;
    font-size: 1.1rem;
    transition: color 0.3s;
    padding: 0.25rem;
    border-radius: 4px;
}

.toolbar-button:hover {
    color: var(--primary-color);
    background-color: rgba(58, 123, 213, 0.1);
}

.post-submit {
    padding: 0.5rem 1.25rem;
    background-color: var(--primary-color);
    color: white;
    border: none;
    border-radius: var(--border-radius);
    font-weight: 500;
    cursor: pointer;
    transition: all 0.3s;
}

.post-submit:hover {
    background-color: #2a68c4;
    transform: translateY(-1px);
}

.post-submit:active {
    transform: translateY(0);
}

/* 帖子列表 */
.posts-list {
    display: grid;
    gap: 1.5rem;
}

.post-card {
    border: 1px solid var(--medium-gray-color);
    border-radius: var(--border-radius);
    padding: 1.5rem;
    transition: box-shadow 0.3s;
}

.post-card:hover {
    box-shadow: 0 6px 16px rgba(0, 0, 0, 0.1);
}

.post-header {
    display: flex;
    align-items: center;
    margin-bottom: 1rem;
}

.post-avatar {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    background-color: var(--light-color);
    margin-right: 1rem;
    background-size: cover;
    flex-shrink: 0;
    border: 2px solid white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.post-user-info {
    flex: 1;
}

.post-user {
    font-weight: 600;
    margin-bottom: 0.25rem;
    color: var(--dark-color);
}

.post-meta {
    display: flex;
    align-items: center;
    color: var(--dark-gray-color);
    font-size: 0.85rem;
    flex-wrap: wrap;
    gap: 0.5rem;
}

.post-time {
    margin-right: 0.75rem;
}

.post-tag {
    background-color: var(--light-color);
    color: var(--primary-color);
    padding: 0.25rem 0.5rem;
    border-radius: var(--border-radius);
    font-size: 0.75rem;
    font-weight: 500;
    display: inline-block;
}

.post-content {
    margin-bottom: 1rem;
    line-height: 1.6;
    white-space: pre-line;
}

.post-images {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 0.75rem;
    margin-bottom: 1rem;
}

.post-image {
    height: 180px;
    background-color: var(--light-color);
    background-size: cover;
    background-position: center;
    border-radius: var(--border-radius);
    cursor: pointer;
    transition: transform 0.3s;
}

.post-image:hover {
    transform: scale(1.02);
}

.post-actions {
    display: flex;
    gap: 1.5rem;
    padding-top: 1rem;
    border-top: 1px solid var(--light-color);
}

.post-action {
    display: flex;
    align-items: center;
    color: var(--dark-gray-color);
    cursor: pointer;
    transition: all 0.3s;
    padding: 0.25rem 0.5rem;
    border-radius: 4px;
}

.post-action:hover {
    color: var(--primary-color);
    background-color: rgba(58, 123, 213, 0.1);
}

.post-action i {
    margin-right: 0.5rem;
    font-size: 1.1rem;
}

.post-action.active {
    color: var(--accent-color-2);
}

/* 版权交易专区 */
.copyright-section {
    background-color: rgba(58, 123, 213, 0.05);
    border-left: 3px solid var(--primary-color);
    padding: 1rem;
    margin-bottom: 1.5rem;
    border-radius: 0 var(--border-radius) var(--border-radius) 0;
}

.copyright-title {
    font-weight: 600;
    margin-bottom: 0.5rem;
    color: var(--primary-color);
}

.copyright-buttons {
    display: flex;
    gap: 1rem;
    margin-top: 1rem;
}

.btn {
    padding: 0.5rem 1rem;
    border-radius: var(--border-radius);
    font-weight: 500;
    cursor: pointer;
    transition: all 0.3s;
}

.btn-outline {
    border: 1px solid var(--primary-color);
    background-color: transparent;
    color: var(--primary-color);
}

.btn-outline:hover {
    background-color: rgba(58, 123, 213, 0.1);
}

.btn-primary {
    background-color: var(--primary-color);
    color: white;
    border: none;
}

.btn-primary:hover {
    background-color: #2a68c4;
}

/* 热门话题 */
.topics-section {
    margin-top: 2rem;
}

.topics-section h3 {
    margin-bottom: 1rem;
    padding-bottom: 0.5rem;
    border-bottom: 1px solid var(--light-color);
}

.topics-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 1rem;
}

.topic-card {
    padding: 1rem;
    background-color: var(--light-color);
    border-radius: var(--border-radius);
    transition: all 0.3s;
    cursor: pointer;
}

.topic-card:hover {
    transform: translateY(-3px);
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.topic-name {
    font-weight: 600;
    margin-bottom: 0.5rem;
    color: var(--primary-color);
}

.topic-posts {
    font-size: 0.85rem;
    color: var(--dark-gray-color);
}

/* 加载和错误状态 */
.loading-state,
.error-state {
    text-align: center;
    padding: 4rem 0;
}

.spinner {
    display: inline-block;
    width: 40px;
    height: 40px;
    border: 4px solid var(--light-color);
    border-radius: 50%;
    border-top-color: var(--primary-color);
    animation: spin 1s ease-in-out infinite;
    margin-bottom: 1rem;
}

@keyframes spin {
    to {
        transform: rotate(360deg);
    }
}

.error-state {
    color: var(--danger-color);
}

.retry-btn {
    background: var(--primary-color);
    color: white;
    border: none;
    padding: 0.75rem 1.5rem;
    border-radius: var(--border-radius);
    cursor: pointer;
    margin-top: 1rem;
    font-weight: 500;
    transition: all 0.3s ease;
}

.retry-btn:hover {
    background: #065bb5;
    transform: translateY(-1px);
}

/* 响应式设计 */
@media (max-width: 768px) {
    .page-header {
        padding: 3rem 0;
    }

    .page-header h1 {
        font-size: 2rem;
    }

    .community-tabs {
        padding-bottom: 0.5rem;
    }

    .tab {
        padding: 0.75rem 1rem;
        font-size: 0.9rem;
    }

    .post-images {
        grid-template-columns: 1fr;
    }

    .post-image {
        height: 150px;
    }

    .topics-grid {
        grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    }

    .post-actions {
        gap: 1rem;
    }
}

@media (max-width: 480px) {
    .page-header {
        padding: 2rem 0;
    }

    .post-card {
        padding: 1rem;
    }

    .post-avatar {
        width: 40px;
        height: 40px;
    }

    .post-actions {
        gap: 0.75rem;
        justify-content: space-between;
    }

    .post-action span {
        display: none;
    }

    .post-action i {
        margin-right: 0;
        font-size: 1.2rem;
    }

    .copyright-buttons {
        flex-direction: column;
        gap: 0.5rem;
    }
}

/* 讨论区样式 */
.discussion-list {
    display: grid;
    gap: 1.5rem;
}

.discussion-item {
    display: flex;
    gap: 1rem;
    padding: 1.5rem;
    background: white;
    border-radius: var(--border-radius);
    box-shadow: var(--box-shadow);
    transition: all 0.3s ease;
    cursor: pointer;
}

.discussion-item:hover {
    transform: translateY(-4px);
    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.12);
}

.discussion-avatar {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    overflow: hidden;
    flex-shrink: 0;
}

.discussion-avatar img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.discussion-content {
    flex: 1;
}

.discussion-title {
    font-size: 1.1rem;
    font-weight: 600;
    margin-bottom: 0.5rem;
    color: var(--dark-color);
    line-height: 1.4;
}

.discussion-preview {
    color: var(--dark-gray-color);
    margin-bottom: 0.75rem;
    line-height: 1.6;
    font-size: 0.95rem;
}

.discussion-meta {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    font-size: 0.85rem;
    color: var(--medium-gray-color);
}

.discussion-meta span {
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

/* 活动公告样式 */
.events-list {
    display: grid;
    gap: 1.5rem;
}

.event-card {
    display: flex;
    gap: 1.5rem;
    padding: 1.5rem;
    background: white;
    border-radius: var(--border-radius);
    box-shadow: var(--box-shadow);
    transition: all 0.3s ease;
    cursor: pointer;
}

.event-card:hover {
    transform: translateY(-4px);
    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.12);
}

.event-date {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 70px;
    height: 70px;
    background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
    color: white;
    border-radius: var(--border-radius);
    flex-shrink: 0;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.event-date .day {
    font-size: 1.4rem;
    font-weight: 700;
    line-height: 1;
}

.event-date .month {
    font-size: 0.8rem;
    opacity: 0.9;
}

.event-info {
    flex: 1;
}

.event-title {
    font-size: 1.1rem;
    font-weight: 600;
    margin-bottom: 0.5rem;
    color: var(--dark-color);
    line-height: 1.4;
}

.event-description {
    color: var(--dark-gray-color);
    margin-bottom: 0.75rem;
    line-height: 1.6;
    font-size: 0.95rem;
}

.event-meta {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    font-size: 0.85rem;
    color: var(--medium-gray-color);
}

.event-meta span {
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

/* 响应式设计 */
@media (max-width: 768px) {
    .community-page {
        padding: 1rem 0;
    }

    .community-sections {
        gap: 2rem;
    }

    .section-title {
        font-size: 1.3rem;
        margin-bottom: 1.5rem;
    }

    .works-grid {
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 1.5rem;
    }

    .work-info {
        padding: 1rem;
    }

    .discussion-item {
        padding: 1rem;
        flex-direction: column;
        gap: 0.75rem;
    }

    .discussion-avatar {
        align-self: flex-start;
        width: 40px;
        height: 40px;
    }

    .discussion-meta {
        gap: 0.75rem;
    }

    .event-card {
        padding: 1rem;
        flex-direction: column;
        gap: 1rem;
    }

    .event-date {
        align-self: flex-start;
        width: 60px;
        height: 60px;
    }

    .event-date .day {
        font-size: 1.2rem;
    }

    .event-meta {
        gap: 0.75rem;
    }
}

@media (max-width: 480px) {
    .community-page {
        padding: 0.5rem 0;
    }

    .works-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }

    .discussion-item,
    .event-card {
        padding: 0.75rem;
    }

    .section-title {
        font-size: 1.2rem;
        margin-bottom: 1rem;
    }

    .discussion-meta,
    .event-meta {
        flex-direction: column;
        gap: 0.5rem;
    }
}
</style>
