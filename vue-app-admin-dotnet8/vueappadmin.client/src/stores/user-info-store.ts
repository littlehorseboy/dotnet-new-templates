import { defineStore } from 'pinia';
import axios from 'axios';

export const useUserInfoStore = defineStore('userInfo', {
    state: () => ({
        username: '',
        displayName: ''
    }),
    actions: {
        async fetchUserInfo() {
            const { data } = await axios.get<{ username: string; displayName: string }>('/api/Auth/Me');
            this.username = data.username;
            this.displayName = data.displayName;
        }
    }
});
