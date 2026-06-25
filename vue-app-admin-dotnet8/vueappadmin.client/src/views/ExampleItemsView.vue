<script setup lang="ts">
import { ref, onMounted } from 'vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import { getAllItems } from '@/api/example-items.api';
import type { ItemResponse } from '@/types/api';

const items = ref<ItemResponse[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);

async function loadItems() {
    loading.value = true;
    error.value = null;
    try {
        items.value = await getAllItems();
    } catch (err) {
        error.value = err instanceof Error ? err.message : '載入失敗';
    } finally {
        loading.value = false;
    }
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
        <DataTable v-else :value="items" :loading="loading">
            <Column field="id" header="ID" style="width: 80px" />
            <Column field="name" header="名稱" />
            <Column field="description" header="說明" />
        </DataTable>
    </div>
</template>
