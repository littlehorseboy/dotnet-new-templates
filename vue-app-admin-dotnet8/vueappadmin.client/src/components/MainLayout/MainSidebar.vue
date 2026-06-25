<script setup lang="ts">
import { useRouter } from 'vue-router';

const router = useRouter();

const sidebarRoutes = router.getRoutes().filter(
    (r) => r.meta.showInSidebar === true
);
</script>

<template>
    <nav class="bg-light border-end" style="width: 220px; min-height: 100%">
        <ul class="nav flex-column pt-3">
            <li v-for="route in sidebarRoutes" :key="route.path" class="nav-item">
                <RouterLink
                    :to="route.path"
                    class="nav-link text-dark d-flex align-items-center gap-2"
                    active-class="fw-bold text-primary"
                >
                    <i v-if="route.meta.sidebarIcon" class="bi" :class="route.meta.sidebarIcon"></i>
                    {{ route.meta.sidebarLabel }}
                </RouterLink>
            </li>
        </ul>
    </nav>
</template>
