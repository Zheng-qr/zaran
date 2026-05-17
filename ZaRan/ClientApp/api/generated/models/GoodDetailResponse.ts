/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CollectionDetailResponse } from './CollectionDetailResponse';
import type { GoodStatus } from './GoodStatus';
import type { ResponseBase } from './ResponseBase';
import type { UserDetailResponse } from './UserDetailResponse';
export type GoodDetailResponse = (ResponseBase & {
    id?: string;
    name?: string;
    price?: number;
    stoke?: number;
    discountedPrice?: number;
    publisherName?: string | null;
    status?: GoodStatus;
    imageUrl?: string | null;
    copyrightInfo?: string | null;
    createdAt?: string;
    updatedAt?: string;
    publisher?: UserDetailResponse | null;
    collection?: CollectionDetailResponse | null;
    relatedArticleId?: string;
    publisherId?: string;
    collectionId?: string;
});

