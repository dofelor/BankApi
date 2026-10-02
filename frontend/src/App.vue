<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from './stores/auth'
import Navbar from './components/Navbar.vue'
import ToastContainer from './components/ToastContainer.vue'

const authStore = useAuthStore()

onMounted(async () => {
  if (authStore.isAuthenticated) {
    try {
      await authStore.fetchMe()
    } catch {
      // Token might be invalid or expired
    }
  }
})
</script>

<template>
  <div class="app-layout">
    <Navbar />
    <main class="app-content">
      <router-view />
    </main>
    <ToastContainer />
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-content {
  flex-grow: 1;
}
</style>
