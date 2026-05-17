/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArrayResultOfMessageDetailResponse } from '../models/ArrayResultOfMessageDetailResponse';
import type { MessageDetailResponse } from '../models/MessageDetailResponse';
import type { MessagePostEndpointRequest } from '../models/MessagePostEndpointRequest';
import type { MessageType } from '../models/MessageType';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class MessagesService {
    /**
     * @param id
     * @returns void
     * @throws ApiError
     */
    public static messageDeleteEndpoint(
        id: string,
    ): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/messages/{id}',
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
     * @returns MessageDetailResponse Success
     * @throws ApiError
     */
    public static messageGetEndpoint(
        id: string,
    ): CancelablePromise<MessageDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/messages/{id}',
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
     * @param offset
     * @param limit
     * @param desc
     * @param type
     * @param keyword
     * @returns ArrayResultOfMessageDetailResponse Success
     * @throws ApiError
     */
    public static messageListEndpoint(
        offset: number,
        limit: number,
        desc: boolean,
        type?: MessageType | null,
        keyword?: string | null,
    ): CancelablePromise<ArrayResultOfMessageDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/messages',
            query: {
                'type': type,
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
     * @param requestBody
     * @returns MessageDetailResponse Success
     * @throws ApiError
     */
    public static messagePostEndpoint(
        requestBody: MessagePostEndpointRequest,
    ): CancelablePromise<MessageDetailResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/messages',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }
    /**
     * @returns void
     * @throws ApiError
     */
    public static messageReadAllEndpoint(): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/messages/read-all',
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }
    /**
     * @param id
     * @returns void
     * @throws ApiError
     */
    public static messageReadEndpoint(
        id: string,
    ): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/messages/{id}/read',
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
}
