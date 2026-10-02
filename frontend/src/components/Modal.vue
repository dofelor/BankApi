<script setup>
import { onMounted, onUnmounted } from 'vue'

const props = defineProps({
  show: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: ''
  },
  maxWidth: {
    type: String,
    default: '520px'
  }
})

const emit = defineEmits(['close'])

function handleKeyDown(e) {
  if (e.key === 'Escape' && props.show) {
    emit('close')
  }
}

onMounted(() => {
  window.addEventListener('keydown', handleKeyDown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeyDown)
})
</script>

<template>
  <Teleport to="body">
    <Transition name="modal-fade">
      <div v-if="show" class="modal-backdrop" @click.self="emit('close')">
        <div class="modal-dialog" :style="{ maxWidth }">
          <div class="modal-header">
            <h3 class="modal-title">{{ title }}</h3>
            <button class="modal-close" @click="emit('close')" aria-label="Close modal">
              &times;
            </button>
          </div>

          <div class="modal-body">
            <slot />
          </div>

          <div class="modal-footer" v-if="$slots.footer">
            <slot name="footer" />
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.45);
  backdrop-filter: blur(2px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
  padding: 16px;
}

.modal-dialog {
  background: var(--bg-surface);
  border-radius: 12px;
  width: 100%;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  border: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
  max-height: 90vh;
  overflow: hidden;
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 18px 24px;
  border-bottom: 1px solid var(--border-color);
}

.modal-title {
  margin: 0;
  font-size: 17px;
  font-weight: 600;
  color: var(--text-primary);
}

.modal-close {
  background: transparent;
  border: none;
  font-size: 24px;
  line-height: 1;
  color: var(--text-muted);
  cursor: pointer;
  padding: 0 4px;
  border-radius: 4px;
  transition: color 0.15s;
}

.modal-close:hover {
  color: var(--text-primary);
}

.modal-body {
  padding: 20px 24px;
  overflow-y: auto;
}

.modal-footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 10px;
  padding: 16px 24px;
  border-top: 1px solid var(--border-color);
  background: var(--bg-surface-alt);
}

/* Animations */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.2s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-active .modal-dialog {
  transition: transform 0.2s ease;
}

.modal-fade-enter-from .modal-dialog {
  transform: scale(0.96);
}
</style>
