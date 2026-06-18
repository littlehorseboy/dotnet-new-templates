import { defineStore } from 'pinia';
import axios from 'axios';

const TOKEN_KEY = 'SiteToken';

export const useAuthStore = defineStore('auth', {
    state: () => ({
        isAuthenticated: !!localStorage.getItem(TOKEN_KEY)
    }),
    actions: {
        init() {
            const token = localStorage.getItem(TOKEN_KEY);
            if (token) {
                axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
            }
        },
        login(token: string) {
            localStorage.setItem(TOKEN_KEY, token);
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
            this.isAuthenticated = true;
        },
        logout() {
            localStorage.removeItem(TOKEN_KEY);
            delete axios.defaults.headers.common['Authorization'];
            this.isAuthenticated = false;
        }
    }
});
