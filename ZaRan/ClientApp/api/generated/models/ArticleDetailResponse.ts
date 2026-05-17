/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArticleStatus } from './ArticleStatus';
import type { ArticleType } from './ArticleType';
import type { ResponseBase } from './ResponseBase';
import type { UserDetailResponse } from './UserDetailResponse';
export type ArticleDetailResponse = (ResponseBase & {
    id?: string;
    title?: string | null;
    status?: ArticleStatus;
    type?: ArticleType;
    body?: string | null;
    summary?: string | null;
    tags?: Array<string>;
    imageUrl?: string | null;
    imageSmallUrl?: string | null;
    createdAt?: string;
    updatedAt?: string;
    author?: UserDetailResponse | null;
    color?: string | null;
});

