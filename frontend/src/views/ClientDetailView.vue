<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useClientStore } from '../stores/clients'
import { useAccountStore } from '../stores/accounts'
import { useCardStore } from '../stores/cards'
import { usePhoneStore } from '../stores/phones'
import { useAuthStore } from '../stores/auth'
import { useToast } from '../composables/useToast'
import StatusBadge from '../components/StatusBadge.vue'
import Modal from '../components/Modal.vue'

const route = useRoute()
const router = useRouter()
const clientId = computed(() => parseInt(route.params.id, 10))

const clientStore = useClientStore()
const accountStore = useAccountStore()
const cardStore = useCardStore()
const phoneStore = usePhoneStore()
const authStore = useAuthStore()
const toast = useToast()

const isLoading = ref(true)
const isSubmitting = ref(false)

// Modals
const showOpenAccountModal = ref(false)
const showDepositModal = ref(false)
const showIssueCardModal = ref(false)
const showAddPhoneModal = ref(false)
const showCloseAccountConfirm = ref(false)

// Forms state
const targetAccount = ref(null)
const depositForm = ref({ amount: 100 })
const openAccountForm = ref({ currency: 'KGS' })
const issueCardForm = ref({ cardType: 'Debit', validityYears: 3 })
const addPhoneForm = ref({ phoneNumber: '+996', phoneType: 'Mobile' })

onMounted(async () => {
  await loadClientData()
})

async function loadClientData() {
  isLoading.value = true
  try {
    await clientStore.fetchClientById(clientId.value)
    await accountStore.fetchAccountsByClientId(clientId.value)
    await phoneStore.fetchPhonesByClientId(clientId.value)

    // Load cards for each account
    if (accountStore.accounts) {
      for (const acc of accountStore.accounts) {
        await cardStore.fetchCardsByAccountId(acc.id)
      }
    }
  } catch (err) {
    toast.error('Failed to load client details: ' + err.message)
  } finally {
    isLoading.value = false
  }
}

// Account actions
async function handleOpenAccount() {
  isSubmitting.value = true
  try {
    await accountStore.createAccount(clientId.value, openAccountForm.value.currency)
    toast.success('Bank account created successfully!')
    showOpenAccountModal.value = false
  } catch (err) {
    toast.error(err.message || 'Failed to open account.')
  } finally {
    isSubmitting.value = false
  }
}

function openDepositModal(acc) {
  targetAccount.value = acc
  depositForm.value.amount = 500
  showDepositModal.value = true
}

async function handleDeposit() {
  if (!targetAccount.value) return
  isSubmitting.value = true
  try {
    await accountStore.deposit(targetAccount.value.id, depositForm.value.amount, clientId.value)
    toast.success(`Successfully deposited ${depositForm.value.amount} ${targetAccount.value.currency}!`)
    showDepositModal.value = false
  } catch (err) {
    toast.error(err.message || 'Failed to deposit.')
  } finally {
    isSubmitting.value = false
  }
}

function confirmCloseAccount(acc) {
  targetAccount.value = acc
  showCloseAccountConfirm.value = true
}

async function handleCloseAccount() {
  if (!targetAccount.value) return
  isSubmitting.value = true
  try {
    await accountStore.closeAccount(targetAccount.value.id, clientId.value)
    toast.success('Bank account closed.')
    showCloseAccountConfirm.value = false
  } catch (err) {
    toast.error(err.message || 'Cannot close account.')
  } finally {
    isSubmitting.value = false
  }
}

// Card actions
function openIssueCardModal(acc) {
  targetAccount.value = acc
  issueCardForm.value = { cardType: 'Debit', validityYears: 3 }
  showIssueCardModal.value = true
}

async function handleIssueCard() {
  if (!targetAccount.value) return
  isSubmitting.value = true
  try {
    const today = new Date()
    const expYear = today.getFullYear() + parseInt(issueCardForm.value.validityYears, 10)
    const month = String(today.getMonth() + 1).padStart(2, '0')
    const day = String(today.getDate()).padStart(2, '0')
    const validityPeriod = `${expYear}-${month}-${day}`

    await cardStore.createCard(targetAccount.value.id, {
      cardType: issueCardForm.value.cardType,
      validityPeriod
    })

    toast.success('Card issued successfully!')
    showIssueCardModal.value = false
  } catch (err) {
    toast.error(err.message || 'Failed to issue card.')
  } finally {
    isSubmitting.value = false
  }
}

