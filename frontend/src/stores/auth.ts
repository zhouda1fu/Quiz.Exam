import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { login as loginApi } from '@/api/user'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string>(localStorage.getItem('token') || '')
  const refreshToken = ref<string>(localStorage.getItem('refreshToken') || '')
  const user = ref<{
    userId: string
    name: string
    email: string
  } | null>(null)

  const isAuthenticated = computed(() => !!token.value)

  const setToken = (newToken: string, newRefreshToken: string) => {
    token.value = newToken
    refreshToken.value = newRefreshToken
    localStorage.setItem('token', newToken)
    localStorage.setItem('refreshToken', newRefreshToken)
  }

  const setUser = (userInfo: { userId: string; name: string; email: string }) => {
    user.value = userInfo
  }

  const logout = () => {
    token.value = ''
    refreshToken.value = ''
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
  }

  const login = async (username: string, password: string) => {
    try {
      const response = await loginApi({ username, password })
      const { token: newToken, refreshToken: newRefreshToken, userId, name, email } = response.data
      
      setToken(newToken, newRefreshToken)
      setUser({ userId, name, email })
      
      return response
    } catch (error) {
      throw error
    }
  }

  return {
    token,
    refreshToken,
    user,
    isAuthenticated,
    setToken,
    setUser,
    logout,
    login
  }
}) 