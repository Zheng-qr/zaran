/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CollectionType } from './CollectionType';
import type { ResponseBase } from './ResponseBase';
import type { UserDetailResponse } from './UserDetailResponse';
export type CollectionDetailResponse = (ResponseBase & {
    id?: string;
    name?: string;
    summary?: string;
    description?: string;
    color?: string | null;
    tags?: Array<string> | null;
    type?: CollectionType;
    childrenIds?: Array<string>;
    creator?: UserDetailResponse | null;
    creatorId?: string;
});