async function toggleCardBlock(card, accountId) {
  try {
    if (card.isBlocked) {
      await cardStore.unblockCard(card.id, accountId)
      toast.success('Card unblocked.')
    } else {
      await cardStore.blockCard(card.id, accountId)
      toast.info('Card blocked.')
    }
  } catch (err) {
    toast.error(err.message || 'Failed to update card status.')
  }
}

// Phone actions
async function handleAddPhone() {
  isSubmitting.value = true
  try {
    await phoneStore.createPhone(clientId.value, {
      phoneNumber: addPhoneForm.value.phoneNumber.trim(),
      phoneType: addPhoneForm.value.phoneType
    })
    toast.success('Phone number added!')
    showAddPhoneModal.value = false
    addPhoneForm.value.phoneNumber = '+996'
  } catch (err) {
    toast.error(err.message || 'Failed to add phone.')
  } finally {
    isSubmitting.value = false
  }
}

async function handleDeletePhone(phoneId) {
  try {
    await phoneStore.deletePhone(phoneId, clientId.value)
    toast.success('Phone number removed.')
  } catch (err) {
    toast.error(err.message || 'Cannot delete phone.')
  }
}

function formatCardNumber(num) {
  if (!num) return '•••• •••• •••• ••••'
  return num.replace(/(\d{4})/g, '$1 ').trim()
}
</script>

