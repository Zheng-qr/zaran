/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArrayResultOfTransactionDetailResponse } from '../models/ArrayResultOfTransactionDetailResponse';
import type { TransactionDetailResponse } from '../models/TransactionDetailResponse';
import type { TransactionPatchEndpointRequest } from '../models/TransactionPatchEndpointRequest';
import type { TransactionPostEndpointRequest } from '../models/TransactionPostEndpointRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class TransactionsService {
    /**
     * @param id
     * @returns TransactionDetailResponse Success
     * @throws ApiError
     */
    public static transactionGetEndpoint(
        id: string,
    ): CancelablePromise<TransactionDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/transactions/{id}',
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
     * @param requestBody
     * @returns TransactionDetailResponse Success
     * @throws ApiError
     */
    public static transactionPatchEndpoint(
        id: string,
        requestBody: TransactionPatchEndpointRequest,
    ): CancelablePromise<TransactionDetailResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/transactions/{id}',
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
     * @returns ArrayResultOfTransactionDetailResponse Success
     * @throws ApiError
     */
    public static transactionListEndpoint(
        offset: number,
        limit: number,
        desc: boolean,
        keyword?: string | null,
    ): CancelablePromise<ArrayResultOfTransactionDetailResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/transactions',
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
    /**
     * @param requestBody
     * @returns TransactionDetailResponse Success
     * @throws ApiError
     */
    public static transactionPostEndpoint(
        requestBody: TransactionPostEndpointRequest,
    ): CancelablePromise<TransactionDetailResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/transactions',
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
