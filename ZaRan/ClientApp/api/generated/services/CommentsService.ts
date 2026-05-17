/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArrayResultOfCommentDetailResponse } from '../models/ArrayResultOfCommentDetailResponse';
import type { CommentDetailResponse } from '../models/CommentDetailResponse';
import type { CommentPatchEndpointRequest } from '../models/CommentPatchEndpointRequest';
import type { CommentPostEndpointRequest } from '../models/CommentPostEndpointRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CommentsService {
    /**
     * @param id
     * @returns void
     * @throws ApiError
     */
    public static commentDeleteEndpoint(
        id: string,
    ): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/comments/{id}',
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
     * @returns CommentDetailResponse Success
     * @throws ApiError
     */
    public static commentGetEndpoint(
        id: string,
    ): CancelablePromise<CommentDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/comments/{id}',
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
     * @returns CommentDetailResponse Success
     * @throws ApiError
     */
    public static commentPatchEndpoint(
        id: string,
        requestBody: CommentPatchEndpointRequest,
    ): CancelablePromise<CommentDetailResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/comments/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param targetId
     * @param offset
     * @param limit
     * @param desc
     * @returns ArrayResultOfCommentDetailResponse Success
     * @throws ApiError
     */
    public static commentListEndpoint(
        targetId: string,
        offset: number,
        limit: number,
        desc: boolean,
    ): CancelablePromise<ArrayResultOfCommentDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/comments/{targetId}',
            path: {
                'targetId': targetId,
            },
            query: {
                'offset': offset,
                'limit': limit,
                'desc': desc,
            },
        });
    }
    /**
     * @param requestBody
     * @returns CommentDetailResponse Success
     * @throws ApiError
     */
    public static commentPostEndpoint(
        requestBody: CommentPostEndpointRequest,
    ): CancelablePromise<CommentDetailResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/comments',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }
}
