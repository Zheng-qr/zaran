/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArrayResultOfGoodCollectsListEndpointResponse } from '../models/ArrayResultOfGoodCollectsListEndpointResponse';
import type { ArrayResultOfGoodDetailResponse } from '../models/ArrayResultOfGoodDetailResponse';
import type { GoodDetailResponse } from '../models/GoodDetailResponse';
import type { GoodPatchEndpointRequest } from '../models/GoodPatchEndpointRequest';
import type { GoodPostEndpointRequest } from '../models/GoodPostEndpointRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class GoodsService {
    /**
     * @param goodId
     * @param offset
     * @param limit
     * @param desc
     * @returns ArrayResultOfGoodCollectsListEndpointResponse Success
     * @throws ApiError
     */
    public static goodCollectsListEndpoint(
        goodId: string,
        offset: number,
        limit: number,
        desc: boolean,
    ): CancelablePromise<ArrayResultOfGoodCollectsListEndpointResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/goods/{goodId}/collects',
            path: {
                'goodId': goodId,
            },
            query: {
                'offset': offset,
                'limit': limit,
                'desc': desc,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * @param id
     * @returns void
     * @throws ApiError
     */
    public static goodDeleteEndpoint(
        id: string,
    ): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/goods/{id}',
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
     * @returns GoodDetailResponse Success
     * @throws ApiError
     */
    public static goodGetEndpoint(
        id: string,
    ): CancelablePromise<GoodDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/goods/{id}',
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
     * @returns GoodDetailResponse Success
     * @throws ApiError
     */
    public static goodPatchEndpoint(
        id: string,
        requestBody: GoodPatchEndpointRequest,
    ): CancelablePromise<GoodDetailResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/goods/{id}',
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
     * @param offset
     * @param limit
     * @param desc
     * @param keyword
     * @returns ArrayResultOfGoodDetailResponse Success
     * @throws ApiError
     */
    public static goodListEndpoint(
        offset: number,
        limit: number,
        desc: boolean,
        keyword?: string | null,
    ): CancelablePromise<ArrayResultOfGoodDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/goods',
            query: {
                'keyword': keyword,
                'offset': offset,
                'limit': limit,
                'desc': desc,
            },
        });
    }
    /**
     * @param requestBody
     * @returns GoodDetailResponse Success
     * @throws ApiError
     */
    public static goodPostEndpoint(
        requestBody: GoodPostEndpointRequest,
    ): CancelablePromise<GoodDetailResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/goods',
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
