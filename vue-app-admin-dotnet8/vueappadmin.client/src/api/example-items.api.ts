import apiClient from '@/lib/axios';
import type { ApiPagedResponse, ApiResponse, ItemResponse } from '@/types/api';

export interface GetItemsParams {
    skip: number;
    top: number;
    sortField: string;
    sortOrder: string;
}

export async function getAllItems(params: GetItemsParams): Promise<{ items: ItemResponse[]; total: number }> {
    const { data } = await apiClient.get<ApiPagedResponse<ItemResponse>>('/api/ExampleItems', { params });
    return { items: data.results ?? [], total: data.total };
}

export async function getItemById(id: number): Promise<ItemResponse> {
    const { data } = await apiClient.get<ApiResponse<ItemResponse>>(`/api/ExampleItems/${id}`);
    return data.result!;
}
