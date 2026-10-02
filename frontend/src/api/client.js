import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL 
  ? `${import.meta.env.VITE_API_URL.replace(/\/$/, '')}/api` 
  : '/api'

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor: attach JWT token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('bank_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Response interceptor: handle 401 and extract human-friendly error messages
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('bank_token')
      localStorage.removeItem('bank_user')
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
    }

    let errorMessage = 'An unexpected error occurred.'

    if (error.response?.data) {
      const data = error.response.data

      // Check standard ProblemDetails or custom { message: ... }
      if (typeof data === 'string') {
        errorMessage = data
      } else if (data.message) {
        errorMessage = data.message
      } else if (data.detail) {
        errorMessage = data.detail
      } else if (data.errors && typeof data.errors === 'object') {
        // FluentValidation / ModelState errors: { "Field": ["Error 1", "Error 2"] }
        const messages = Object.values(data.errors).flat()
        if (messages.length > 0) {
          errorMessage = messages.join(' ')
        }
      } else if (data.title) {
        errorMessage = data.title
      }
    } else if (error.message) {
      errorMessage = error.message
    }

    const enhancedError = new Error(errorMessage)
    enhancedError.originalError = error
    enhancedError.response = error.response
    return Promise.reject(enhancedError)
  }
)

export default apiClient
