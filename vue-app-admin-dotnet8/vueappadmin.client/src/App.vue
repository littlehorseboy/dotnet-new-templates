<script setup lang="ts">
import { onMounted } from 'vue';
import NavigationProgress from '@/components/NavigationProgress.vue';
import { init } from '@/composables/useTheme';

onMounted(init);
</script>

<template>
    <NavigationProgress />
    <RouterView />
</template>