<template>
  <div class="page-container">
    <!-- Back breadcrumb -->
    <div class="breadcrumb">
      <router-link to="/clients" class="back-link">
        &larr; Back to Clients
      </router-link>
    </div>

    <div v-if="isLoading" class="loading-state">
      <div class="spinner-large"></div>
      <p>Loading client profile...</p>
    </div>

    <div v-else-if="!clientStore.currentClient" class="empty-state">
      <h3>Client not found</h3>
      <router-link to="/clients" class="btn-primary">Return to Directory</router-link>
    </div>

    <div v-else class="client-detail-layout">
      <!-- Client Profile Summary Card -->
      <div class="card client-summary-card">
        <div class="client-hero">
          <div class="client-avatar">
            {{ clientStore.currentClient.fullName.charAt(0) }}
          </div>
          <div class="client-info">
            <h1 class="client-name">{{ clientStore.currentClient.fullName }}</h1>
            <div class="client-meta">
              <span><strong>Email:</strong> {{ clientStore.currentClient.email }}</span>
              <span class="meta-dot">&bull;</span>
              <span><strong>Birth Date:</strong> {{ clientStore.currentClient.birthDate }}</span>
              <span class="meta-dot">&bull;</span>
              <span class="font-mono"><strong>Client ID:</strong> #{{ clientStore.currentClient.id }}</span>
            </div>
          </div>
        </div>

        <div class="quick-stats">
          <div class="stat-box">
            <span class="stat-number">{{ accountStore.accounts.length }}</span>
            <span class="stat-label">Accounts</span>
          </div>
          <div class="stat-box">
            <span class="stat-number">{{ phoneStore.phones.length }}</span>
            <span class="stat-label">Phones</span>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="detail-grid">
        <!-- Section: Bank Accounts -->
        <div class="grid-main">
          <div class="section-header">
            <div>
              <h2 class="section-title">Bank Accounts</h2>
              <p class="section-subtitle">Manage accounts, cards, balances, and deposits</p>
            </div>
            <!-- Только Admin может открывать новые счета -->
            <button v-if="authStore.isAdmin" class="btn-primary btn-sm" @click="showOpenAccountModal = true">
              + Open Account
            </button>
          </div>

          <div v-if="accountStore.accounts.length === 0" class="card empty-card">
            <p>This client has no active bank accounts.</p>
            <button v-if="authStore.isAdmin" class="btn-secondary btn-sm" @click="showOpenAccountModal = true">
              Open First Account
            </button>
          </div>

          <div class="accounts-list">
            <div
              v-for="acc in accountStore.accounts"
              :key="acc.id"
              class="card account-card"
              :class="{ 'account-closed': acc.isClosed }"
            >
              <div class="account-card-header">
                <div>
                  <div class="account-badge-row">
                    <span class="account-number font-mono">{{ acc.accountNumber }}</span>
                    <StatusBadge :status="acc.isClosed" type="account" />
                  </div>
                  <span class="account-id-hint font-mono">Account ID #{{ acc.id }}</span>
                </div>

                <div class="account-balance-box">
                  <span class="balance-value">{{ acc.balance.toLocaleString('en-US', { minimumFractionDigits: 2 }) }}</span>
                  <span class="balance-currency">{{ acc.currency }}</span>
                </div>
              </div>

              <!-- Account Actions Toolbar -->
              <div class="account-card-toolbar">
                <div class="account-actions">
                  <!-- Пополнение баланса — только Admin -->
                  <button
                    v-if="authStore.isAdmin"
                    class="btn-sm btn-secondary"
                    :disabled="acc.isClosed"
                    @click="openDepositModal(acc)"
                  >
                    + Deposit Funds
                  </button>
                  <!-- Выпуск карты — только Admin -->
                  <button
                    v-if="authStore.isAdmin"
                    class="btn-sm btn-secondary"
                    :disabled="acc.isClosed"
                    @click="openIssueCardModal(acc)"
                  >
                    + Issue Card
                  </button>
                  <!-- Перевод — доступен всем -->
                  <router-link
                    :to="{ path: '/transfers', query: { from: acc.id } }"
                    class="btn-sm btn-secondary"
                    v-if="!acc.isClosed && acc.balance > 0"
                  >
                    Transfer Out &rarr;
                  </router-link>
                </div>

                <button
                  v-if="!acc.isClosed"
                  class="btn-sm btn-outline-danger"
                  @click="confirmCloseAccount(acc)"
                  title="Close Account"
                >
                  Close Account
                </button>
              </div>

              <!-- Nested Cards Section -->
              <div class="cards-section">
                <div class="cards-section-header">
                  <span class="cards-title">Linked Payment Cards</span>
                </div>

                <div
                  v-if="!cardStore.cardsByAccount[acc.id] || cardStore.cardsByAccount[acc.id].length === 0"
                  class="empty-cards-hint"
                >
                  No cards issued for this account yet.
                </div>

                <div v-else class="cards-grid">
                  <div
                    v-for="card in cardStore.cardsByAccount[acc.id]"
                    :key="card.id"
                    class="bank-card-item"
                    :class="{ 'card-blocked': card.isBlocked }"
                  >
                    <div class="card-item-top">
                      <StatusBadge :status="card.cardType" type="card-type" />
                      <StatusBadge :status="card.isBlocked" type="card" />
                    </div>

                    <div class="card-item-number font-mono">
                      {{ formatCardNumber(card.cardNumber) }}
                    </div>

                    <div class="card-item-bottom">
                      <div class="card-validity">
                        <span class="label">EXP</span>
                        <span class="value">{{ card.validityPeriod }}</span>
                      </div>

                      <button
                        class="btn-text-action"
                        :class="card.isBlocked ? 'text-success' : 'text-danger'"
                        @click="toggleCardBlock(card, acc.id)"
                      >
                        {{ card.isBlocked ? 'Unblock' : 'Block' }}
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Section: Phone Numbers & Quick Info -->
        <div class="grid-side">
          <div class="card side-card">
            <div class="side-card-header">
              <h3 class="side-title">Phone Numbers</h3>
              <!-- Добавлять телефоны — только Admin -->
              <button v-if="authStore.isAdmin" class="btn-sm btn-secondary" @click="showAddPhoneModal = true">
                + Add Phone
              </button>
            </div>

            <div v-if="phoneStore.phones.length === 0" class="empty-side-hint">
              No registered phone numbers.
            </div>

            <ul v-else class="phones-list">
              <li v-for="phone in phoneStore.phones" :key="phone.id" class="phone-item">
                <div class="phone-info">
                  <span class="phone-number font-mono">{{ phone.phoneNumber }}</span>
                  <span class="phone-type-badge">{{ phone.phoneType }}</span>
                </div>
                <!-- Удалять телефоны — только Admin -->
                <button
                  v-if="authStore.isAdmin"
                  class="btn-icon btn-danger-icon"
                  @click="handleDeletePhone(phone.id)"
                  title="Delete Phone"
                >
                  &times;
                </button>
              </li>
            </ul>
          </div>

          <!-- Shortcuts Card -->
          <div class="card side-card">
            <h3 class="side-title">Quick Actions</h3>
            <div class="side-actions-list">
              <router-link to="/transfers" class="side-action-link">
                <span>&bull; Open Transfer Portal</span>
              </router-link>
              <!-- Логи транзакций — только Admin -->
              <router-link v-if="authStore.isAdmin" to="/transactions" class="side-action-link">
                <span>&bull; View Transaction Audit Logs</span>
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal: Open Bank Account -->
    <Modal :show="showOpenAccountModal" title="Open New Bank Account" @close="showOpenAccountModal = false">
      <form @submit.prevent="handleOpenAccount" id="openAccountForm" class="modal-form">
        <p class="text-muted text-sm">
          A new bank account number will be generated automatically.
        </p>

        <div class="form-group">
          <label for="accCurrency">Account Currency</label>
          <select id="accCurrency" v-model="openAccountForm.currency" class="form-input">
            <option value="KGS">KGS - Kyrgyzstani Som</option>
            <option value="USD">USD - US Dollar</option>
            <option value="EUR">EUR - Euro</option>
            <option value="RUB">RUB - Russian Ruble</option>
            <option value="KZT">KZT - Kazakhstani Tenge</option>
          </select>
        </div>
      </form>

      <template #footer>
        <button type="button" class="btn-secondary" @click="showOpenAccountModal = false">Cancel</button>
        <button type="submit" form="openAccountForm" class="btn-primary" :disabled="isSubmitting">
          {{ isSubmitting ? 'Opening...' : 'Open Account' }}
        </button>
      </template>
    </Modal>

    <!-- Modal: Deposit Funds -->
    <Modal :show="showDepositModal" title="Deposit Funds to Account" @close="showDepositModal = false">
      <form @submit.prevent="handleDeposit" id="depositForm" class="modal-form">
        <div class="account-summary-pill font-mono">
          Account: {{ targetAccount?.accountNumber }} ({{ targetAccount?.currency }})
        </div>

        <div class="form-group">
          <label for="depositAmount">Deposit Amount</label>
          <input
            id="depositAmount"
            v-model.number="depositForm.amount"
            type="number"
            min="1"
            step="0.01"
            required
            class="form-input font-mono"
          />
        </div>
      </form>

      <template #footer>
        <button type="button" class="btn-secondary" @click="showDepositModal = false">Cancel</button>
        <button type="submit" form="depositForm" class="btn-primary" :disabled="isSubmitting">
          {{ isSubmitting ? 'Depositing...' : 'Confirm Deposit' }}
        </button>
      </template>
    </Modal>

    <!-- Modal: Issue Card -->
    <Modal :show="showIssueCardModal" title="Issue Payment Card" @close="showIssueCardModal = false">
      <form @submit.prevent="handleIssueCard" id="issueCardForm" class="modal-form">
        <div class="account-summary-pill font-mono">
          Account: {{ targetAccount?.accountNumber }}
        </div>

        <div class="form-group">
          <label for="cardType">Card Type</label>
          <select id="cardType" v-model="issueCardForm.cardType" class="form-input">
            <option value="Debit">Debit Card</option>
            <option value="Credit">Credit Card</option>
          </select>
        </div>

        <div class="form-group">
          <label for="validityYears">Validity Period (Years)</label>
          <select id="validityYears" v-model="issueCardForm.validityYears" class="form-input">
            <option :value="1">1 Year</option>
            <option :value="2">2 Years</option>
            <option :value="3">3 Years</option>
            <option :value="5">5 Years</option>
          </select>
        </div>
      </form>

      <template #footer>
        <button type="button" class="btn-secondary" @click="showIssueCardModal = false">Cancel</button>
        <button type="submit" form="issueCardForm" class="btn-primary" :disabled="isSubmitting">
          {{ isSubmitting ? 'Issuing...' : 'Issue Card' }}
        </button>
      </template>
    </Modal>

    <!-- Modal: Add Phone -->
    <Modal :show="showAddPhoneModal" title="Add Phone Number" @close="showAddPhoneModal = false">
      <form @submit.prevent="handleAddPhone" id="addPhoneForm" class="modal-form">
        <div class="form-group">
          <label for="phoneNum">Phone Number (Kyrgyzstan format: +996XXXXXXXXX)</label>
          <input
            id="phoneNum"
            v-model="addPhoneForm.phoneNumber"
            type="text"
            required
            placeholder="+996555123456"
            class="form-input font-mono"
          />
        </div>

        <div class="form-group">
          <label for="phoneType">Phone Type</label>
          <select id="phoneType" v-model="addPhoneForm.phoneType" class="form-input">
            <option value="Mobile">Mobile</option>
            <option value="Home">Home</option>
            <option value="Work">Work</option>
          </select>
        </div>
      </form>

      <template #footer>
        <button type="button" class="btn-secondary" @click="showAddPhoneModal = false">Cancel</button>
        <button type="submit" form="addPhoneForm" class="btn-primary" :disabled="isSubmitting">
          {{ isSubmitting ? 'Adding...' : 'Add Phone' }}
        </button>
      </template>
    </Modal>

    <!-- Modal: Confirm Close Account -->
    <Modal :show="showCloseAccountConfirm" title="Confirm Close Account" @close="showCloseAccountConfirm = false" max-width="440px">
      <p>Are you sure you want to close account <strong class="font-mono">{{ targetAccount?.accountNumber }}</strong>?</p>
      <p class="text-muted text-sm" style="margin-top: 8px">
        Closing the account will disable all operations on it and prevent new cards from being issued.
      </p>

      <template #footer>
        <button type="button" class="btn-secondary" @click="showCloseAccountConfirm = false">Cancel</button>
        <button type="button" class="btn-danger" @click="handleCloseAccount" :disabled="isSubmitting">
          {{ isSubmitting ? 'Closing...' : 'Close Account' }}
        </button>
      </template>
    </Modal>
  </div>
