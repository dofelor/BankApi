import { defineStore } from 'pinia'
import apiClient from '../api/client'

export const usePhoneStore = defineStore('phones', {
  state: () => ({
    phones: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchPhonesByClientId(clientId) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.get(`/phones/client/${clientId}`)
        this.phones = response.data
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async createPhone(clientId, phoneDto) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.post(`/phones/client/${clientId}`, phoneDto)
        await this.fetchPhonesByClientId(clientId)
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async deletePhone(id, clientId) {
      this.loading = true
      this.error = null
      try {
        await apiClient.delete(`/phones/${id}`)
        if (clientId) {
          await this.fetchPhonesByClientId(clientId)
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
