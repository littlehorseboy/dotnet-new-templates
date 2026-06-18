<script setup lang="ts">
import { useUserInfoStore } from '@/stores/user-info-store';

const userInfoStore = useUserInfoStore();
</script>

<template>
    <div>
        <h2>Dashboard</h2>
        <p class="text-muted">歡迎，{{ userInfoStore.displayName || '使用者' }}！</p>
    </div>
</template>
