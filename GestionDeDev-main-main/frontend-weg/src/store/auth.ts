import { reactive, computed } from 'vue'

interface User {
    usuarioId?: number
    name: string
    role: string
    email?: string
}

interface AuthState {
    user: User | null
    token: string | null
    isAuthenticated: boolean
}

const state = reactive<AuthState>({
    user: null,
    token: null,
    isAuthenticated: false
})

export const useAuth = () => {
    const login = (userData: User, token: string) => {
        state.user = userData
        state.token = token
        state.isAuthenticated = true
        localStorage.setItem('weg_auth', JSON.stringify({ user: userData, token }))
    }

    const logout = () => {
        state.user = null
        state.token = null
        state.isAuthenticated = false
        localStorage.removeItem('weg_auth')
    }

    const checkAuth = () => {
        const saved = localStorage.getItem('weg_auth')
        if (saved) {
            const data = JSON.parse(saved)
            state.user = data.user
            state.token = data.token
            state.isAuthenticated = true
        }
    }

    return {
        user: computed(() => state.user),
        token: computed(() => state.token),
        isAuthenticated: computed(() => state.isAuthenticated),
        login,
        logout,
        checkAuth
    }
}
