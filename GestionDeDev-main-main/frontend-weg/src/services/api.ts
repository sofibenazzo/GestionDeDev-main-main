import axios from 'axios'

const API_BASE_URL = 'http://localhost:5299' // Updated to match active dotnet run port

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Add token to requests
api.interceptors.request.use((config) => {
  const authData = localStorage.getItem('weg_auth')
  if (authData) {
    const { token } = JSON.parse(authData)
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
  }
  return config
})

export default api
