/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ResponseBase } from './ResponseBase';
import type { UserDetailResponse } from './UserDetailResponse';
export type CommentDetailResponse = (ResponseBase & {
    id?: string;
    content?: string;
    targetId?: string;
    createdAt?: string;
    sender?: UserDetailResponse;
});

