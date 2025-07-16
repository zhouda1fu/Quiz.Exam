import api from './index'

// 用户登录
export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  refreshToken: string
  userId: string
  name: string
  email: string
}

export const login = (data: LoginRequest) => {
  return api.post<LoginResponse>('/user/login', data)
}

// 用户注册
export interface RegisterRequest {
  name: string
  email: string
  password: string
  phone: string
  realName: string
  status: number
  roleIds: string[]
}

export interface RegisterResponse {
  userId: string
  name: string
  email: string
}

export const register = (data: RegisterRequest) => {
  return api.post<RegisterResponse>('/user/register', data)
}

// 获取用户资料
export interface UserProfileResponse {
  userId: string
  name: string
  phone: string
  roles: string[]
  realName: string
  status: number
  email: string
  createdAt: string
}

export const getUserProfile = (userId: string) => {
  return api.get<UserProfileResponse>(`/user/profile/${userId}`)
}

// 更新用户信息
export interface UpdateUserRequest {
  name: string
  email: string
  phone: string
  realName: string
  status: number
}

export const updateUser = (userId: string, data: UpdateUserRequest) => {
  return api.put(`/user/${userId}`, data)
}

// 更新用户角色
export interface UpdateUserRolesRequest {
  roleIds: string[]
}

export const updateUserRoles = (userId: string, data: UpdateUserRolesRequest) => {
  return api.put(`/user/${userId}/roles`, data)
} 