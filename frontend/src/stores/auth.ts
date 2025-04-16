import { defineStore } from 'pinia'
const API_URL = import.meta.env.VITE_API_BASE_URL
import { ref, computed } from 'vue'
import { jwtDecode } from 'jwt-decode'
interface User {
  userId: string
  email: string
  token: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)

  function isTokenExpired(token: string): boolean {
    try {
      const decoded: { exp: number } = jwtDecode(token)
      const currentTime = Date.now() / 1000
      return decoded.exp < currentTime
    } catch {
      return true
    }
  }

  const loadUser = () => {
    const storedUser = localStorage.getItem('user')
    if (storedUser) {
      const parsedUser = JSON.parse(storedUser)
      if (!isTokenExpired(parsedUser.token)) {
        user.value = parsedUser
      } else {
        logout()
      }
    }
  }

  const login = async (credentials: { email: string; password: string }) => {
    try {
      const response = await fetch(`${API_URL}v1/authenticate/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: credentials.email,
          password: credentials.password,
        }),
      })

      if (!response.ok) {
        throw new Error('Invalid username or password')
      }

      const data = await response.json()
      user.value = {
        userId: data.userId,
        email: data.email,
        token: data.accessToken,
      }
      localStorage.setItem('user', JSON.stringify(user.value))
    } catch (error) {
      throw new Error(error instanceof Error ? error.message : 'Failed to login')
    }
  }

  const register = async (credentials: {
    password: string
    confirmPassword: string
    email: string
  }) => {
    try {
      const response = await fetch(`${API_URL}v1/authenticate/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials),
      })

      let errorMessage = 'Registration failed'

      if (!response.ok) {
        const contentType = response.headers.get('content-type')
        if (contentType && contentType.includes('application/json')) {
          const data = await response.json()
          errorMessage = data.message || errorMessage
        } else {
          errorMessage = await response.text()
        }
        throw new Error(errorMessage)
      }
    } catch (error) {
      if (error instanceof Error) {
        throw error
      } else {
        throw new Error('Failed to register')
      }
    }
  }

  const logout = () => {
    user.value = null
    localStorage.removeItem('user')
  }

  const isAuthenticated = computed(() => !!user.value)

  return { user, loadUser, login, register, logout, isAuthenticated }
})
