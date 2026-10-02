import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ClientsView from '../views/ClientsView.vue'
import ClientDetailView from '../views/ClientDetailView.vue'
import TransfersView from '../views/TransfersView.vue'
import TransactionsView from '../views/TransactionsView.vue'

const routes = [
  {
    path: '/',
    redirect: '/clients'
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: { guestOnly: true }
  },
  {
    path: '/register',
    name: 'Register',
    component: RegisterView,
    meta: { guestOnly: true }
  },
  {
    path: '/clients',
    name: 'Clients',
    component: ClientsView,
    meta: { requiresAuth: true }
  },
  {
    path: '/clients/:id',
    name: 'ClientDetail',
    component: ClientDetailView,
    meta: { requiresAuth: true }
  },
  {
    path: '/transfers',
    name: 'Transfers',
    component: TransfersView,
    meta: { requiresAuth: true }
  },
  {
    path: '/transactions',
    name: 'Transactions',
    component: TransactionsView,
    meta: { requiresAuth: true }
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/clients'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'Login', query: { redirect: to.fullPath } })
  } else if (to.meta.guestOnly && authStore.isAuthenticated) {
    next({ name: 'Clients' })
  } else {
    next()
  }
})

export default router
