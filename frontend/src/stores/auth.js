import { defineStore } from 'pinia'
import apiClient from '../api/client'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('bank_token') || '',
    user: (() => {
      try {
        const stored = localStorage.getItem('bank_user')
        return stored ? JSON.parse(stored) : null
      } catch {
        return null
      }
    })(),
    loading: false,
    error: null
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    username: (state) => state.user?.username || state.user?.fullName || 'User',
    roles: (state) => state.user?.roles || [],
    isAdmin: (state) => (state.user?.roles || []).includes('Admin')
  },

  actions: {
    async login(usernameOrEmail, password) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.post('/auth/login', {
          usernameOrEmail,
          password
        })
        const data = response.data
        this.token = data.token
        this.user = {
          id: data.userId,
          username: data.username,
          email: data.email,
          fullName: data.fullName,
          roles: data.roles
        }
        localStorage.setItem('bank_token', this.token)
        localStorage.setItem('bank_user', JSON.stringify(this.user))
        return data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async register(payload) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.post('/auth/register', payload)
        const data = response.data
        this.token = data.token
        this.user = {
          id: data.userId,
          username: data.username,
          email: data.email,
          fullName: data.fullName,
          roles: data.roles
        }
        localStorage.setItem('bank_token', this.token)
        localStorage.setItem('bank_user', JSON.stringify(this.user))
        return data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async fetchMe() {
      if (!this.token) return null
      try {
        const response = await apiClient.get('/auth/me')
        this.user = response.data
        localStorage.setItem('bank_user', JSON.stringify(this.user))
        return this.user
      } catch (err) {
        this.logout()
        return null
      }
    },

    logout() {
      this.token = ''
      this.user = null
      localStorage.removeItem('bank_token')
      localStorage.removeItem('bank_user')
    }
  }
})
