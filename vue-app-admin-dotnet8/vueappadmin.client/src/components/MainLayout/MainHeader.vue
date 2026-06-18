<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth-store';
import { useUserInfoStore } from '@/stores/user-info-store';

const router = useRouter();
const authStore = useAuthStore();
const userInfoStore = useUserInfoStore();

function logout() {
    authStore.logout();
    router.push({ name: 'login' });
}
</script>

<template>
    <header class="navbar navbar-expand navbar-dark bg-dark px-3">
        <span class="navbar-brand mb-0 h1">VueAppAdmin</span>
        <div class="ms-auto d-flex align-items-center gap-3">
            <span class="text-white-50 small">{{ userInfoStore.displayName }}</span>
            <button class="btn btn-outline-light btn-sm" @click="logout">登出</button>
        </div>
    </header>
</template>
