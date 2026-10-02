<script setup>
import { computed } from 'vue'

const props = defineProps({
  status: {
    type: [String, Boolean],
    required: true
  },
  type: {
    type: String,
    default: 'status' // 'account', 'card', 'card-type', or 'phone-type'
  }
})

const badgeClass = computed(() => {
  if (props.type === 'account') {
    return props.status ? 'badge-danger' : 'badge-success' // isClosed
  }
  if (props.type === 'card') {
    return props.status ? 'badge-danger' : 'badge-success' // isBlocked
  }
  if (props.type === 'card-type') {
    return String(props.status).toLowerCase() === 'credit' ? 'badge-purple' : 'badge-blue'
  }
  return 'badge-gray'
})

const badgeText = computed(() => {
  if (props.type === 'account') {
    return props.status ? 'Closed' : 'Active'
  }
  if (props.type === 'card') {
    return props.status ? 'Blocked' : 'Active'
  }
  return String(props.status)
})
</script>

<template>
  <span class="badge" :class="badgeClass">
    <span class="badge-dot" v-if="type === 'account' || type === 'card'"></span>
    {{ badgeText }}
  </span>
</template>

<style scoped>
.badge {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  padding: 2px 8px;
  border-radius: 9999px;
  font-size: 12px;
  font-weight: 500;
  line-height: 1.4;
}

.badge-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
}

.badge-success {
  background: #dcfce7;
  color: #15803d;
}
.badge-success .badge-dot {
  background: #16a34a;
}

.badge-danger {
  background: #fee2e2;
  color: #b91c1c;
}
.badge-danger .badge-dot {
  background: #dc2626;
}

.badge-blue {
  background: #e0f2fe;
  color: #0369a1;
}

.badge-purple {
  background: #f3e8ff;
  color: #7e22ce;
}

.badge-gray {
  background: #f1f5f9;
  color: #475569;
}
</style>