</template>

<style scoped>
.page-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 24px 20px 48px;
}

.breadcrumb {
  margin-bottom: 16px;
}

.back-link {
  color: var(--text-muted);
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
  display: inline-flex;
  align-items: center;
  gap: 4px;
  transition: color 0.15s;
}

.back-link:hover {
  color: var(--accent-color);
}

.client-summary-card {
  padding: 24px;
  margin-bottom: 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 20px;
}

.client-hero {
  display: flex;
  align-items: center;
  gap: 18px;
}

.client-avatar {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: var(--accent-light);
  color: var(--accent-color);
  font-size: 24px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
}

.client-name {
  font-size: 22px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 6px;
  letter-spacing: -0.5px;
}

.client-meta {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 13px;
  color: var(--text-muted);
  flex-wrap: wrap;
}

.meta-dot {
  color: var(--border-color);
}

.quick-stats {
  display: flex;
  gap: 24px;
}

.stat-box {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stat-number {
  font-size: 24px;
  font-weight: 700;
  color: var(--text-primary);
}

.stat-label {
  font-size: 12px;
  color: var(--text-muted);
}

.detail-grid {
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 24px;
  align-items: start;
}

@media (max-width: 900px) {
  detail-grid {
    grid-template-columns: 1fr;
  }
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}

.section-title {
  font-size: 18px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 2px;
}

.section-subtitle {
  font-size: 13px;
  color: var(--text-muted);
  margin: 0;
}

.empty-card {
  padding: 32px;
  text-align: center;
  color: var(--text-muted);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}

.accounts-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.account-card {
  padding: 20px;
  border-radius: 10px;
  transition: all 0.15s;
}

.account-closed {
  opacity: 0.75;
  background: var(--bg-surface-alt);
}

.account-card-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 16px;
}

