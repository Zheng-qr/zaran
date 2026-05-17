/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { UserRole } from './UserRole';
import type { UserStatus } from './UserStatus';
export type UserPatchRequest = {
    nickName?: string | null;
    email?: string | null;
    password?: string | null;
    avatar?: string | null;
    signature?: string | null;
    balance?: number | null;
    userRole?: UserRole | null;
    userStatus?: UserStatus | null;
    unbanTime?: string | null;
};

