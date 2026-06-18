import './assets/main.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'primeicons/primeicons.css';

import { createApp } from 'vue';
import { createPinia } from 'pinia';
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';

import App from './App.vue';
import router from './router';
import { useAuthStore } from './stores/auth-store';

const app = createApp(App);

app.use(PrimeVue, { theme: { preset: Aura } });
app.use(createPinia());
app.use(router);

const authStore = useAuthStore();
authStore.init();

app.mount('#app');
