<script setup lang="ts">
import { ref, onMounted } from 'vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import { getAllItems } from '@/api/example-items.api';
import type { ItemResponse } from '@/types/api';

const items = ref<ItemResponse[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const totalRecords = ref(0);
const lazyParams = ref({
    first: 0,
    rows: 10,
    sortField: 'id',
    sortOrder: 'asc',
});

async function loadItems() {
    loading.value = true;
    error.value = null;
    try {
        const { first, rows, sortField, sortOrder } = lazyParams.value;
        const result = await getAllItems({ skip: first, top: rows, sortField, sortOrder });
        items.value = result.items;
        totalRecords.value = result.total;
    } catch (err) {
        error.value = err instanceof Error ? err.message : '載入失敗';
    } finally {
        loading.value = false;
    }
}

function onPage(event: { first: number; rows: number }) {
    lazyParams.value.first = event.first;
    lazyParams.value.rows = event.rows;
    loadItems();
}

function onSort(event: { sortField: string; sortOrder: number }) {
    lazyParams.value.first = 0;
    lazyParams.value.sortField = event.sortField;
    lazyParams.value.sortOrder = event.sortOrder === -1 ? 'desc' : 'asc';
    loadItems();
}

onMounted(loadItems);
</script>

<template>
    <div>
        <h2 class="mb-3">Example Items</h2>
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
        </DataTable>
    </div>
</template>
