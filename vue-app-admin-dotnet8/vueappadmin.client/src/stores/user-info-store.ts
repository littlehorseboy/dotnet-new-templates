import { defineStore } from 'pinia';
import { getMe } from '@/api/auth.api';

export const useUserInfoStore = defineStore('userInfo', {
    state: () => ({
        username: '',
        displayName: '',
        isLoading: false,
        error: null as string | null
    }),
    actions: {
        async fetchUserInfo() {
            this.isLoading = true;
            this.error = null;
            try {
                const me = await getMe();
                this.username = me.username;
                this.displayName = me.displayName;
            } catch (err) {
                this.error = err instanceof Error ? err.message : '無法取得使用者資訊';
            } finally {
                this.isLoading = false;
            }
        }
    }
});
