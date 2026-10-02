<script setup>
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const currentPath = computed(() => route.path)

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <header class="navbar" v-if="authStore.isAuthenticated">
    <div class="nav-container">
      <div class="nav-brand">
        <router-link to="/clients" class="brand-link">
          <svg class="brand-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 21h18M3 10h18M5 10v11M19 10v11M9 10v11M15 10v11M12 3l9 7H3l9-7z" />
          </svg>
          <span class="brand-text">Bank<span class="brand-accent">Api</span></span>
        </router-link>

        <nav class="nav-links">
          <router-link
            to="/clients"
            class="nav-link"
            :class="{ active: currentPath.startsWith('/clients') }"
          >
            Clients
          </router-link>
          <router-link
            to="/transfers"
            class="nav-link"
            :class="{ active: currentPath === '/transfers' }"
          >
            Transfers
          </router-link>
          <!-- Аудит логи видны только Admin -->
          <router-link
            v-if="authStore.isAdmin"
            to="/transactions"
            class="nav-link"
            :class="{ active: currentPath === '/transactions' }"
          >
            Audit Logs
          </router-link>
        </nav>
      </div>

      <div class="nav-user">
        <div class="user-chip">
          <span class="user-avatar">{{ authStore.username.charAt(0).toUpperCase() }}</span>
          <div class="user-details">
            <span class="user-name">{{ authStore.username }}</span>
            <span class="user-role" v-if="authStore.roles.length">{{ authStore.roles.join(', ') }}</span>
          </div>
        </div>

        <button @click="handleLogout" class="btn-logout" title="Sign out">
          <svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4M16 17l5-5-5-5M21 12H9" />
          </svg>
          <span>Logout</span>
        </button>
      </div>
    </div>
  </header>
</template>

<style scoped>
.navbar {
  background: var(--bg-surface);
  border-bottom: 1px solid var(--border-color);
  position: sticky;
  top: 0;
  z-index: 40;
}

.nav-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.nav-brand {
  display: flex;
  align-items: center;
  gap: 32px;
}

.brand-link {
  display: flex;
  align-items: center;
  gap: 10px;
  text-decoration: none;
  color: var(--text-primary);
  font-weight: 700;
  font-size: 19px;
  letter-spacing: -0.5px;
}

.brand-icon {
  width: 24px;
  height: 24px;
  color: var(--accent-color);
}

.brand-accent {
  color: var(--accent-color);
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 6px;
}

.nav-link {
  padding: 8px 14px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  color: var(--text-muted);
  text-decoration: none;
  transition: all 0.15s ease;
}

.nav-link:hover {
  color: var(--text-primary);
  background: var(--bg-hover);
}

.nav-link.active {
  color: var(--accent-color);
  background: var(--accent-light);
  font-weight: 600;
}

.nav-user {
  display: flex;
  align-items: center;
  gap: 16px;
}

.user-chip {
  display: flex;
  align-items: center;
  gap: 10px;
}

.user-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--accent-light);
  color: var(--accent-color);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 14px;
}

.user-details {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-primary);
}

.user-role {
  font-size: 11px;
  color: var(--text-muted);
  text-transform: capitalize;
}

.btn-logout {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-muted);
  font-size: 13px;
  cursor: pointer;
  transition: all 0.15s ease;
}

.btn-logout:hover {
  background: #fee2e2;
  border-color: #fca5a5;
  color: #dc2626;
}
</style>
