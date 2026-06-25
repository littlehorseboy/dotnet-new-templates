export interface ApiResponse<T> {
    success: boolean;
    message: string | null;
    result: T | null;
    results: T[] | null;
}

export interface ApiPagedResponse<T> extends ApiResponse<T> {
    total: number;
}

export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
}

export interface MeResponse {
    username: string;
    displayName: string;
}

export interface ItemResponse {
    id: number;
    name: string;
    description: string;
}
