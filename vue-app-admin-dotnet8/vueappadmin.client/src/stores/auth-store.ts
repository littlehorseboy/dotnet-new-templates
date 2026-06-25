import { defineStore } from 'pinia';

const TOKEN_KEY = 'SiteToken';

export const useAuthStore = defineStore('auth', {
    state: () => ({
        isAuthenticated: !!localStorage.getItem(TOKEN_KEY)
    }),
    actions: {
        login(token: string) {
            localStorage.setItem(TOKEN_KEY, token);
            this.isAuthenticated = true;
        },
        logout() {
            localStorage.removeItem(TOKEN_KEY);
            this.isAuthenticated = false;
        }
    }
});
