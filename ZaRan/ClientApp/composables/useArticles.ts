import { ref, computed, readonly } from 'vue'
import { ArticlesService, type ArticleDetailResponse, ArticleType } from '~/api'
import { useApi } from '~/composables/useApi'

export function useArticles(type?: string, userId?: string) {
  const articles = ref<ArticleDetailResponse[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const total = ref(0);

  // 文章类型映射
  const getArticleTypeString = (type?: string): string => {
    if (!type) return '0' // 默认为Story

    // 如果是"all"类型，直接返回
    if (type.toLowerCase() === 'all') {
      return 'all'
    }

    // 如果传入的是数字字符串，直接返回
    if (/^\d+$/.test(type)) {
      return type
    }

    // 如果传入的是类型名称，转换为数字
    const typeMap: Record<string, string> = {
      'story': '0',
      'character': '1',
      'gene': '2',
      'wiki': '3',
      'post': '4',
      'pattern': '5',
      'all': 'all'
    }

    return typeMap[type.toLowerCase()] || '0'
  }

  const fetchArticles = async (page: number = 1, pageSize: number = 12, keyword: string = '', targetUserId?: string) => {
    loading.value = true;
    error.value = null;

    try {
      const { executeApi, createPaginationParams } = useApi();
      const articleType = getArticleTypeString(type);
      const paginationParams = createPaginationParams(page, pageSize, true, keyword);

      // 使用传入的 targetUserId 或者初始化时的 userId
      const userIdToUse = targetUserId || userId;

      const response = await executeApi(
        () => ArticlesService.articleListEndpoint(
          articleType,
          paginationParams.offset,
          paginationParams.limit,
          paginationParams.desc,
          userIdToUse || null,
          paginationParams.keyword || null
        ),
        { showError: false }
      );

      if (response.error) {
        error.value = response.error;
        articles.value = [];
        total.value = 0;
      } else if (response.data) {
        articles.value = response.data.items;
        total.value = response.data.total;
      }

    } catch (err) {
      error.value = '加载文章失败，请稍后重试';
      console.error('获取文章失败:', err);
      articles.value = [];
      total.value = 0;
    } finally {
      loading.value = false;
    }
  };

  const getArticleById = async (id: string): Promise<ArticleDetailResponse | null> => {
    loading.value = true;
    error.value = null;

    try {
      const { executeApi } = useApi();

      const response = await executeApi(
        () => ArticlesService.articleGetEndpoint(id),
        { showError: false }
      );

      if (response.error) {
        error.value = response.error;
        return null;
      }

      return response.data;
    } catch (err) {
      error.value = '加载文章详情失败';
      console.error('获取文章详情失败:', err);
      return null;
    } finally {
      loading.value = false;
    }
  };

  // 计算总页数
  const totalPages = computed(() => {
    return Math.ceil(total.value / 12); // 默认每页12条
  });

  // 获取指定用户的文章
  const fetchUserArticles = async (targetUserId: string, page: number = 1, pageSize: number = 12, keyword: string = '') => {
    return fetchArticles(page, pageSize, keyword, targetUserId);
  };

  return {
    articles: readonly(articles),
    loading: readonly(loading),
    error: readonly(error),
    total: readonly(total),
    totalPages: readonly(totalPages),
    fetchArticles,
    fetchUserArticles,
    getArticleById
  };
}
