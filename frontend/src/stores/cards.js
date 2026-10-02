import { defineStore } from 'pinia'
import apiClient from '../api/client'

export const useCardStore = defineStore('cards', {
  state: () => ({
    cardsByAccount: {},
    loading: false,
    error: null
  }),

  actions: {
    async fetchCardsByAccountId(accountId) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.get(`/cards/account/${accountId}`)
        this.cardsByAccount[accountId] = response.data
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async createCard(accountId, cardDto) {
      this.loading = true
      this.error = null
      try {
        const response = await apiClient.post(`/cards/account/${accountId}`, cardDto)
        await this.fetchCardsByAccountId(accountId)
        return response.data
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async blockCard(id, accountId) {
      this.loading = true
      this.error = null
      try {
        await apiClient.patch(`/cards/${id}/block`)
        if (accountId) {
          await this.fetchCardsByAccountId(accountId)
        }
      } catch (err) {
        this.error = err.message
        throw err
      } finally {
        this.loading = false
      }
    },

    async unblockCard(id, accountId) {
      this.loading = true
      this.error = null
      try {
        await apiClient.patch(`/cards/${id}/unblock`)
        if (accountId) {
          await this.fetchCardsByAccountId(accountId)
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
