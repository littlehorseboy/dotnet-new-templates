<script setup lang="ts">
import MainHeader from '@/components/MainLayout/MainHeader.vue';
import MainSidebar from '@/components/MainLayout/MainSidebar.vue';
</script>

<template>
    <div class="d-flex flex-column vh-100">
        <MainHeader />
        <div class="d-flex flex-grow-1 overflow-hidden">
            <MainSidebar />
            <main class="flex-grow-1 overflow-auto p-4">
                <RouterView />
            </main>
        </div>
    </div>
</template>
