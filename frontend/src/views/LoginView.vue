<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useToast } from '../composables/useToast'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const toast = useToast()

const usernameOrEmail = ref('')
const password = ref('')
const isLoading = ref(false)
const errorMessage = ref('')

function fillDemoAdmin() {
  usernameOrEmail.value = 'admin'
  password.value = 'Admin123!'
}

async function handleLogin() {
  if (!usernameOrEmail.value || !password.value) {
    errorMessage.value = 'Please enter both username/email and password.'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    await authStore.login(usernameOrEmail.value, password.value)
    toast.success('Successfully logged in!')
    const redirect = route.query.redirect || '/clients'
    router.push(redirect)
  } catch (err) {
    errorMessage.value = err.message || 'Login failed. Please check your credentials.'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="auth-page">
    <div class="auth-card">
      <div class="auth-header">
        <div class="brand-badge">
          <svg viewBox="0 0 24 24" width="28" height="28" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M3 21h18M3 10h18M5 10v11M19 10v11M9 10v11M15 10v11M12 3l9 7H3l9-7z" />
          </svg>
        </div>
        <h1 class="auth-title">Welcome to BankApi</h1>
        <p class="auth-subtitle">Sign in to manage clients, accounts, and transactions</p>
      </div>

      <div v-if="errorMessage" class="alert-error" role="alert">
        {{ errorMessage }}
      </div>

      <form @submit.prevent="handleLogin" class="auth-form">
        <div class="form-group">
          <label for="username">Username or Email</label>
          <input
            id="username"
            v-model="usernameOrEmail"
            type="text"
            required
            placeholder="admin or user@bank.com"
            class="form-input"
            autocomplete="username"
          />
        </div>

        <div class="form-group">
          <label for="password">Password</label>
          <input
            id="password"
            v-model="password"
            type="password"
            required
            placeholder="Enter your password"
            class="form-input"
            autocomplete="current-password"
          />
        </div>

        <button type="submit" class="btn-primary btn-block" :disabled="isLoading">
          <span v-if="isLoading" class="spinner"></span>
          <span v-else>Sign In</span>
        </button>

        <div class="demo-box">
          <button type="button" @click="fillDemoAdmin" class="btn-demo">
            Quick Fill Demo Admin Credentials
          </button>
          <span class="demo-hint">(admin / Admin123!)</span>
        </div>
      </form>

      <div class="auth-footer">
        <p>Don't have an account? <router-link to="/register" class="auth-link">Create one</router-link></p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
  background: var(--bg-page);
}

.auth-card {
  width: 100%;
  max-width: 420px;
  background: var(--bg-surface);
  border: 1px solid var(--border-color);
  border-radius: 12px;
  padding: 36px 32px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03);
}

.auth-header {
  text-align: center;
  margin-bottom: 24px;
}

.brand-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 52px;
  height: 52px;
  border-radius: 12px;
  background: var(--accent-light);
  color: var(--accent-color);
  margin-bottom: 16px;
}

.auth-title {
  font-size: 22px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 6px;
  letter-spacing: -0.5px;
}

.auth-subtitle {
  font-size: 14px;
  color: var(--text-muted);
  margin: 0;
}

.alert-error {
  background: #fee2e2;
  border: 1px solid #fca5a5;
  color: #b91c1c;
  padding: 10px 14px;
  border-radius: 6px;
  font-size: 13px;
  margin-bottom: 18px;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  font-size: 13px;
  font-weight: 500;
  color: var(--text-primary);
}

.form-input {
  width: 100%;
  padding: 10px 12px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-primary);
  font-size: 14px;
  outline: none;
  box-sizing: border-box;
  transition: border-color 0.15s;
}

.form-input:focus {
  border-color: var(--accent-color);
  box-shadow: 0 0 0 3px var(--accent-light);
}

.btn-primary {
  padding: 11px 16px;
  border-radius: 6px;
  border: none;
  background: var(--accent-color);
  color: #fff;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.15s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-primary:hover:not(:disabled) {
  background: var(--accent-hover);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-block {
  width: 100%;
}

.demo-box {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  padding-top: 8px;
}

.btn-demo {
  background: transparent;
  border: 1px dashed var(--border-color);
  color: var(--accent-color);
  border-radius: 6px;
  padding: 6px 12px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-demo:hover {
  background: var(--accent-light);
  border-color: var(--accent-color);
}

.demo-hint {
  font-size: 11px;
  color: var(--text-muted);
}

.auth-footer {
  margin-top: 24px;
  text-align: center;
  font-size: 13px;
  color: var(--text-muted);
}

.auth-link {
  color: var(--accent-color);
  text-decoration: none;
  font-weight: 600;
}

.auth-link:hover {
  text-decoration: underline;
}

.spinner {
  width: 18px;
  height: 18px;
  border: 2px solid #ffffff;
  border-top-color: transparent;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
