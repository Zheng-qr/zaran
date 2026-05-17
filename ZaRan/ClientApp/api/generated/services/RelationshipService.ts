/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class RelationshipService {
    /**
     * @param userId
     * @param targetType
     * @param targetId
     * @returns any Created
     * @throws ApiError
     */
    public static userRelationshipAddEndpoint(
        userId: string,
        targetType: string,
        targetId: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/relationship/{userId}/{targetType}/{targetId}',
            path: {
                'userId': userId,
                'targetType': targetType,
                'targetId': targetId,
            },
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param userId
     * @param targetType
     * @param targetId
     * @returns any Created
     * @throws ApiError
     */
    public static userRelationshipDeleteEndpoint(
        userId: string,
        targetType: string,
        targetId: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/relationship/{userId}/{targetType}/{targetId}',
            path: {
                'userId': userId,
                'targetType': targetType,
                'targetId': targetId,
            },
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param typeId
     * @param userId
     * @param target
     * @param offset
     * @param limit
     * @param desc
     * @returns any Success
     * @throws ApiError
     */
    public static userRelationshipGetEndpoint(
        typeId: string,
        userId: string,
        target: boolean,
        offset: number,
        limit: number,
        desc: boolean,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/relationship/{typeId}/{userId}',
            path: {
                'typeId': typeId,
                'userId': userId,
            },
            query: {
                'target': target,
                'offset': offset,
                'limit': limit,
                'desc': desc,
            },
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
}
