<template>
  <div class="users-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">用户管理</h1>
        <p class="page-subtitle">管理系统中的所有用户账户和权限</p>
      </div>
      <el-button type="primary" class="create-btn hover-button" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新建用户
      </el-button>
    </div>
    
    <!-- 搜索栏 -->
    <el-card class="search-card hover-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="搜索">
          <el-input
            v-model="searchForm.keyword"
            placeholder="请输入用户名或邮箱"
            clearable
            class="search-input"
            @keyup.enter="handleSearch"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="searchForm.status" placeholder="请选择状态" clearable class="status-select">
            <el-option label="启用" :value="1" />
            <el-option label="禁用" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" class="search-btn hover-button" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索 
          </el-button>
          <el-button class="reset-btn" @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 用户列表 -->
    <el-card class="table-card hover-card">
      <template #header>
        <div class="table-header">
          <div class="header-left">
            <span class="table-title">用户列表</span>
            <el-tag type="info" class="count-tag">{{ pagination.total }} 个用户</el-tag>
          </div>
          <div class="header-right">
            <el-button 
              v-if="selectedUsers.length > 0"
              type="danger" 
              size="small"
              class="batch-delete-btn"
            >
              <el-icon><Delete /></el-icon>
              批量删除 ({{ selectedUsers.length }})
            </el-button>
          </div>
        </div>
      </template>
      
      <el-table
        v-loading="loading"
        :data="users"
        class="users-table"
        @selection-change="handleSelectionChange"
        stripe
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="name" label="用户名" min-width="120">
          <template #default="{ row }">
            <div class="user-cell">
              <el-avatar :size="32" class="user-avatar">
                <el-icon><UserFilled /></el-icon>
              </el-avatar>
              <div class="user-info">
                <div class="user-name">{{ row.name }}</div>
                <div class="user-email">{{ row.email }}</div>
              </div>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="realName" label="真实姓名" min-width="100" />
        <el-table-column prop="phone" label="手机号" min-width="120" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'danger'" class="status-tag">
              <el-icon size="12">
                <component :is="row.status === 1 ? 'CircleCheck' : 'CircleClose'" />
              </el-icon>
              {{ row.status === 1 ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="roles" label="角色" min-width="150">
          <template #default="{ row }">
            <div class="roles-container">
              <el-tag
                v-for="role in row.roles"
                :key="role"
                type="info"
                class="role-tag"
                size="small"
              >
                {{ role }}
              </el-tag>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" min-width="160">
          <template #default="{ row }">
            <div class="time-cell">
              <el-icon size="14" color="#94a3b8"><Timer /></el-icon>
              <span>{{ formatDate(row.createdAt) }}</span>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <div class="action-buttons">
              <el-button size="small" type="primary" class="action-btn" @click="handleEdit(row)">
                <el-icon size="14"><Edit /></el-icon>
                编辑
              </el-button>
              <el-button size="small" type="warning" class="action-btn" @click="handleRoles(row)">
                <el-icon size="14"><Setting /></el-icon>
                角色
              </el-button>
              <el-button size="small" type="danger" class="action-btn" @click="handleDelete(row)">
                <el-icon size="14"><Delete /></el-icon>
                删除
              </el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分页 -->
      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="pagination.pageIndex"
          v-model:page-size="pagination.pageSize"
          :total="pagination.total"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          class="pagination"
          @update:page-size="handleSizeChange"
          @update:current-page="handleCurrentChange"
        />
      </div>
    </el-card>
    
    <!-- 创建/编辑用户对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      class="user-dialog"
      @close="handleDialogClose"
    >
      <el-form
        ref="userFormRef"
        :model="userForm"
        :rules="userRules"
        label-width="100px"
        class="user-form"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="用户名" prop="name">
              <el-input v-model="userForm.name" placeholder="请输入用户名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="邮箱" prop="email">
              <el-input v-model="userForm.email" placeholder="请输入邮箱" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="真实姓名" prop="realName">
              <el-input v-model="userForm.realName" placeholder="请输入真实姓名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="手机号" prop="phone">
              <el-input v-model="userForm.phone" placeholder="请输入手机号" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="密码" prop="password" v-if="!isEdit">
          <el-input
            v-model="userForm.password"
            type="password"
            placeholder="请输入密码"
            show-password
          />
        </el-form-item>
        
        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="userForm.status" class="status-radio-group">
            <el-radio :label="1" class="status-radio">
              <el-icon size="16" color="#10b981"><CircleCheck /></el-icon>
              启用
            </el-radio>
            <el-radio :label="0" class="status-radio">
              <el-icon size="16" color="#ef4444"><CircleClose /></el-icon>
              禁用
            </el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogVisible = false" class="cancel-btn">取消</el-button>
          <el-button type="primary" :loading="submitLoading" @click="handleSubmit" class="submit-btn">
            <el-icon v-if="!submitLoading"><Check /></el-icon>
            {{ isEdit ? '更新' : '创建' }}
          </el-button>
        </div>
      </template>
    </el-dialog>
    
    <!-- 分配角色对话框 -->
    <el-dialog
      v-model="roleDialogVisible"
      title="分配角色"
      width="500px"
      class="role-dialog"
    >
      <div class="role-dialog-content">
        <div class="user-info-card">
          <el-avatar :size="48" class="user-avatar-large">
            <el-icon><UserFilled /></el-icon>
          </el-avatar>
          <div class="user-details">
            <div class="user-name-large">{{ currentUser?.name }}</div>
            <div class="user-email-large">{{ currentUser?.email }}</div>
          </div>
        </div>
        
        <el-form label-width="80px" class="role-form">
          <el-form-item label="角色">
            <el-checkbox-group v-model="selectedRoleIds" class="role-checkbox-group">
              <el-checkbox
                v-for="role in allRoles"
                :key="role.roleId"
                :value="role.roleId"
                class="role-checkbox"
              >
                <div class="role-checkbox-content">
                  <el-icon size="16" color="#667eea"><Setting /></el-icon>
                  <span>{{ role.name }}</span>
                </div>
              </el-checkbox>
            </el-checkbox-group>
          </el-form-item>
        </el-form>
      </div>
      
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="roleDialogVisible = false" class="cancel-btn">取消</el-button>
          <el-button type="primary" :loading="roleSubmitLoading" @click="handleRoleSubmit" class="submit-btn">
            <el-icon v-if="!roleSubmitLoading"><Check /></el-icon>
            确定
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { register, updateUser, updateUserRoles, getUsers, deleteUser, type RegisterRequest, type UserInfo } from '@/api/user'
import { getAllRoles, type RoleInfo } from '@/api/role'

const loading = ref(false)
const submitLoading = ref(false)
const roleSubmitLoading = ref(false)
const dialogVisible = ref(false)
const roleDialogVisible = ref(false)
const isEdit = ref(false)
const currentUserId = ref<string>('')
const currentUser = ref<any>(null)

const users = ref<UserInfo[]>([])
const selectedUsers = ref<UserInfo[]>([])
const allRoles = ref<RoleInfo[]>([])
const selectedRoleIds = ref<string[]>([])

const searchForm = reactive({
  keyword: '',
  status: null as number | null
})

const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const userForm = reactive<RegisterRequest>({
  name: '',
  email: '',
  password: '',
  phone: '',
  realName: '',
  status: 1,
  roleIds: []
})

const userFormRef = ref<FormInstance>()

const userRules: FormRules = {
  name: [
    { required: true, message: '请输入用户名', trigger: 'onBlur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'onBlur' },
    { type: 'email', message: '请输入正确的邮箱格式', trigger: 'onBlur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'onBlur' },
    { min: 6, message: '密码长度不能少于6位', trigger: 'onBlur' }
  ],
  realName: [
    { required: true, message: '请输入真实姓名', trigger: 'onBlur' }
  ],
  phone: [
    { required: true, message: '请输入手机号', trigger: 'onBlur' }
  ]
}

const dialogTitle = computed(() => isEdit.value ? '编辑用户' : '新建用户')

const loadUsers = async () => {
  loading.value = true
  try {
    const response = await getUsers({
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      keyword: searchForm.keyword || undefined,
      status: searchForm.status || undefined
    })
    users.value = response.data.items
    pagination.total = response.data.total
  } catch (error: any) {
    // 错误已在全局拦截器中处理
  } finally {
    loading.value = false
  }
}

const loadRoles = async () => {
  try {
    const response = await getAllRoles({
      pageIndex: 1,
      pageSize: 100
    })
    allRoles.value = response.data.items
  } catch (error) {
    // 错误已在全局拦截器中处理
  }
}

const handleSearch = () => {
  pagination.pageIndex = 1
  loadUsers()
}

const handleReset = () => {
  searchForm.keyword = ''
  searchForm.status = null
  pagination.pageIndex = 1
  loadUsers()
}

const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  loadUsers()
}

const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page
  loadUsers()
}

const handleSelectionChange = (selection: UserInfo[]) => {
  selectedUsers.value = selection
}

const showCreateDialog = () => {
  isEdit.value = false
  currentUserId.value = ''
  Object.assign(userForm, {
    name: '',
    email: '',
    password: '',
    phone: '',
    realName: '',
    status: 1,
    roleIds: []
  })
  dialogVisible.value = true
}

const handleEdit = (user: UserInfo) => {
  isEdit.value = true
  currentUserId.value = user.userId
  Object.assign(userForm, {
    name: user.name,
    email: user.email,
    password: '',
    phone: user.phone,
    realName: user.realName,
    status: user.status,
    roleIds: []
  })
  dialogVisible.value = true
}

const handleRoles = (user: UserInfo) => {
  currentUser.value = user
  selectedRoleIds.value = user.roles.map((role: string) => {
    const foundRole = allRoles.value.find(r => r.name === role)
    return foundRole?.roleId || ''
  }).filter(Boolean)
  roleDialogVisible.value = true
}

const handleDelete = async (user: UserInfo) => {
  try {
    await ElMessageBox.confirm(`确定要删除用户"${user.name}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await deleteUser(user.userId)
    ElMessage.success('删除成功')
    loadUsers()
  } catch (error: any) {
    if (error !== 'cancel') {
      // 错误已在全局拦截器中处理
    }
  }
}

const handleSubmit = async () => {
  if (!userFormRef.value) return
  
  try {
    await userFormRef.value.validate()
    submitLoading.value = true
    
    if (isEdit.value) {
      await updateUser(currentUserId.value, userForm)
      ElMessage.success('更新成功')
    } else {
      await register(userForm)
      ElMessage.success('创建成功')
    }
    
    dialogVisible.value = false
    loadUsers()
  } catch (error: any) {
    // 错误已在全局拦截器中处理
  } finally {
    submitLoading.value = false
  }
}

const handleRoleSubmit = async () => {
  if (!currentUser.value) return
  
  try {
    roleSubmitLoading.value = true
    await updateUserRoles({
      roleIds: selectedRoleIds.value,
      userId: currentUser.value.userId
    })
    ElMessage.success('角色分配成功')
    roleDialogVisible.value = false
    loadUsers()
  } catch (error: any) {
    // 错误已在全局拦截器中处理
  } finally {
    roleSubmitLoading.value = false
  }
}

const handleDialogClose = () => {
  userFormRef.value?.resetFields()
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN')
}

onMounted(() => {
  loadUsers()
  loadRoles()
})
</script>

<style scoped>
.users-page {
  padding: 0;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 32px;
  padding: 0 0 24px 0;
  border-bottom: 1px solid #e2e8f0;
}

.header-content {
  flex: 1;
}

.page-title {
  font-size: 28px;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 8px 0;
  line-height: 1.2;
}

.page-subtitle {
  font-size: 16px;
  color: #64748b;
  margin: 0;
  font-weight: 400;
}

.create-btn {
  height: 44px;
  padding: 0 24px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 14px;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.search-card {
  margin-bottom: 24px;
  border-radius: 16px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.search-form {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.search-input {
  width: 300px;
}

.status-select {
  width: 200px;
}

.search-btn {
  height: 40px;
  padding: 0 20px;
  border-radius: 8px;
  font-weight: 500;
}

.reset-btn {
  height: 40px;
  padding: 0 20px;
  border-radius: 8px;
  color: #64748b;
  border-color: #e2e8f0;
}

.reset-btn:hover {
  color: #334155;
  border-color: #cbd5e1;
  background: #f8fafc;
}

.table-card {
  border-radius: 16px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.table-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.table-title {
  font-size: 18px;
  font-weight: 600;
  color: #1e293b;
}

.count-tag {
  font-size: 12px;
  padding: 4px 8px;
  border-radius: 6px;
}

.batch-delete-btn {
  height: 32px;
  padding: 0 16px;
  border-radius: 8px;
  font-size: 12px;
}

.users-table {
  border-radius: 12px;
  overflow: hidden;
}

.user-cell {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-avatar {
  border: 2px solid #e2e8f0;
  flex-shrink: 0;
}

.user-info {
  flex: 1;
  min-width: 0;
}

.user-name {
  font-weight: 600;
  color: #1e293b;
  font-size: 14px;
  line-height: 1.2;
  margin-bottom: 2px;
}

.user-email {
  color: #64748b;
  font-size: 12px;
  line-height: 1.2;
}

.status-tag {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
}

.roles-container {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}

.role-tag {
  border-radius: 6px;
  font-size: 11px;
  padding: 2px 6px;
}

.time-cell {
  display: flex;
  align-items: center;
  gap: 6px;
  color: #64748b;
  font-size: 12px;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.action-btn {
  height: 28px;
  padding: 0 12px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 4px;
}

.pagination-wrapper {
  margin-top: 24px;
  display: flex;
  justify-content: center;
}

.pagination {
  --el-pagination-bg-color: transparent;
  --el-pagination-border-radius: 8px;
}

.user-dialog,
.role-dialog {
  border-radius: 16px;
}

.user-dialog :deep(.el-dialog__header) {
  padding: 24px 24px 0;
  border-bottom: 1px solid #e2e8f0;
}

.user-dialog :deep(.el-dialog__body) {
  padding: 24px;
}

.user-dialog :deep(.el-dialog__footer) {
  padding: 0 24px 24px;
  border-top: 1px solid #e2e8f0;
}

.user-form {
  padding: 0;
}

.status-radio-group {
  display: flex;
  gap: 24px;
}

.status-radio {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  transition: all 0.2s ease;
}

.status-radio:hover {
  border-color: #667eea;
  background: #f8fafc;
}

.status-radio.is-checked {
  border-color: #667eea;
  background: #f0f4ff;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.cancel-btn {
  height: 40px;
  padding: 0 20px;
  border-radius: 8px;
  color: #64748b;
  border-color: #e2e8f0;
}

.submit-btn {
  height: 40px;
  padding: 0 20px;
  border-radius: 8px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 6px;
}

.role-dialog-content {
  padding: 0;
}

.user-info-card {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 20px;
  background: #f8fafc;
  border-radius: 12px;
  margin-bottom: 24px;
}

.user-avatar-large {
  border: 3px solid #e2e8f0;
  flex-shrink: 0;
}

.user-details {
  flex: 1;
  min-width: 0;
}

.user-name-large {
  font-weight: 600;
  color: #1e293b;
  font-size: 16px;
  line-height: 1.2;
  margin-bottom: 4px;
}

.user-email-large {
  color: #64748b;
  font-size: 14px;
  line-height: 1.2;
}

.role-form {
  padding: 0;
}

.role-checkbox-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.role-checkbox {
  padding: 12px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  transition: all 0.2s ease;
  margin: 0;
}

.role-checkbox:hover {
  border-color: #667eea;
  background: #f8fafc;
}

.role-checkbox.is-checked {
  border-color: #667eea;
  background: #f0f4ff;
}

.role-checkbox-content {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }
  
  .page-title {
    font-size: 24px;
  }
  
  .search-form {
    flex-direction: column;
    align-items: stretch;
    gap: 12px;
  }
  
  .search-input,
  .status-select {
    width: 100%;
  }
  
  .table-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
  
  .action-buttons {
    flex-direction: column;
    gap: 4px;
  }
  
  .action-btn {
    width: 100%;
    justify-content: center;
  }
  
  .status-radio-group {
    flex-direction: column;
    gap: 12px;
  }
  
  .role-checkbox-group {
    gap: 8px;
  }
}

.dialog-footer {
  text-align: right;
}
</style> 