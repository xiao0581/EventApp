import { defineStore } from 'pinia'

interface User {
  username: string
  email: string
  token: string
}

interface AuthState {
  user: User | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    user: null,
  }),

  actions: {
    loadUser(): void {
      const storedUser = localStorage.getItem('user')
      if (storedUser) {
        this.user = JSON.parse(storedUser)
      }
    },

    async login(credentials: { email: string; password: string }): Promise<void> {
      try {
        const response = await fetch('http://localhost:5102/api/v1/authenticate/login', {
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

        this.user = {
          username: data.userId,
          email: data.email,
          token: data.accessToken,
        }
        localStorage.setItem('user', JSON.stringify(this.user))
      } catch (error) {
        if (error instanceof Error) {
          throw new Error(error.message || 'Failed to login')
        } else {
          throw new Error('Failed to login')
        }
      }
    },

    async register(credentials: {
      username: string
      password: string
      confirmPassword: string
      email: string
    }): Promise<void> {
      try {
        const response = await fetch('http://localhost:5102/api/v1/authenticate/register', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(credentials),
        })

        if (!response.ok) {
          throw new Error('Registration failed')
        }
      } catch (error) {
        if (error instanceof Error) {
          throw new Error(error.message || 'Failed to register')
        } else {
          throw new Error('Failed to register')
        }
      }
    },

    logout(): void {
      this.user = null
      localStorage.removeItem('user')
    },

    isAuthenticated(): boolean {
      return !!this.user
    },
  },
})
