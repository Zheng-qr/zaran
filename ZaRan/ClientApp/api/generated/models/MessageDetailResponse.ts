/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { MessageType } from './MessageType';
import type { ResponseBase } from './ResponseBase';
import type { UserDetailResponse } from './UserDetailResponse';
export type MessageDetailResponse = (ResponseBase & {
    id?: string;
    title?: string;
    content?: string;
    isRead?: boolean;
    type?: MessageType;
    createdAt?: string;
    sender?: UserDetailResponse;
    receiver?: UserDetailResponse;
});