.account-badge-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 4px;
}

.account-number {
  font-size: 16px;
  font-weight: 700;
  color: var(--text-primary);
  letter-spacing: 0.5px;
}

.account-id-hint {
  font-size: 12px;
  color: var(--text-muted);
}

.account-balance-box {
  text-align: right;
}

.balance-value {
  font-size: 22px;
  font-weight: 700;
  color: var(--text-primary);
}

.balance-currency {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-muted);
  margin-left: 6px;
}

.account-card-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 12px 0;
  border-top: 1px solid var(--border-color);
  border-bottom: 1px solid var(--border-color);
  flex-wrap: wrap;
}

.account-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.cards-section {
  margin-top: 14px;
}

.cards-section-header {
  margin-bottom: 10px;
}

.cards-title {
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  color: var(--text-muted);
  letter-spacing: 0.5px;
}

.empty-cards-hint {
  font-size: 13px;
  color: var(--text-muted);
  font-style: italic;
  padding: 4px 0;
}

.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: 12px;
}

.bank-card-item {
  background: var(--bg-surface-alt);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 14px 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.bank-card-item.card-blocked {
  background: #fef2f2;
  border-color: #fecaca;
}

.card-item-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.card-item-number {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-primary);
  letter-spacing: 1px;
}

.card-item-bottom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 12px;
}

