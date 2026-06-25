import apiClient from '@/lib/axios';
import type { ApiResponse, LoginRequest, LoginResponse, MeResponse } from '@/types/api';

export async function login(request: LoginRequest): Promise<LoginResponse> {
    const { data } = await apiClient.post<ApiResponse<LoginResponse>>('/api/Auth/Login', request);
    return data.result!;
}

export async function getMe(): Promise<MeResponse> {
    const { data } = await apiClient.get<ApiResponse<MeResponse>>('/api/Auth/Me');
    return data.result!;
}
