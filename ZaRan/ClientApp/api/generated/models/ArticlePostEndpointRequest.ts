/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ArticleType } from './ArticleType';
export type ArticlePostEndpointRequest = {
    title: string;
    body: string;
    summary?: string | null;
    tags: Array<string>;
    imageUrl?: string | null;
    color?: string | null;
    imageSmallUrl?: string | null;
    type?: ArticleType;
};

