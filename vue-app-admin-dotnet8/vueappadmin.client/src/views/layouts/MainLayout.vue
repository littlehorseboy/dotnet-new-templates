<script setup lang="ts">
import MainHeader from '@/components/MainLayout/MainHeader.vue';
import MainSidebar from '@/components/MainLayout/MainSidebar.vue';
</script>

<template>
    <!-- 整體版面：Header 固定頂部，下方左側 Sidebar + 右側 RouterView -->
    <!-- Sidebar 寬度自適應內容（minmax(8rem, max-content)），主要區域佔剩餘空間 -->
    <div class="d-flex flex-column vh-100">
        <MainHeader />
        <div class="flex-grow-1 overflow-hidden" style="display: grid; grid-template-columns: minmax(8rem, max-content) 1fr">
            <MainSidebar />
            <main class="flex-grow-1 overflow-auto p-4">
                <RouterView />
            </main>
        </div>
    </div>
</template>
