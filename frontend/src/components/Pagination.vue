<script setup>
import { computed } from 'vue'

const props = defineProps({
  currentPage: {
    type: Number,
    required: true
  },
  totalPages: {
    type: Number,
    required: true
  },
  pageSize: {
    type: Number,
    default: 10
  },
  totalItems: {
    type: Number,
    default: 0
  },
  pageSizeOptions: {
    type: Array,
    default: () => [5, 10, 20, 50]
  }
})

const emit = defineEmits(['update:currentPage', 'update:pageSize', 'change'])

const startItem = computed(() => {
  if (props.totalItems === 0) return 0
  return (props.currentPage - 1) * props.pageSize + 1
})

const endItem = computed(() => {
  return Math.min(props.currentPage * props.pageSize, props.totalItems)
})

const visiblePages = computed(() => {
  const current = props.currentPage
  const total = props.totalPages
  const delta = 2

  if (total <= 7) {
    return Array.from({ length: total }, (_, i) => i + 1)
  }

  const range = []
  const left = Math.max(2, current - delta)
  const right = Math.min(total - 1, current + delta)

  range.push(1)

  if (left > 2) {
    range.push('...')
  }

  for (let i = left; i <= right; i++) {
    range.push(i)
  }

  if (right < total - 1) {
    range.push('...')
  }

  range.push(total)
  return range
})

function goToPage(page) {
  if (page === '...' || page < 1 || page > props.totalPages || page === props.currentPage) return
  emit('update:currentPage', page)
  emit('change', { page, pageSize: props.pageSize })
}

function onPageSizeChange(e) {
  const newSize = parseInt(e.target.value, 10)
  emit('update:pageSize', newSize)
  emit('update:currentPage', 1)
  emit('change', { page: 1, pageSize: newSize })
}
</script>

<template>
  <div class="pagination-wrapper" v-if="totalPages > 0">
    <div class="pagination-info">
      <span>
        Showing <strong>{{ startItem }}-{{ endItem }}</strong> of <strong>{{ totalItems }}</strong> items
      </span>
      <div class="page-size-selector">
        <label for="page-size">Per page:</label>
        <select id="page-size" :value="pageSize" @change="onPageSizeChange" class="select-sm">
          <option v-for="opt in pageSizeOptions" :key="opt" :value="opt">
            {{ opt }}
          </option>
        </select>
      </div>
    </div>

    <div class="pagination-controls">
      <button
        class="btn-page"
        :disabled="currentPage <= 1"
        @click="goToPage(currentPage - 1)"
        aria-label="Previous Page"
      >
        &lsaquo; Prev
      </button>

      <template v-for="(p, idx) in visiblePages" :key="idx">
        <span v-if="p === '...'" class="pagination-ellipsis">...</span>
        <button
          v-else
          class="btn-page"
          :class="{ active: p === currentPage }"
          @click="goToPage(p)"
        >
          {{ p }}
        </button>
      </template>

      <button
        class="btn-page"
        :disabled="currentPage >= totalPages"
        @click="goToPage(currentPage + 1)"
        aria-label="Next Page"
      >
        Next &rsaquo;
      </button>
    </div>
  </div>
</template>

<style scoped>
.pagination-wrapper {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 12px;
  padding: 14px 4px;
  font-size: 14px;
  color: var(--text-muted);
}

.pagination-info {
  display: flex;
  align-items: center;
  gap: 16px;
}

.page-size-selector {
  display: flex;
  align-items: center;
  gap: 6px;
}

.select-sm {
  padding: 4px 8px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-primary);
  font-size: 13px;
  cursor: pointer;
  outline: none;
}

.select-sm:focus {
  border-color: var(--accent-color);
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: 4px;
}

.btn-page {
  padding: 6px 12px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-surface);
  color: var(--text-primary);
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.15s ease;
}

.btn-page:hover:not(:disabled):not(.active) {
  background: var(--bg-hover);
  border-color: var(--border-focus);
}

.btn-page.active {
  background: var(--accent-color);
  border-color: var(--accent-color);
  color: #fff;
}

.btn-page:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.pagination-ellipsis {
  padding: 0 4px;
  color: var(--text-muted);
}
</style>
