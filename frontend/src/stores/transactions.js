import { defineStore } from 'pinia'
import apiClient from '../api/client'

export const useTransactionStore = defineStore('transactions', {
  state: () => ({
    logs: [],
    totalCount: 0,
    page: 1,
    pageSize: 15,
    loading: false,
    error: null
  }),

  getters: {
    totalPages: (state) => Math.ceil(state.totalCount / state.pageSize) || 1
  },

  actions: {
    async transfer(fromAccountId, toAccountId, amount) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.post('/transaction/transfer', {
          fromAccountId: parseInt(fromAccountId, 10),
          toAccountId: parseInt(toAccountId, 10),
          amount: parseFloat(amount)
        })
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async fetchLogs(page = 1, pageSize = 15) {
      this.loading = true
      this.error = null
      this.page = page
      this.pageSize = pageSize
      try {
        const response = await apiClient.get('/transactionlogs', {
          params: { page, pageSize }
        })
        this.logs = response.data

        const headerCount = response.headers['x-total-count']
        if (headerCount !== undefined) {
          this.totalCount = parseInt(headerCount, 10) || 0
        } else {
          try {
            const countRes = await apiClient.get('/transactionlogs/count')
            this.totalCount = countRes.data
          } catch {
            this.totalCount = response.data.length
          }
        }
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    }
  }
})
