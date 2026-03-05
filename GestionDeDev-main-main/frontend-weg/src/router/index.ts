import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '../store/auth'
import DashboardView from '../views/DashboardView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue')
    },
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true }
    },
    {
      path: '/returns/new',
      name: 'new-return',
      component: () => import('../views/NewReturnView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/returns/:id',
      name: 'return-details',
      component: () => import('../views/ReturnDetailsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/returns',
      name: 'returns',
      component: () => import('../views/ReturnsListView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/clients',
      name: 'clients',
      component: () => import('../views/ClientsListView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/clients/new',
      name: 'new-client',
      component: () => import('../views/NewClientView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/clients/:id',
      name: 'client-details',
      component: () => import('../views/ClientDetailsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/products',
      name: 'products',
      component: () => import('../views/ProductsListView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/users',
      name: 'users',
      component: () => import('../views/UsersListView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/reports',
      name: 'reports',
      component: () => import('../views/ReportsView.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

router.beforeEach((to, _from, next) => {
  const auth = useAuth()
  auth.checkAuth() // Verify if session exists in localStorage

  if (to.meta.requiresAuth && !auth.isAuthenticated.value) {
    next('/login')
  } else if ((to.name === 'login' || to.name === 'register') && auth.isAuthenticated.value) {
    next('/')
  } else {
    next()
  }
})

export default router
