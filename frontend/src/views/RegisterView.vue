<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useToast } from '../composables/useToast'

const router = useRouter()
const authStore = useAuthStore()
const toast = useToast()

const username = ref('')
const fullName = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const role = ref('User')

const isLoading = ref(false)
const errorMessage = ref('')

async function handleRegister() {
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match.'
    return
  }
  if (password.value.length < 6) {
    errorMessage.value = 'Password must be at least 6 characters long.'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    await authStore.register({
      username: username.value,
      fullName: fullName.value || username.value,
      email: email.value,
      password: password.value,
      role: role.value
    })
    toast.success('Registration successful! Welcome.')
    router.push('/clients')
  } catch (err) {
    errorMessage.value = err.message || 'Registration failed.'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="auth-page">
    <div class="auth-card">
      <div class="auth-header">
        <h1 class="auth-title">Create Account</h1>
        <p class="auth-subtitle">Get started with BankApi management portal</p>
      </div>

      <div v-if="errorMessage" class="alert-error" role="alert">
        {{ errorMessage }}
      </div>

      <form @submit.prevent="handleRegister" class="auth-form">
        <div class="form-group">
          <label for="username">Username</label>
          <input
            id="username"
            v-model="username"
            type="text"
            required
            placeholder="e.g. john_doe"
            class="form-input"
          />
        </div>

        <div class="form-group">
          <label for="fullName">Full Name</label>
          <input
            id="fullName"
            v-model="fullName"
            type="text"
            placeholder="e.g. John Doe"
            class="form-input"
          />
        </div>

        <div class="form-group">
          <label for="email">Email Address</label>
          <input
            id="email"
            v-model="email"
            type="email"
            required
            placeholder="e.g. john@bank.com"
            class="form-input"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="password">Password</label>
            <input
              id="password"
              v-model="password"
              type="password"
              required
              placeholder="Min 6 characters"
              class="form-input"
            />
          </div>

          <div class="form-group">
            <label for="confirm">Confirm</label>
            <input
              id="confirm"
              v-model="confirmPassword"
              type="password"
              required
              placeholder="Repeat password"
              class="form-input"
            />
          </div>
        </div>

        <div class="form-group">
          <label for="role">Role</label>
          <select id="role" v-model="role" class="form-input">
            <option value="User">Standard User</option>
            <option value="Admin">Administrator</option>
          </select>
        </div>

        <button type="submit" class="btn-primary btn-block" :disabled="isLoading">
          <span v-if="isLoading" class="spinner"></span>
          <span v-else>Register Account</span>
        </button>
      </form>

      <div class="auth-footer">
        <p>Already have an account? <router-link to="/login" class="auth-link">Sign in</router-link></p>
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
  max-width: 440px;
  background: var(--bg-surface);
  border: 1px solid var(--border-color);
  border-radius: 12px;
  padding: 32px 28px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);
}

.auth-header {
  text-align: center;
  margin-bottom: 24px;
}

.auth-title {
  font-size: 22px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 6px;
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
  gap: 14px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.form-group label {
  font-size: 13px;
  font-weight: 500;
  color: var(--text-primary);
}

.form-input {
  width: 100%;
  padding: 9px 12px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-primary);
  font-size: 14px;
  outline: none;
  box-sizing: border-box;
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
  margin-top: 6px;
  transition: background 0.15s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-primary:hover:not(:disabled) {
  background: var(--accent-hover);
}

.btn-block {
  width: 100%;
}

.auth-footer {
  margin-top: 20px;
  text-align: center;
  font-size: 13px;
  color: var(--text-muted);
}

.auth-link {
  color: var(--accent-color);
  text-decoration: none;
  font-weight: 600;
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
