<script setup>
import { onMounted } from 'vue'
import { useTransactionStore } from '../stores/transactions'
import { useToast } from '../composables/useToast'
import Pagination from '../components/Pagination.vue'

const transactionStore = useTransactionStore()
const toast = useToast()

onMounted(() => {
  loadLogs()
})

async function loadLogs(page = transactionStore.page, size = transactionStore.pageSize) {
  try {
    await transactionStore.fetchLogs(page, size)
  } catch (err) {
    toast.error('Failed to load transaction logs: ' + err.message)
  }
}

function handlePaginationChange({ page, pageSize }) {
  loadLogs(page, pageSize)
}

function formatDate(isoStr) {
  if (!isoStr) return '-'
  const d = new Date(isoStr)
  return d.toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}
</script>

<template>
  <div class="page-container">
    <div class="page-header">
      <div>
        <h1 class="page-title">Transaction Audit Logs</h1>
        <p class="page-subtitle">Complete ledger of account transfers and transactions</p>
      </div>

      <button class="btn-secondary" @click="loadLogs(1, transactionStore.pageSize)">
        <svg viewBox="0 0 20 20" fill="currentColor" width="16" height="16">
          <path fill-rule="evenodd" d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z" clip-rule="evenodd" />
        </svg>
        <span>Refresh Logs</span>
      </button>
    </div>

    <!-- Table Card -->
    <div class="card table-card">
      <div v-if="transactionStore.loading && transactionStore.logs.length === 0" class="loading-state">
        <div class="spinner-large"></div>
        <p>Loading audit records...</p>
      </div>

      <div v-else-if="transactionStore.logs.length === 0" class="empty-state">
        <svg viewBox="0 0 24 24" width="48" height="48" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
        <h3>No transactions recorded</h3>
        <p>Transfers executed between accounts will appear here automatically.</p>
        <router-link to="/transfers" class="btn-primary btn-sm">
          Make a Transfer
        </router-link>
      </div>

      <div v-else class="table-responsive">
        <table class="data-table">
          <thead>
            <tr>
              <th style="width: 70px">ID</th>
              <th>Date & Time</th>
              <th>From Account</th>
              <th>To Account</th>
              <th>Amount</th>
              <th>Currency</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in transactionStore.logs" :key="log.id">
              <td class="font-mono text-muted">#{{ log.id }}</td>
              <td class="text-muted">{{ formatDate(log.createdAt) }}</td>
              <td class="font-mono">{{ log.fromAccountNumber }}</td>
              <td class="font-mono">{{ log.toAccountNumber }}</td>
              <td class="font-medium text-amount">
                {{ Number(log.amount).toLocaleString('en-US', { minimumFractionDigits: 2 }) }}
              </td>
              <td>
                <span class="currency-badge">{{ log.currency }}</span>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Integrated Pagination -->
        <Pagination
          :current-page="transactionStore.page"
          :total-pages="transactionStore.totalPages"
          :page-size="transactionStore.pageSize"
          :total-items="transactionStore.totalCount"
          @change="handlePaginationChange"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 32px 20px;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 24px;
}

.page-title {
  font-size: 24px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 4px;
}

.page-subtitle {
  font-size: 14px;
  color: var(--text-muted);
  margin: 0;
}

.table-card {
  background: var(--bg-surface);
  border: 1px solid var(--border-color);
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.table-responsive {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
  font-size: 14px;
}

.data-table th {
  padding: 12px 16px;
  background: var(--bg-surface-alt);
  border-bottom: 1px solid var(--border-color);
  font-weight: 600;
  color: var(--text-muted);
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.data-table td {
  padding: 14px 16px;
  border-bottom: 1px solid var(--border-color);
  color: var(--text-primary);
}

.data-table tbody tr:hover {
  background: var(--bg-hover);
}

.text-amount {
  font-weight: 600;
  color: var(--accent-color);
}

.currency-badge {
  display: inline-block;
  padding: 2px 8px;
  border-radius: 4px;
  background: var(--bg-surface-alt);
  border: 1px solid var(--border-color);
  font-size: 12px;
  font-weight: 600;
  color: var(--text-muted);
}

.btn-secondary {
  padding: 8px 16px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-primary);
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.15s ease;
}

.btn-secondary:hover {
  background: var(--bg-hover);
}

.btn-primary {
  padding: 8px 16px;
  border-radius: 6px;
  border: none;
  background: var(--accent-color);
  color: #fff;
  font-size: 14px;
  font-weight: 600;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
}

.btn-sm {
  padding: 6px 12px;
  font-size: 13px;
}

.loading-state,
.empty-state {
  padding: 48px 24px;
  text-align: center;
  color: var(--text-muted);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}

.spinner-large {
  width: 32px;
  height: 32px;
  border: 3px solid var(--border-color);
  border-top-color: var(--accent-color);
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
