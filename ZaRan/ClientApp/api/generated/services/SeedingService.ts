/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class SeedingService {
    /**
     * @returns string Success
     * @throws ApiError
     */
    public static seedingEndpoint(): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/seeding',
            errors: {
                400: `Bad Request`,
            },
        });
    }
}
