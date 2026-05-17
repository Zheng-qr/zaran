/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { UserDetailResponse } from '../models/UserDetailResponse';
import type { UserLoginRequest } from '../models/UserLoginRequest';
import type { UserLoginResponse } from '../models/UserLoginResponse';
import type { UserRegisterRequest } from '../models/UserRegisterRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class UserService {
    /**
     * @param requestBody
     * @returns UserLoginResponse Success
     * @throws ApiError
     */
    public static userLoginEndpoint(
        requestBody: UserLoginRequest,
    ): CancelablePromise<UserLoginResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/user/login',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param requestBody
     * @returns UserDetailResponse Success
     * @throws ApiError
     */
    public static userRegisterEndpoint(
        requestBody: UserRegisterRequest,
    ): CancelablePromise<UserDetailResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/user/register',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * @returns UserDetailResponse Success
     * @throws ApiError
     */
    public static userStatusEndpoint(): CancelablePromise<UserDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/user/status',
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }
}
