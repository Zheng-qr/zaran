import { ref, computed, readonly } from 'vue'
import { ArticlesService, type ArticleDetailResponse } from '~/api'
import { useApi } from '~/composables/useApi'

// 基因库纹样接口定义
export interface GenePattern {
  id: string
  name: string
  description: string
  image: string
  category: string
  origin: string
  technique: string
  difficulty: string
  downloads: number
  likes: number
  tags: string[]
  color?: string
  createdAt: string
  updatedAt: string
  author?: {
    id: string
    userName: string
  }
}

// 分类定义
export interface PatternCategory {
  id: string
  name: string
}

export function useGeneLibrary() {
  const patterns = ref<GenePattern[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const total = ref(0)
  const currentPage = ref(1)
  const pageSize = ref(12)

  // 分类数据
  const categories = ref<PatternCategory[]>([
    { id: 'all', name: '全部' },
    { id: 'traditional', name: '传统纹样' },
    { id: 'modern', name: '现代创新' },
    { id: 'geometric', name: '几何图案' },
    { id: 'floral', name: '花卉纹样' },
    { id: 'abstract', name: '抽象艺术' }
  ])

  const activeCategory = ref('all')
  const searchKeyword = ref('')
  const selectedPattern = ref<GenePattern | null>(null)
  const downloading = ref(false)

  // 将后端 Article 数据转换为 GenePattern 格式
  const convertArticleToPattern = (article: ArticleDetailResponse): GenePattern => {
    // 从 tags 中提取分类、起源、工艺、难度等信息
    const tags = article.tags || []

    // 根据标签内容推断分类
    let category = '传统纹样'
    if (tags.some(tag => tag.includes('几何') || tag.includes('现代') || tag.includes('创新'))) {
      category = '几何图案'
    } else if (tags.some(tag => tag.includes('花卉') || tag.includes('花朵'))) {
      category = '花卉纹样'
    } else if (tags.some(tag => tag.includes('抽象'))) {
      category = '抽象艺术'
    } else if (tags.some(tag => tag.includes('现代') || tag.includes('创新'))) {
      category = '现代创新'
    }

    // 根据文章内容推断起源
    const origin = tags.find(tag => tag.includes('云南') || tag.includes('大理'))?.replace('云南', '云南大理') ||
                  (tags.some(tag => tag.includes('现代') || tag.includes('创新')) ? '现代设计' : '传统工艺')

    // 根据标签推断技法
    let technique = '绞缬'
    if (tags.some(tag => tag.includes('浸染'))) {
      technique = '浸染'
    } else if (tags.some(tag => tag.includes('夹缬'))) {
      technique = '夹缬'
    } else if (tags.some(tag => tag.includes('蜡染'))) {
      technique = '蜡染'
    }

    // 根据复杂度推断难度
    let difficulty = '中等'
    if (tags.some(tag => tag.includes('入门') || tag.includes('简单') || tag.includes('基础'))) {
      difficulty = '简单'
    } else if (tags.some(tag => tag.includes('高级') || tag.includes('复杂') || tag.includes('困难'))) {
      difficulty = '困难'
    }

    return {
      id: article.id || '',
      name: article.title || '未命名纹样',
      description: article.summary || (article.body ? `${article.body.substring(0, 100)}...` : '暂无描述'),
      image: article.imageUrl || '/image/default-pattern.jpg',
      category,
      origin,
      technique,
      difficulty,
      downloads: Math.floor(Math.random() * 1000) + 100, // 模拟下载数
      likes: Math.floor(Math.random() * 500) + 50, // 模拟点赞数
      tags: article.tags || [],
      color: article.color,
      createdAt: article.createdAt || '',
      updatedAt: article.updatedAt || '',
      author: article.author ? {
        id: article.author.id || '',
        userName: article.author.userName || '未知作者'
      } : undefined
    }
  }

  // 获取基因库数据
  const fetchPatterns = async (page: number = 1, keyword: string = '') => {
    loading.value = true
    error.value = null

    try {
      const { executeApi, createPaginationParams } = useApi()
      const paginationParams = createPaginationParams(page, pageSize.value, true, keyword)

      // 使用 Gene 类型 (ArticleType.Gene = 2)
      const response = await executeApi(
        () => ArticlesService.articleListEndpoint(
          '2', // Gene 类型
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc,
          null, // userId - 不筛选特定用户
          paginationParams.keyword || null
        ),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        patterns.value = []
        total.value = 0
      } else if (response.data) {
        // 转换数据格式
        const convertedPatterns = response.data.items.map(convertArticleToPattern)

        patterns.value = convertedPatterns
        total.value = response.data.total
        currentPage.value = page
      }
    } catch (err) {
      error.value = '加载基因库数据失败，请稍后重试'
      console.error('获取基因库数据失败:', err)
      patterns.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  // 获取单个纹样详情
  const getPatternById = async (id: string): Promise<GenePattern | null> => {
    loading.value = true
    error.value = null

    try {
      const { executeApi } = useApi()

      const response = await executeApi(
        () => ArticlesService.articleGetEndpoint(id),
        { showError: false }
      )

      if (response.error) {
        error.value = response.error
        return null
      }

      if (response.data) {
        return convertArticleToPattern(response.data)
      }

      return null
    } catch (err) {
      error.value = '加载纹样详情失败'
      console.error('获取纹样详情失败:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // 搜索纹样
  const searchPatterns = async (keyword: string) => {
    searchKeyword.value = keyword
    await fetchPatterns(1, keyword)
  }

  // 查看纹样详情
  const viewPatternDetail = (pattern: GenePattern) => {
    selectedPattern.value = pattern
  }

  // 关闭详情模态框
  const closeModal = () => {
    selectedPattern.value = null
  }

  // 下载纹样
  const downloadPattern = async (pattern: GenePattern) => {
    if (downloading.value) {
      return // 防止重复下载
    }

    downloading.value = true

    try {
      // 显示下载开始提示
      console.log('开始下载纹样:', pattern.name)

      // 获取图片URL
      const imageUrl = pattern.image

      // 如果是相对路径，转换为绝对路径
      const fullImageUrl = imageUrl.startsWith('http')
        ? imageUrl
        : `http://localhost:5127${imageUrl}`

      // 获取图片数据
      const response = await fetch(fullImageUrl)
      if (!response.ok) {
        throw new Error(`下载失败: ${response.status} ${response.statusText}`)
      }

      // 转换为 Blob
      const blob = await response.blob()

      // 创建下载链接
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url

      // 设置文件名
      const fileExtension = getFileExtension(imageUrl) || 'jpg'
      const fileName = `${pattern.name.replace(/[^\w\s-]/g, '')}_纹样.${fileExtension}`
      link.download = fileName

      // 触发下载
      document.body.appendChild(link)
      link.click()

      // 清理
      document.body.removeChild(link)
      window.URL.revokeObjectURL(url)

      console.log('下载完成:', fileName)

      // 显示成功提示
      if (process.client) {
        // 可以使用 toast 通知或其他方式
        console.log(`✅ "${pattern.name}" 下载成功！`)
      }

      // 这里可以添加下载统计逻辑
      // await updateDownloadCount(pattern.id)

    } catch (error) {
      console.error('下载失败:', error)

      // 显示错误提示
      if (process.client) {
        alert(`下载失败: ${error.message}`)
      }
    } finally {
      downloading.value = false
    }
  }

  // 获取文件扩展名
  const getFileExtension = (url: string): string => {
    const match = url.match(/\.([^.?]+)(\?|$)/)
    return match ? match[1].toLowerCase() : 'jpg'
  }

  // 在工坊中使用纹样
  const useInWorkshop = (pattern: GenePattern) => {
    navigateTo(`/workshop?pattern=${pattern.id}`)
  }

  // 计算属性
  const totalPages = computed(() => {
    return Math.ceil(total.value / pageSize.value)
  })

  const filteredPatterns = computed(() => {
    return patterns.value
  })

  // 初始化时加载数据
  onMounted(() => {
    if (process.client) {
      fetchPatterns()
    }
  })

  return {
    // 数据
    patterns: readonly(patterns),
    loading: readonly(loading),
    error: readonly(error),
    total: readonly(total),
    currentPage: readonly(currentPage),
    pageSize: readonly(pageSize),
    categories: readonly(categories),
    activeCategory: readonly(activeCategory),
    searchKeyword,
    selectedPattern: readonly(selectedPattern),
    downloading: readonly(downloading),
    totalPages,
    filteredPatterns,

    // 方法
    fetchPatterns,
    getPatternById,
    searchPatterns,
    viewPatternDetail,
    closeModal,
    downloadPattern,
    useInWorkshop
  }
}
