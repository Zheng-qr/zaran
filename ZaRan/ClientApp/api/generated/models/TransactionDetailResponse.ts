/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { GoodDetailResponse } from './GoodDetailResponse';
import type { ResponseBase } from './ResponseBase';
import type { TransactionStatus } from './TransactionStatus';
import type { UserDetailResponse } from './UserDetailResponse';
export type TransactionDetailResponse = (ResponseBase & {
    id?: string;
    price?: number;
    quantity?: number;
    description?: string | null;
    status?: TransactionStatus;
    createdAt?: string;
    updatedAt?: string;
    user?: UserDetailResponse | null;
    targetUser?: UserDetailResponse | null;
    good?: GoodDetailResponse | null;
});

