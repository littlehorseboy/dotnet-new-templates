<script setup lang="ts">
import { onMounted } from 'vue';
import NavigationProgress from '@/components/NavigationProgress.vue';
import { init } from '@/composables/useTheme';

// 應用程式掛載時初始化主題（讀取 localStorage 偏好或跟隨系統設定）
onMounted(init);
</script>

<template>
    <!-- NavigationProgress 固定在視窗頂端，RouterView 渲染目前路由對應的頁面 -->
    <NavigationProgress />
    <RouterView />
</template>
