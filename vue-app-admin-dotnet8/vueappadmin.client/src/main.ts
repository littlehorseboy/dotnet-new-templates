import './assets/main.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'primeicons/primeicons.css';

import { createApp } from 'vue';
import { createPinia } from 'pinia';
import PrimeVue from 'primevue/config';
import Aura from '@primeuix/themes/aura';

import App from './App.vue';
import router from './router';

const app = createApp(App);

app.use(PrimeVue, {
    theme: {
        preset: Aura,
        options: { darkModeSelector: '.p-dark' }
    }
});
app.use(createPinia());
app.use(router);

app.mount('#app');
