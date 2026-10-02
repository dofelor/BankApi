<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useClientStore } from '../stores/clients'
import { useToast } from '../composables/useToast'
import Pagination from '../components/Pagination.vue'
import Modal from '../components/Modal.vue'

const router = useRouter()
const clientStore = useClientStore()
const toast = useToast()

const searchQuery = ref('')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const isSubmitting = ref(false)
const clientToDelete = ref(null)
const showDeleteConfirm = ref(false)

// Form states
const createForm = ref({
  firstName: '',
  lastName: '',
  middleName: '',
  email: '',
  birthDate: '1995-01-01',
  phoneNumbers: [
    { phoneNumber: '+996555123456', phoneType: 'Mobile' }
  ]
})

const editForm = ref({
  id: null,
  firstName: '',
  lastName: '',
  middleName: '',
  email: '',
  birthDate: ''
})

onMounted(() => {
  loadClients()
})

async function loadClients(page = clientStore.pageNumber, size = clientStore.pageSize) {
  try {
    await clientStore.fetchClients(page, size)
  } catch (err) {
    toast.error('Failed to load clients: ' + err.message)
  }
}

function handlePaginationChange({ page, pageSize }) {
  loadClients(page, pageSize)
}

const filteredClients = computed(() => {
  if (!searchQuery.value.trim()) return clientStore.clients
  const q = searchQuery.value.toLowerCase()
  return clientStore.clients.filter(c =>
    c.fullName.toLowerCase().includes(q) ||
    c.email.toLowerCase().includes(q) ||
    c.id.toString().includes(q)
  )
})

function openCreateModal() {
  createForm.value = {
    firstName: '',
    lastName: '',
    middleName: '',
    email: '',
    birthDate: '1995-01-01',
    phoneNumbers: [
      { phoneNumber: '+996', phoneType: 'Mobile' }
    ]
  }
  showCreateModal.value = true
}

function addPhoneField() {
  createForm.value.phoneNumbers.push({ phoneNumber: '+996', phoneType: 'Mobile' })
}

function removePhoneField(index) {
  if (createForm.value.phoneNumbers.length > 1) {
    createForm.value.phoneNumbers.splice(index, 1)
  }
}

async function handleCreateClient() {
  isSubmitting.value = true
  try {
    const payload = {
      ...createForm.value,
      phoneNumbers: createForm.value.phoneNumbers.map(p => ({
        phoneNumber: p.phoneNumber.trim(),
        phoneType: p.phoneType
      }))
    }
    await clientStore.createClient(payload)
    toast.success('Client created successfully!')
    showCreateModal.value = false
  } catch (err) {
    toast.error(err.message || 'Failed to create client.')
  } finally {
    isSubmitting.value = false
  }
}

function openEditModal(client) {
  editForm.value = {
    id: client.id,
    firstName: client.fullName.split(' ')[1] || client.fullName,
    lastName: client.fullName.split(' ')[0] || '',
    middleName: client.fullName.split(' ')[2] || '',
    email: client.email,
    birthDate: client.birthDate
  }
  showEditModal.value = true
}

async function handleUpdateClient() {
  isSubmitting.value = true
  try {
    await clientStore.updateClient(editForm.value.id, {
      firstName: editForm.value.firstName,
      lastName: editForm.value.lastName,
      middleName: editForm.value.middleName || null,
      email: editForm.value.email,
      birthDate: editForm.value.birthDate
    })
    toast.success('Client updated successfully!')
    showEditModal.value = false
  } catch (err) {
    toast.error(err.message || 'Failed to update client.')
  } finally {
    isSubmitting.value = false
  }
}

function confirmDelete(client) {
  clientToDelete.value = client
  showDeleteConfirm.value = true
}

