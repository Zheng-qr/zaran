/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ResponseBase } from './ResponseBase';
import type { UserRole } from './UserRole';
import type { UserStatus } from './UserStatus';
export type UserDetailResponse = (ResponseBase & {
    id?: string;
    username?: string;
    nickname?: string;
    signature?: string | null;
    email?: string | null;
    userRole?: UserRole;
    userStatus?: UserStatus;
    unbanTime?: string | null;
    balance?: number;
    createdAt?: string;
    lastLoginAt?: string;
    avatar?: string | null;
});

