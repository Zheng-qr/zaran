/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArrayResultOfCollectionDetailResponse } from '../models/ArrayResultOfCollectionDetailResponse';
import type { CollectionDetailResponse } from '../models/CollectionDetailResponse';
import type { CollectionPatchEndpointRequest } from '../models/CollectionPatchEndpointRequest';
import type { CollectionPostEndpointRequest } from '../models/CollectionPostEndpointRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CollectionsService {
    /**
     * @param id
     * @returns void
     * @throws ApiError
     */
    public static collectionDeleteEndpoint(
        id: string,
    ): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/collections/{id}',
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
     * @returns CollectionDetailResponse Success
     * @throws ApiError
     */
    public static collectionGetEndpoint(
        id: string,
    ): CancelablePromise<CollectionDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/collections/{id}',
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
     * @returns CollectionDetailResponse Success
     * @throws ApiError
     */
    public static collectionPatchEndpoint(
        id: string,
        requestBody: CollectionPatchEndpointRequest,
    ): CancelablePromise<CollectionDetailResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/collections/{id}',
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
     * @param id
     * @param offset
     * @param limit
     * @param desc
     * @returns any Success
     * @throws ApiError
     */
    public static collectionItemsEndpoint(
        id: string,
        offset: number,
        limit: number,
        desc: boolean,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/collections/{id}/items',
            path: {
                'id': id,
            },
            query: {
                'offset': offset,
                'limit': limit,
                'desc': desc,
            },
            errors: {
                400: `Bad Request`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param offset
     * @param limit
     * @param desc
     * @param type
     * @param creatorId
     * @param keyword
     * @returns ArrayResultOfCollectionDetailResponse Success
     * @throws ApiError
     */
    public static collectionListEndpoint(
        offset: number,
        limit: number,
        desc: boolean,
        type?: string | null,
        creatorId?: string | null,
        keyword?: string | null,
    ): CancelablePromise<ArrayResultOfCollectionDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/collections',
            query: {
                'type': type,
                'creatorId': creatorId,
                'keyword': keyword,
                'offset': offset,
                'limit': limit,
                'desc': desc,
            },
        });
    }
    /**
     * @param requestBody
     * @returns CollectionDetailResponse Success
     * @throws ApiError
     */
    public static collectionPostEndpoint(
        requestBody: CollectionPostEndpointRequest,
    ): CancelablePromise<CollectionDetailResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/collections',
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
