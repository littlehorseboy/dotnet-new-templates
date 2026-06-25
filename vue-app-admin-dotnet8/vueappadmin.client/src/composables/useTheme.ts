import { ref } from 'vue';

const isDark = ref(false);

function apply() {
    document.documentElement.classList.toggle('p-dark', isDark.value);
    document.documentElement.setAttribute('data-bs-theme', isDark.value ? 'dark' : 'light');
}

function init() {
    const saved = localStorage.getItem('theme');
    if (saved) {
        isDark.value = saved === 'dark';
    } else {
        isDark.value = window.matchMedia('(prefers-color-scheme: dark)').matches;
    }
    apply();
}

function toggle() {
    isDark.value = !isDark.value;
    localStorage.setItem('theme', isDark.value ? 'dark' : 'light');
    apply();
}

export { isDark, init, toggle };
