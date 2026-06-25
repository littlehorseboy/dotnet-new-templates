import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth-store';

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/login',
            name: 'login',
            component: () => import('@/views/LoginView.vue'),
            meta: { noAuthRequired: true, title: '登入' }
        },
        {
            path: '/',
            component: () => import('@/views/layouts/MainLayout.vue'),
            redirect: { name: 'dashboard' },
            children: [
                {
                    path: 'dashboard',
                    name: 'dashboard',
                    component: () => import('@/views/DashboardView.vue'),
                    meta: { showInSidebar: true, sidebarLabel: 'Dashboard', sidebarIcon: 'bi-speedometer2', title: 'Dashboard' }
                },
                {
                    path: 'example-items',
                    name: 'example-items',
                    component: () => import('@/views/ExampleItemsView.vue'),
                    meta: { showInSidebar: true, sidebarLabel: 'Example Items', sidebarIcon: 'bi-list-ul', title: 'Example Items' }
                }
            ]
        },
        {
            path: '/:pathMatch(.*)*',
            name: 'not-found',
            component: () => import('@/views/NotFoundView.vue'),
            meta: { noAuthRequired: true, title: '404' }
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

router.afterEach((to) => {
    const title = to.meta.title;
    document.title = title ? `${title} | VueAppAdmin` : 'VueAppAdmin';
});

export default router;