.card-validity .label {
  color: var(--text-muted);
  font-size: 10px;
  margin-right: 4px;
}

.card-validity .value {
  color: var(--text-primary);
  font-weight: 500;
}

.btn-text-action {
  background: transparent;
  border: none;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  padding: 2px 6px;
  border-radius: 4px;
}

.btn-text-action:hover {
  text-decoration: underline;
}

.text-danger { color: #dc2626; }
.text-success { color: #16a34a; }

.side-card {
  padding: 20px;
  margin-bottom: 20px;
}

.side-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 14px;
}

.side-title {
  font-size: 16px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0;
}

.empty-side-hint {
  font-size: 13px;
  color: var(--text-muted);
  text-align: center;
  padding: 16px 0;
}

.phones-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.phone-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 10px;
  border-radius: 6px;
  background: var(--bg-surface-alt);
  border: 1px solid var(--border-color);
}

.phone-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.phone-number {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-primary);
}

.phone-type-badge {
  font-size: 11px;
  color: var(--text-muted);
}

.side-actions-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 12px;
}

.side-action-link {
  font-size: 14px;
  color: var(--accent-color);
  text-decoration: none;
  font-weight: 500;
}

.side-action-link:hover {
  text-decoration: underline;
}

.account-summary-pill {
  background: var(--bg-surface-alt);
  border: 1px solid var(--border-color);
  border-radius: 6px;
  padding: 8px 12px;
  font-size: 13px;
  color: var(--text-primary);
  margin-bottom: 12px;
}

.btn-outline-danger {
  border: 1px solid #fca5a5;
  background: transparent;
  color: #dc2626;
  border-radius: 6px;
  padding: 6px 12px;
  font-size: 13px;
  cursor: pointer;
  transition: all 0.15s;
}

.btn-outline-danger:hover {
  background: #fee2e2;
}

.card {
  background: var(--bg-surface);
  border: 1px solid var(--border-color);
  border-radius: 10px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.modal-form {
  display: flex;
  flex-direction: column;
  gap: 14px;
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
  padding: 8px 12px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-primary);
  font-size: 14px;
  outline: none;
  box-sizing: border-box;
}

.btn-primary {
  padding: 8px 16px;
  border-radius: 6px;
  border: none;
  background: var(--accent-color);
  color: #fff;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
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
}

.btn-danger {
  padding: 8px 16px;
  border-radius: 6px;
  border: none;
  background: #dc2626;
  color: #fff;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
}

.btn-sm {
  padding: 6px 12px;
  font-size: 13px;
}

.btn-icon {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-muted);
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.btn-danger-icon:hover {
  background: #fee2e2;
  border-color: #fca5a5;
  color: #dc2626;
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