async function handleDeleteClient() {
  if (!clientToDelete.value) return
  isSubmitting.value = true
  try {
    await clientStore.deleteClient(clientToDelete.value.id)
    toast.success('Client deleted successfully!')
    showDeleteConfirm.value = false
    clientToDelete.value = null
  } catch (err) {
    toast.error(err.message || 'Cannot delete client.')
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="page-container">
    <div class="page-header">
      <div>
        <h1 class="page-title">Clients Directory</h1>
        <p class="page-subtitle">Manage bank clients, view accounts, cards, and phone records</p>
      </div>
      <button class="btn-primary" @click="openCreateModal">
        <svg viewBox="0 0 20 20" fill="currentColor" width="16" height="16">
          <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
        </svg>
        <span>New Client</span>
      </button>
    </div>

    <!-- Filter & stats bar -->
    <div class="table-toolbar">
      <div class="search-box">
        <svg viewBox="0 0 20 20" fill="currentColor" width="16" height="16" class="search-icon">
          <path fill-rule="evenodd" d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z" clip-rule="evenodd" />
        </svg>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Filter by name, email, or ID..."
          class="search-input"
        />
      </div>

      <div class="stats-badge">
        Total Clients: <strong>{{ clientStore.totalCount }}</strong>
      </div>
    </div>

    <!-- Clients Table Card -->
    <div class="card table-card">
      <div v-if="clientStore.loading && clientStore.clients.length === 0" class="loading-state">
        <div class="spinner-large"></div>
        <p>Loading clients...</p>
      </div>

      <div v-else-if="filteredClients.length === 0" class="empty-state">
        <svg viewBox="0 0 24 24" width="48" height="48" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2M9 11a4 4 0 100-8 4 4 0 000 8zM23 21v-2a4 4 0 00-3-3.87M16 3.13a4 4 0 010 7.75" />
        </svg>
        <h3>No clients found</h3>
        <p v-if="searchQuery">No clients match your search query "{{ searchQuery }}".</p>
        <p v-else>Get started by creating your first client.</p>
        <button class="btn-primary btn-sm" @click="openCreateModal" v-if="!searchQuery">
          Create Client
        </button>
      </div>

      <div v-else class="table-responsive">
        <table class="data-table">
          <thead>
            <tr>
              <th style="width: 60px">ID</th>
              <th>Client Name</th>
              <th>Email</th>
              <th>Date of Birth</th>
              <th>Accounts</th>
              <th>Phones</th>
              <th style="text-align: right; width: 170px">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="client in filteredClients" :key="client.id">
              <td class="font-mono text-muted">#{{ client.id }}</td>
              <td class="font-medium text-primary">
                <router-link :to="`/clients/${client.id}`" class="client-link">
                  {{ client.fullName }}
                </router-link>
              </td>
              <td>{{ client.email }}</td>
              <td class="text-muted">{{ client.birthDate }}</td>
              <td>
                <span class="count-pill">
                  {{ client.accounts ? client.accounts.length : 0 }} accounts
                </span>
              </td>
              <td>
                <span class="count-pill">
                  {{ client.phoneNumbers ? client.phoneNumbers.length : 0 }} phones
                </span>
              </td>
              <td style="text-align: right">
                <div class="action-buttons">
                  <router-link :to="`/clients/${client.id}`" class="btn-icon" title="View & Manage">
                    <svg viewBox="0 0 20 20" fill="currentColor" width="16" height="16">
                      <path d="M10 12a2 2 0 100-4 2 2 0 000 4z" />
                      <path fill-rule="evenodd" d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z" clip-rule="evenodd" />
                    </svg>
                  </router-link>
                  <button class="btn-icon" @click="openEditModal(client)" title="Edit Client">
                    <svg viewBox="0 0 20 20" fill="currentColor" width="16" height="16">
                      <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                    </svg>
                  </button>
                  <button class="btn-icon btn-danger-icon" @click="confirmDelete(client)" title="Delete Client">
                    <svg viewBox="0 0 20 20" fill="currentColor" width="16" height="16">
                      <path fill-rule="evenodd" d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z" clip-rule="evenodd" />
                    </svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Integrated Pagination -->
        <Pagination
          :current-page="clientStore.pageNumber"
          :total-pages="clientStore.totalPages"
          :page-size="clientStore.pageSize"
          :total-items="clientStore.totalCount"
          @change="handlePaginationChange"
        />
      </div>
    </div>

    <!-- Create Client Modal -->
    <Modal :show="showCreateModal" title="Create New Client" @close="showCreateModal = false" max-width="600px">
      <form @submit.prevent="handleCreateClient" id="createClientForm" class="modal-form">
        <div class="form-row">
          <div class="form-group">
            <label for="createLastName">Last Name *</label>
            <input id="createLastName" v-model="createForm.lastName" type="text" required placeholder="Иванов" class="form-input" />
          </div>
          <div class="form-group">
            <label for="createFirstName">First Name *</label>
            <input id="createFirstName" v-model="createForm.firstName" type="text" required placeholder="Иван" class="form-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="createMiddleName">Middle Name</label>
            <input id="createMiddleName" v-model="createForm.middleName" type="text" placeholder="Иванович" class="form-input" />
          </div>
          <div class="form-group">
            <label for="createBirthDate">Birth Date * (Must be 18+)</label>
            <input id="createBirthDate" v-model="createForm.birthDate" type="date" required class="form-input" />
          </div>
        </div>

        <div class="form-group">
          <label for="createEmail">Email Address *</label>
          <input id="createEmail" v-model="createForm.email" type="email" required placeholder="client@example.com" class="form-input" />
        </div>

        <div class="form-divider">
          <span>Phone Numbers (At least one required)</span>
        </div>

        <div v-for="(phone, idx) in createForm.phoneNumbers" :key="idx" class="phone-input-row">
          <input
            v-model="phone.phoneNumber"
            type="text"
            required
            placeholder="+996XXXXXXXXX"
            class="form-input flex-1"
          />
          <select v-model="phone.phoneType" class="form-input select-type">
            <option value="Mobile">Mobile</option>
            <option value="Home">Home</option>
            <option value="Work">Work</option>
          </select>
          <button
            type="button"
            class="btn-icon btn-danger-icon"
            @click="removePhoneField(idx)"
            :disabled="createForm.phoneNumbers.length <= 1"
          >
            &times;
          </button>
        </div>

        <button type="button" class="btn-secondary btn-sm" @click="addPhoneField" style="margin-top: 6px">
          + Add Another Phone
        </button>
      </form>

      <template #footer>
        <button type="button" class="btn-secondary" @click="showCreateModal = false">Cancel</button>
        <button type="submit" form="createClientForm" class="btn-primary" :disabled="isSubmitting">
          {{ isSubmitting ? 'Creating...' : 'Create Client' }}
        </button>
      </template>
    </Modal>

    <!-- Edit Client Modal -->
    <Modal :show="showEditModal" title="Edit Client Information" @close="showEditModal = false" max-width="520px">
      <form @submit.prevent="handleUpdateClient" id="editClientForm" class="modal-form">
        <div class="form-row">
          <div class="form-group">
            <label for="editLastName">Last Name *</label>
            <input id="editLastName" v-model="editForm.lastName" type="text" required class="form-input" />
          </div>
          <div class="form-group">
            <label for="editFirstName">First Name *</label>
            <input id="editFirstName" v-model="editForm.firstName" type="text" required class="form-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="editMiddleName">Middle Name</label>
            <input id="editMiddleName" v-model="editForm.middleName" type="text" class="form-input" />
          </div>
          <div class="form-group">
            <label for="editBirthDate">Birth Date *</label>
            <input id="editBirthDate" v-model="editForm.birthDate" type="date" required class="form-input" />
          </div>
        </div>

        <div class="form-group">
          <label for="editEmail">Email Address *</label>
          <input id="editEmail" v-model="editForm.email" type="email" required class="form-input" />
        </div>
      </form>

      <template #footer>
        <button type="button" class="btn-secondary" @click="showEditModal = false">Cancel</button>
        <button type="submit" form="editClientForm" class="btn-primary" :disabled="isSubmitting">
          {{ isSubmitting ? 'Saving...' : 'Save Changes' }}
        </button>
      </template>
    </Modal>

    <!-- Delete Confirmation Modal -->
    <Modal :show="showDeleteConfirm" title="Confirm Delete Client" @close="showDeleteConfirm = false" max-width="440px">
      <p>Are you sure you want to delete client <strong>{{ clientToDelete?.fullName }}</strong>?</p>
      <p class="text-muted text-sm" style="margin-top: 8px">
        Note: A client with active open bank accounts cannot be deleted. All bank accounts must be closed first.
      </p>

      <template #footer>
        <button type="button" class="btn-secondary" @click="showDeleteConfirm = false">Cancel</button>
        <button type="button" class="btn-danger" @click="handleDeleteClient" :disabled="isSubmitting">
          {{ isSubmitting ? 'Deleting...' : 'Delete Client' }}
        </button>
      </template>
    </Modal>
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
  letter-spacing: -0.5px;
}

.page-subtitle {
  font-size: 14px;
  color: var(--text-muted);
  margin: 0;
}

.table-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
  gap: 16px;
  flex-wrap: wrap;
}

