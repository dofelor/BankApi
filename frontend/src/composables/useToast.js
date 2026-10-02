import { ref } from 'vue'

const toasts = ref([])
let idCounter = 0

export function useToast() {
  const show = (message, type = 'info', duration = 3500) => {
    const id = ++idCounter
    toasts.value.push({ id, message, type })

    setTimeout(() => {
      remove(id)
    }, duration)
  }

  const remove = (id) => {
    const idx = toasts.value.findIndex(t => t.id === id)
    if (idx !== -1) {
      toasts.value.splice(idx, 1)
    }
  }

  const success = (msg, duration) => show(msg, 'success', duration)
  const error = (msg, duration) => show(msg, 'error', duration || 4500)
  const info = (msg, duration) => show(msg, 'info', duration)

  return {
    toasts,
    show,
    remove,
    success,
    error,
    info
  }
}
