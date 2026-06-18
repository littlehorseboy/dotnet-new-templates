<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';

interface ExampleItem {
    id: number;
    name: string;
    description: string;
}

const items = ref<ExampleItem[]>([]);
const loading = ref(true);

onMounted(async () => {
    try {
        const { data } = await axios.get<ExampleItem[]>('/api/ExampleItems');
        items.value = data;
    } finally {
        loading.value = false;
    }
});
</script>

<template>
    <div>
        <h2>Example Items</h2>
        <DataTable :value="items" :loading="loading" class="mt-3">
            <Column field="id" header="ID" style="width: 80px" />
            <Column field="name" header="名稱" />
            <Column field="description" header="說明" />
        </DataTable>
    </div>
</template>
