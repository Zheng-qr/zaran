/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { StaticUploadEndpointResponse } from '../models/StaticUploadEndpointResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class StaticService {
    /**
     * @param path
     * @returns void
     * @throws ApiError
     */
    public static staticGetEndpoint(
        path: string,
    ): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/static/{*path}',
            path: {
                '*path': path,
            },
        });
    }
    /**
     * @returns StaticUploadEndpointResponse Success
     * @throws ApiError
     */
    public static staticUploadEndpoint(): CancelablePromise<StaticUploadEndpointResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/static/upload',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }
}
