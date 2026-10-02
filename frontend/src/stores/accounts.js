import { defineStore } from 'pinia'
import apiClient from '../api/client'

export const useAccountStore = defineStore('accounts', {
  state: () => ({
    accounts: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchAccountsByClientId(clientId) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.get(`/accounts/client/${clientId}`)
        this.accounts = response.data
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async fetchAccountById(id) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.get(`/accounts/${id}`)
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async createAccount(clientId, currency = 'KGS') {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.post(`/accounts/client/${clientId}`, {
          currency: currency.toUpperCase()
        })
        await this.fetchAccountsByClientId(clientId)
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async closeAccount(id, clientId) {
      this.loading = true
      this.error = null
      try {
        await apiClient.patch(`/accounts/${id}/close`)
        if (clientId) {
          await this.fetchAccountsByClientId(clientId)
        }
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async deposit(id, amount, clientId) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.post(`/accounts/${id}/deposit`, {
          amount: parseFloat(amount)
        })
        if (clientId) {
          await this.fetchAccountsByClientId(clientId)
        }
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    }
  }
})
