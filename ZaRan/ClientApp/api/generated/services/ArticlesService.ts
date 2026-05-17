/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArrayResultOfArticleDetailResponse } from '../models/ArrayResultOfArticleDetailResponse';
import type { ArticleDetailResponse } from '../models/ArticleDetailResponse';
import type { ArticlePatchEndpointRequest } from '../models/ArticlePatchEndpointRequest';
import type { ArticlePostEndpointRequest } from '../models/ArticlePostEndpointRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class ArticlesService {
    /**
     * @param id
     * @returns void
     * @throws ApiError
     */
    public static articleDeleteEndpoint(
        id: string,
    ): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/articles/{id}',
            path: {
                'id': id,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param id
     * @returns ArticleDetailResponse Success
     * @throws ApiError
     */
    public static articleGetEndpoint(
        id: string,
    ): CancelablePromise<ArticleDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/articles/{id}',
            path: {
                'id': id,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns ArticleDetailResponse Success
     * @throws ApiError
     */
    public static articlePatchEndpoint(
        id: string,
        requestBody: ArticlePatchEndpointRequest,
    ): CancelablePromise<ArticleDetailResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/articles/{id}',
            path: {
                'id': id,
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
     * @param type
     * @param offset
     * @param limit
     * @param desc
     * @param userId
     * @param keyword
     * @returns ArrayResultOfArticleDetailResponse Success
     * @throws ApiError
     */
    public static articleListEndpoint(
        type: string,
        offset: number,
        limit: number,
        desc: boolean,
        userId?: string | null,
        keyword?: string | null,
    ): CancelablePromise<ArrayResultOfArticleDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/articles/{type}',
            path: {
                'type': type,
            },
            query: {
                'userId': userId,
                'keyword': keyword,
                'offset': offset,
                'limit': limit,
                'desc': desc,
            },
        });
    }
    /**
     * @param requestBody
     * @returns ArticleDetailResponse Success
     * @throws ApiError
     */
    public static articlePostEndpoint(
        requestBody: ArticlePostEndpointRequest,
    ): CancelablePromise<ArticleDetailResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/articles',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
}
