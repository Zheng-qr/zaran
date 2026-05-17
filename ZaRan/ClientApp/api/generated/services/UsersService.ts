/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArrayResultOfUserDetailResponse } from '../models/ArrayResultOfUserDetailResponse';
import type { UserAvatarUploadResponse } from '../models/UserAvatarUploadResponse';
import type { UserDetailResponse } from '../models/UserDetailResponse';
import type { UserPatchRequest } from '../models/UserPatchRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class UsersService {
    /**
     * @param userId
     * @returns UserAvatarUploadResponse Success
     * @throws ApiError
     */
    public static userAvatarUploadEndpoint(
        userId: string,
    ): CancelablePromise<UserAvatarUploadResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/users/{userId}/avatar',
            path: {
                'userId': userId,
            },
            errors: {
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param userId
     * @returns UserDetailResponse Success
     * @throws ApiError
     */
    public static userDetailEndpoint(
        userId: string,
    ): CancelablePromise<UserDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/users/{userId}',
            path: {
                'userId': userId,
            },
            errors: {
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param userId
     * @param requestBody
     * @returns UserDetailResponse Success
     * @throws ApiError
     */
    public static userPatchEndpoint(
        userId: string,
        requestBody: UserPatchRequest,
    ): CancelablePromise<UserDetailResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/users/{userId}',
            path: {
                'userId': userId,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param offset
     * @param limit
     * @param desc
     * @param keyword
     * @returns ArrayResultOfUserDetailResponse Success
     * @throws ApiError
     */
    public static userListEndpoint(
        offset: number,
        limit: number,
        desc: boolean,
        keyword?: string | null,
    ): CancelablePromise<ArrayResultOfUserDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/users',
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
}
