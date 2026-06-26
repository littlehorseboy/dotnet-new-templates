<script setup lang="ts">
import { ref, onMounted } from 'vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import MultiSelect from 'primevue/multiselect';
import { searchItems } from '@/api/example-items.api';
import { getExampleCategories } from '@/api/example-categories.api';
import type { ExampleCategoryResponse, ExampleItemsSearchRequest, ItemResponse } from '@/types/api';

const items = ref<ItemResponse[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const totalRecords = ref(0);

const categories = ref<ExampleCategoryResponse[]>([]);
const filterName = ref('');
const filterDescription = ref('');
const filterCategoryIds = ref<ExampleCategoryResponse[]>([]);

// PrimeVue DataTable lazy mode 的分頁/排序狀態
// first = 目前頁第一筆的 offset（0-based），rows = 每頁筆數
const lazyParams = ref({
    first: 0,
    rows: 10,
    sortField: 'id',
    sortOrder: 'asc',
});

// 文字篩選 debounce timer，避免每次按鍵都送出 API 請求
let debounceTimer: ReturnType<typeof setTimeout> | null = null;

function buildRequest(): ExampleItemsSearchRequest {
    const { first, rows, sortField, sortOrder } = lazyParams.value;
    return {
        page: Math.floor(first / rows) + 1,
        pageSize: rows,
        sortField,
        sortOrder,
        name: filterName.value || undefined,
        description: filterDescription.value || undefined,
        categoryIds: filterCategoryIds.value.map((c) => c.id),
    };
}

async function loadItems() {
    loading.value = true;
    error.value = null;
    try {
        const result = await searchItems(buildRequest());
        items.value = result.items;
        totalRecords.value = result.total;
    } catch (err) {
        error.value = err instanceof Error ? err.message : '載入失敗';
    } finally {
        loading.value = false;
    }
}

// 文字欄位變更：300ms debounce 後重設到第一頁並重新查詢
function onFilterChange() {
    if (debounceTimer) clearTimeout(debounceTimer);
    debounceTimer = setTimeout(() => {
        lazyParams.value.first = 0;
        loadItems();
    }, 300);
}

// 類別多選變更：立即重設到第一頁並重新查詢（不 debounce）
function onCategoryChange() {
    lazyParams.value.first = 0;
    loadItems();
}

function onPage(event: { first: number; rows: number }) {
    lazyParams.value.first = event.first;
    lazyParams.value.rows = event.rows;
    loadItems();
}

// PrimeVue DataTable sortOrder: 1 = asc, -1 = desc
function onSort(event: { sortField: string; sortOrder: number }) {
    lazyParams.value.first = 0;
    lazyParams.value.sortField = event.sortField;
    lazyParams.value.sortOrder = event.sortOrder === -1 ? 'desc' : 'asc';
    loadItems();
}

onMounted(async () => {
    categories.value = await getExampleCategories();
    await loadItems();
});
</script>

<template>
    <div>
        <h2 class="mb-3">Example Items</h2>

        <div class="row g-2 mb-3">
            <div class="col-12 col-md-4">
                <input
                    v-model="filterName"
                    type="text"
                    class="form-control form-control-sm"
                    placeholder="名稱搜尋..."
                    @input="onFilterChange"
                />
            </div>
            <div class="col-12 col-md-4">
                <input
                    v-model="filterDescription"
                    type="text"
                    class="form-control form-control-sm"
                    placeholder="說明搜尋..."
                    @input="onFilterChange"
                />
            </div>
            <div class="col-12 col-md-4">
                <MultiSelect
                    v-model="filterCategoryIds"
                    :options="categories"
                    optionLabel="name"
                    placeholder="選擇類別..."
                    class="w-100"
                    @change="onCategoryChange"
                />
            </div>
        </div>

        <div v-if="error" class="alert alert-danger d-flex align-items-center gap-2">
            <i class="bi bi-exclamation-triangle-fill"></i>
            <span>{{ error }}</span>
            <button class="btn btn-sm btn-outline-danger ms-auto" @click="loadItems">重試</button>
        </div>
        <DataTable
            v-else
            :value="items"
            :loading="loading"
            :lazy="true"
            :paginator="true"
            :rows="lazyParams.rows"
            :totalRecords="totalRecords"
            @page="onPage"
            @sort="onSort"
        >
            <Column field="id" header="ID" style="width: 80px" />
            <Column field="name" header="名稱" :sortable="true" />
            <Column field="description" header="說明" :sortable="true" />
            <Column field="categoryName" header="類別" style="width: 120px" />
        </DataTable>
    </div>
</template>
