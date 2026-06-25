import apiClient from '@/lib/axios';
import type { ApiResponse, ItemResponse } from '@/types/api';

export async function getAllItems(): Promise<ItemResponse[]> {
    const { data } = await apiClient.get<ApiResponse<ItemResponse>>('/api/ExampleItems');
    return data.results ?? [];
}

export async function getItemById(id: number): Promise<ItemResponse> {
    const { data } = await apiClient.get<ApiResponse<ItemResponse>>(`/api/ExampleItems/${id}`);
    return data.result!;
}
