import { defineStore } from 'pinia'
import apiClient from '../api/client'

export const useClientStore = defineStore('clients', {
  state: () => ({
    clients: [],
    currentClient: null,
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    loading: false,
    error: null
  }),

  getters: {
    totalPages: (state) => Math.ceil(state.totalCount / state.pageSize) || 1
  },

  actions: {
    async fetchClients(page = 1, size = 10) {
      this.loading = true
      this.error = null
      this.pageNumber = page
      this.pageSize = size
      try {
        const response = await apiClient.get('/clients', {
          params: { pageNumber: page, pageSize: size }
        })

        this.clients = response.data

        const headerCount = response.headers['x-total-count']
        if (headerCount !== undefined) {
          this.totalCount = parseInt(headerCount, 10) || 0
        } else {
          // Fallback: fetch count endpoint
          try {
            const countRes = await apiClient.get('/clients/count')
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
    },

    async fetchClientById(id) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.get(`/clients/${id}`)
        this.currentClient = response.data
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async createClient(clientDto) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.post('/clients', clientDto)
        await this.fetchClients(this.pageNumber, this.pageSize)
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async updateClient(id, clientDto) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.put(`/clients/${id}`, clientDto)
        if (this.currentClient?.id === id) {
          this.currentClient = { ...this.currentClient, ...response.data }
        }
        await this.fetchClients(this.pageNumber, this.pageSize)
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async deleteClient(id) {
      this.loading = true
      this.error = null
      try {
        await apiClient.delete(`/clients/${id}`)
        await this.fetchClients(this.pageNumber, this.pageSize)
        if (this.currentClient?.id === id) {
          this.currentClient = null
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
