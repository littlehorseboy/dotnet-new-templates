import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth-store';

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/login',
            name: 'login',
            component: () => import('@/views/LoginView.vue'),
            meta: { noAuthRequired: true }
        },
        {
            path: '/',
            component: () => import('@/views/layouts/MainLayout.vue'),
            children: [
                {
                    path: 'dashboard',
                    name: 'dashboard',
                    component: () => import('@/views/DashboardView.vue'),
                    meta: { showInSidebar: true, sidebarLabel: 'Dashboard' }
                },
                {
                    path: 'example-items',
                    name: 'example-items',
                    component: () => import('@/views/ExampleItemsView.vue'),
                    meta: { showInSidebar: true, sidebarLabel: 'Example Items' }
                }
            ]
        }
    ]
});

router.beforeEach((to) => {
    const authStore = useAuthStore();

    if (!to.meta.noAuthRequired && !authStore.isAuthenticated) {
        return { name: 'login' };
    }

    if (to.name === 'login' && authStore.isAuthenticated) {
        return { name: 'dashboard' };
    }
});

export default router;
