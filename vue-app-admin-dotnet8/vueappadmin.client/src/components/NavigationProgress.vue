<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import ProgressBar from 'primevue/progressbar';

const visible = ref(false);
const width = ref(0);
let timer: ReturnType<typeof setTimeout> | null = null;

const router = useRouter();

router.beforeEach(() => {
    visible.value = true;
    width.value = 30;
    timer = setTimeout(() => { width.value = 70; }, 200);
});

router.afterEach(() => {
    if (timer) clearTimeout(timer);
    width.value = 100;
    setTimeout(() => {
        visible.value = false;
        width.value = 0;
    }, 300);
});
</script>

<template>
    <Transition name="nav-progress-fade">
        <div v-if="visible" class="nav-progress">
            <ProgressBar :value="width" :show-value="false" />
        </div>
    </Transition>
</template>

<style scoped>
.nav-progress {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    z-index: 9999;
}
:deep(.p-progressbar) {
    height: 3px;
    border-radius: 0;
    background: transparent;
}
:deep(.p-progressbar-value) {
    transition: width 0.3s ease;
    border-radius: 0;
}
.nav-progress-fade-leave-active {
    transition: opacity 0.3s;
}
.nav-progress-fade-leave-to {
    opacity: 0;
}
</style>
