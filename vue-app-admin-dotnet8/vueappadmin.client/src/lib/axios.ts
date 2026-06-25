import axios from 'axios';
import type { ApiResponse } from '@/types/api';

const TOKEN_KEY = 'SiteToken';

const apiClient = axios.create();

apiClient.interceptors.request.use((config) => {
    const token = localStorage.getItem(TOKEN_KEY);
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

let isRedirectingToLogin = false;

apiClient.interceptors.response.use(
    (response) => {
        const body = response.data as ApiResponse<unknown>;
        if (body && typeof body === 'object' && 'success' in body && !body.success) {
            return Promise.reject(new Error(body.message ?? '操作失敗'));
        }
        return response;
    },
    (error) => {
        const body = error.response?.data as ApiResponse<unknown> | undefined;
        const message = body?.message;

        if (error.response?.status === 401) {
            const isLoginEndpoint = (error.config?.url as string | undefined)?.includes('/api/Auth/Login');
            if (!isLoginEndpoint && !isRedirectingToLogin) {
                isRedirectingToLogin = true;
                localStorage.removeItem(TOKEN_KEY);
                window.location.href = '/login';
            }
        }

        return Promise.reject(message ? new Error(message) : error);
    }
);

export default apiClient;
