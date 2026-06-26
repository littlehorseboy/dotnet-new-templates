<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getMenuItems } from '@/api/menu.api';
import type { MenuNode } from '@/types/api';
import SidebarMenuItem from './SidebarMenuItem.vue';

const menuNodes = ref<MenuNode[]>([]);

// 掛載時向後端請求依使用者權限過濾後的選單，後端依 JWT claims 中的 features 過濾
onMounted(async () => {
    menuNodes.value = await getMenuItems();
});
</script>

<template>
    <nav class="border-end">
        <ul class="nav flex-column pt-3">
            <!-- SidebarMenuItem 支援遞迴渲染，可處理多層巢狀選單 -->
            <SidebarMenuItem v-for="node in menuNodes" :key="node.id" :node="node" />
        </ul>
    </nav>
</template>
