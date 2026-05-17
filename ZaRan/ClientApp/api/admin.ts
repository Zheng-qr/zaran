import { OpenAPI } from './generated/core/OpenAPI';
import { request } from './generated/core/request';
import type { CancelablePromise } from './generated/core/CancelablePromise';
import type { ArrayResultOfUserDetailResponse } from './generated/models/ArrayResultOfUserDetailResponse';
import type { UserDetailResponse } from './generated/models/UserDetailResponse';
import type { ArrayResultOfArticleDetailResponse } from './generated/models/ArrayResultOfArticleDetailResponse';
import type { ArticleDetailResponse } from './generated/models/ArticleDetailResponse';
import type { ArrayResultOfGoodDetailResponse } from './generated/models/ArrayResultOfGoodDetailResponse';
import type { GoodDetailResponse } from './generated/models/GoodDetailResponse';

// 管理员API服务
export class AdminService {
  /**
   * 获取用户列表（管理员权限）
   */
  public static adminUserListEndpoint(
    offset: number,
    limit: number,
    desc: boolean,
    keyword?: string | null,
  ): CancelablePromise<ArrayResultOfUserDetailResponse> {
    return request(OpenAPI, {
      method: 'GET',
      url: '/api/admin/users',
      query: {
        'keyword': keyword,
        'offset': offset,
        'limit': limit,
        'desc': desc,
      },
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
      },
    });
  }

  /**
   * 更新用户角色（管理员权限）
   */
  public static adminUpdateUserRoleEndpoint(
    userId: string,
    requestBody: { userRole: number },
  ): CancelablePromise<UserDetailResponse> {
    return request(OpenAPI, {
      method: 'PATCH',
      url: '/api/admin/users/{userId}/role',
      path: {
        'userId': userId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 更新用户状态（管理员权限）
   */
  public static adminUpdateUserStatusEndpoint(
    userId: string,
    requestBody: { userStatus: number },
  ): CancelablePromise<UserDetailResponse> {
    return request(OpenAPI, {
      method: 'PATCH',
      url: '/api/admin/users/{userId}/status',
      path: {
        'userId': userId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 重置用户密码（管理员权限）
   */
  public static adminResetUserPasswordEndpoint(
    userId: string,
  ): CancelablePromise<{ newPassword: string }> {
    return request(OpenAPI, {
      method: 'POST',
      url: '/api/admin/users/{userId}/reset-password',
      path: {
        'userId': userId,
      },
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 获取文章列表（管理员权限）
   */
  public static adminArticleListEndpoint(
    offset: number,
    limit: number,
    desc: boolean,
    keyword?: string | null,
    userId?: string | null,
  ): CancelablePromise<ArrayResultOfArticleDetailResponse> {
    return request(OpenAPI, {
      method: 'GET',
      url: '/api/admin/articles',
      query: {
        'keyword': keyword,
        'offset': offset,
        'limit': limit,
        'desc': desc,
        'userId': userId,
      },
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
      },
    });
  }

  /**
   * 更新文章状态（管理员权限）
   */
  public static adminUpdateArticleStatusEndpoint(
    articleId: string,
    requestBody: { status: number },
  ): CancelablePromise<ArticleDetailResponse> {
    return request(OpenAPI, {
      method: 'PATCH',
      url: '/api/admin/articles/{articleId}/status',
      path: {
        'articleId': articleId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 删除文章（管理员权限）
   */
  public static adminDeleteArticleEndpoint(
    articleId: string,
  ): CancelablePromise<void> {
    return request(OpenAPI, {
      method: 'DELETE',
      url: '/api/admin/articles/{articleId}',
      path: {
        'articleId': articleId,
      },
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 审核不通过文章（管理员权限）
   */
  public static adminRejectArticleEndpoint(
    articleId: string,
    requestBody: { message: string },
  ): CancelablePromise<ArticleDetailResponse> {
    return request(OpenAPI, {
      method: 'POST',
      url: '/api/admin/articles/{articleId}/reject',
      path: {
        'articleId': articleId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 获取商品列表（管理员权限）
   */
  public static adminGoodListEndpoint(
    offset: number,
    limit: number,
    desc: boolean,
    keyword?: string | null,
    userId?: string | null,
  ): CancelablePromise<ArrayResultOfGoodDetailResponse> {
    return request(OpenAPI, {
      method: 'GET',
      url: '/api/admin/goods',
      query: {
        'keyword': keyword,
        'offset': offset,
        'limit': limit,
        'desc': desc,
        'userId': userId,
      },
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
      },
    });
  }

  /**
   * 更新商品状态（管理员权限）
   */
  public static adminUpdateGoodStatusEndpoint(
    goodId: string,
    requestBody: { status: number },
  ): CancelablePromise<GoodDetailResponse> {
    return request(OpenAPI, {
      method: 'PATCH',
      url: '/api/admin/goods/{goodId}/status',
      path: {
        'goodId': goodId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 删除商品（管理员权限）
   */
  public static adminDeleteGoodEndpoint(
    goodId: string,
  ): CancelablePromise<void> {
    return request(OpenAPI, {
      method: 'DELETE',
      url: '/api/admin/goods/{goodId}',
      path: {
        'goodId': goodId,
      },
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 审核不通过商品（管理员权限）
   */
  public static adminRejectGoodEndpoint(
    goodId: string,
    requestBody: { message: string },
  ): CancelablePromise<GoodDetailResponse> {
    return request(OpenAPI, {
      method: 'POST',
      url: '/api/admin/goods/{goodId}/reject',
      path: {
        'goodId': goodId,
      },
      body: requestBody,
      mediaType: 'application/json',
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
        404: `Not Found`,
      },
    });
  }

  /**
   * 获取系统统计信息（管理员权限）
   */
  public static adminGetStatisticsEndpoint(): CancelablePromise<{
    userCount: number;
    articleCount: number;
    goodCount: number;
    transactionCount: number;
    dailyActiveUsers: number;
  }> {
    return request(OpenAPI, {
      method: 'GET',
      url: '/api/admin/statistics',
      errors: {
        401: `Unauthorized`,
        403: `Forbidden`,
      },
    });
  }
}