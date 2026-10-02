<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useTransactionStore } from '../stores/transactions'
import { useToast } from '../composables/useToast'

const route = useRoute()
const router = useRouter()
const transactionStore = useTransactionStore()
const toast = useToast()

const fromAccountId = ref('')
const toAccountId = ref('')
const amount = ref('')
const isSubmitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

onMounted(() => {
  if (route.query.from) {
    fromAccountId.value = String(route.query.from)
  }
})

async function handleTransfer() {
  errorMessage.value = ''
  successMessage.value = ''

  const from = parseInt(fromAccountId.value, 10)
  const to = parseInt(toAccountId.value, 10)
  const amt = parseFloat(amount.value)

  if (!from || !to) {
    errorMessage.value = 'Please provide valid Source and Destination Account IDs.'
    return
  }

  if (from === to) {
    errorMessage.value = 'Source and Destination accounts must be different.'
    return
  }

  if (!amt || amt <= 0) {
    errorMessage.value = 'Transfer amount must be greater than zero.'
    return
  }

  isSubmitting.value = true

  try {
    const res = await transactionStore.transfer(from, to, amt)
    successMessage.value = res?.message || 'Transfer completed successfully!'
    toast.success('Funds transferred successfully!')
    amount.value = ''
  } catch (err) {
    errorMessage.value = err.message || 'Transfer failed.'
    toast.error(errorMessage.value)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="page-container">
    <div class="page-header">
      <div>
        <h1 class="page-title">Transfer Funds</h1>
        <p class="page-subtitle">Instantly transfer money between bank accounts</p>
      </div>
      <router-link to="/transactions" class="btn-secondary">
        View Audit Trail &rarr;
      </router-link>
    </div>

    <div class="transfer-card-wrapper">
      <div class="card transfer-card">
        <div v-if="successMessage" class="alert-success" role="alert">
          {{ successMessage }}
        </div>

        <div v-if="errorMessage" class="alert-error" role="alert">
          {{ errorMessage }}
        </div>

        <form @submit.prevent="handleTransfer" class="transfer-form">
          <div class="form-row">
            <div class="form-group">
              <label for="fromAccount">Source Account ID *</label>
              <input
                id="fromAccount"
                v-model="fromAccountId"
                type="number"
                min="1"
                required
                placeholder="e.g. 1"
                class="form-input font-mono"
              />
              <span class="field-hint">Account ID from which funds will be deducted</span>
            </div>

            <div class="transfer-arrow-col">
              <div class="transfer-arrow">&rarr;</div>
            </div>

            <div class="form-group">
              <label for="toAccount">Destination Account ID *</label>
              <input
                id="toAccount"
                v-model="toAccountId"
                type="number"
                min="1"
                required
                placeholder="e.g. 2"
                class="form-input font-mono"
              />
              <span class="field-hint">Account ID to which funds will be deposited</span>
            </div>
          </div>

          <div class="form-group" style="margin-top: 8px">
            <label for="amount">Transfer Amount *</label>
            <input
              id="amount"
              v-model="amount"
              type="number"
              step="0.01"
              min="0.01"
              required
              placeholder="0.00"
              class="form-input font-mono amount-input"
            />
            <span class="field-hint">Accounts must share the same currency code</span>
          </div>

          <button type="submit" class="btn-primary btn-block btn-lg" :disabled="isSubmitting">
            <span v-if="isSubmitting" class="spinner"></span>
            <span v-else>Execute Transfer</span>
          </button>
        </form>

        <div class="transfer-rules">
          <h4>Transfer Requirements</h4>
          <ul>
            <li>Both accounts must be open and active.</li>
            <li>Source account must have sufficient balance.</li>
            <li>Source and destination accounts must have the same currency (e.g. KGS to KGS).</li>
            <li>All transactions are recorded in the audit log.</li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-container {
  max-width: 800px;
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

.transfer-card-wrapper {
  margin-top: 16px;
}

.transfer-card {
  padding: 32px;
  background: var(--bg-surface);
  border: 1px solid var(--border-color);
  border-radius: 12px;
}

.alert-success {
  background: #dcfce7;
  border: 1px solid #86efac;
  color: #15803d;
  padding: 12px 16px;
  border-radius: 6px;
  font-size: 14px;
  margin-bottom: 20px;
}

.alert-error {
  background: #fee2e2;
  border: 1px solid #fca5a5;
  color: #b91c1c;
  padding: 12px 16px;
  border-radius: 6px;
  font-size: 14px;
  margin-bottom: 20px;
}

.transfer-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-row {
  display: flex;
  align-items: flex-start;
  gap: 16px;
}

@media (max-width: 600px) {
  .form-row {
    flex-direction: column;
  }
  .transfer-arrow-col {
    display: none;
  }
}

.form-group {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.transfer-arrow-col {
  padding-top: 32px;
}

.transfer-arrow {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: var(--bg-surface-alt);
  border: 1px solid var(--border-color);
  color: var(--text-muted);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
}

.form-group label {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-primary);
}

.field-hint {
  font-size: 12px;
  color: var(--text-muted);
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
}

.form-input:focus {
  border-color: var(--accent-color);
}

.amount-input {
  font-size: 20px;
  font-weight: 600;
  padding: 12px 14px;
}

.btn-primary {
  padding: 12px 20px;
  border-radius: 6px;
  border: none;
  background: var(--accent-color);
  color: #fff;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
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

.btn-secondary {
  padding: 8px 16px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-primary);
  font-size: 14px;
  font-weight: 500;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.btn-secondary:hover {
  background: var(--bg-hover);
}

.btn-block {
  width: 100%;
}

.transfer-rules {
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid var(--border-color);
}

.transfer-rules h4 {
  font-size: 13px;
  font-weight: 600;
  text-transform: uppercase;
  color: var(--text-muted);
  letter-spacing: 0.5px;
  margin: 0 0 10px;
}

.transfer-rules ul {
  margin: 0;
  padding-left: 20px;
  font-size: 13px;
  color: var(--text-muted);
  line-height: 1.6;
}

.spinner {
  width: 20px;
  height: 20px;
  border: 2px solid #ffffff;
  border-top-color: transparent;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
