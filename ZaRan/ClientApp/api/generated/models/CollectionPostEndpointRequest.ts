/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CollectionType } from './CollectionType';
export type CollectionPostEndpointRequest = {
    name: string;
    summary: string;
    description: string;
    color?: string | null;
    tags?: Array<string> | null;
    type?: CollectionType;
    childrenIds: Array<string>;
};