.search-box {
  position: relative;
  width: 320px;
  max-width: 100%;
}

.search-icon {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--text-muted);
}

.search-input {
  width: 100%;
  padding: 8px 12px 8px 36px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-primary);
  font-size: 14px;
  outline: none;
  box-sizing: border-box;
}

.search-input:focus {
  border-color: var(--accent-color);
}

.stats-badge {
  font-size: 13px;
  color: var(--text-muted);
  background: var(--bg-surface);
  border: 1px solid var(--border-color);
  padding: 6px 12px;
  border-radius: 6px;
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

.client-link {
  color: var(--text-primary);
  text-decoration: none;
  font-weight: 600;
}

.client-link:hover {
  color: var(--accent-color);
  text-decoration: underline;
}

.count-pill {
  display: inline-block;
  padding: 3px 8px;
  border-radius: 9999px;
  background: var(--bg-surface-alt);
  border: 1px solid var(--border-color);
  font-size: 12px;
  color: var(--text-muted);
}

.action-buttons {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 6px;
}

.btn-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-muted);
  cursor: pointer;
  text-decoration: none;
  transition: all 0.15s ease;
}

.btn-icon:hover {
  background: var(--bg-hover);
  color: var(--text-primary);
  border-color: var(--border-focus);
}

.btn-danger-icon:hover {
  background: #fee2e2;
  border-color: #fca5a5;
  color: #dc2626;
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

.modal-form {
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
  padding: 8px 12px;
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

.form-divider {
  border-top: 1px solid var(--border-color);
  padding-top: 10px;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  color: var(--text-muted);
  letter-spacing: 0.5px;
}

.phone-input-row {
  display: flex;
  align-items: center;
  gap: 8px;
}

.select-type {
  width: 110px;
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
  display: inline-flex;
  align-items: center;
  gap: 6px;
  transition: background 0.15s ease;
}

.btn-primary:hover:not(:disabled) {
  background: var(--accent-hover);
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
  transition: background 0.15s ease;
}

.btn-secondary:hover:not(:disabled) {
  background: var(--bg-hover);
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

.btn-danger:hover:not(:disabled) {
  background: #b91c1c;
}

.btn-sm {
  padding: 6px 12px;
  font-size: 13px;
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
