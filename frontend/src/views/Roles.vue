<template>
  <div class="roles-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">角色管理</h1>
        <p class="page-subtitle">管理系统中的所有角色和权限分配</p>
      </div>
      <el-button type="primary" class="create-btn hover-button" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新建角色
      </el-button>
    </div>
    
    <!-- 搜索栏 -->
    <el-card class="search-card hover-card">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="搜索">
          <el-input
            v-model="searchForm.keyword"
            placeholder="请输入角色名称"
            clearable
            class="search-input"
            @keyup.enter="handleSearch"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
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
    
    <!-- 角色列表 -->
    <el-card class="table-card hover-card">
      <template #header>
        <div class="table-header">
          <div class="header-left">
            <span class="table-title">角色列表</span>
            <el-tag type="info" class="count-tag">{{ pagination.total }} 个角色</el-tag>
          </div>
          <div class="header-right">
            <el-button 
              v-if="selectedRoles.length > 0"
              type="danger" 
              size="small"
              class="batch-delete-btn"
            >
              <el-icon><Delete /></el-icon>
              批量删除 ({{ selectedRoles.length }})
            </el-button>
          </div>
        </div>
      </template>
      
      <el-table
        v-loading="loading"
        :data="roles"
        class="roles-table"
        @selection-change="handleSelectionChange"
        stripe
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="name" label="角色名称" min-width="150">
          <template #default="{ row }">
            <div class="role-cell">
              <el-avatar :size="32" class="role-avatar">
                <el-icon><Setting /></el-icon>
              </el-avatar>
              <div class="role-info">
                <div class="role-name">{{ row.name }}</div>
                <div class="role-description">{{ row.description }}</div>
              </div>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="permissionCount" label="权限数量" width="120">
          <template #default="{ row }">
            <el-tag type="info" class="permission-count-tag">
              <el-icon size="12"><Key /></el-icon>
              {{ row.permissionCodes?.length || 0 }}
            </el-tag>
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
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <div class="action-buttons">
              <el-button size="small" type="primary" class="action-btn" @click="handleEdit(row)">
                <el-icon size="14"><Edit /></el-icon>
                编辑
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
    
    <!-- 创建/编辑角色对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="800px"
      class="role-dialog"
      @close="handleDialogClose"
    >
      <el-form
        ref="roleFormRef"
        :model="roleForm"
        :rules="roleRules"
        label-width="100px"
        class="role-form"
      >
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="角色名称" prop="name">
              <el-input v-model="roleForm.name" placeholder="请输入角色名称" />
            </el-form-item>
          </el-col>
       
        </el-row>
        
        <el-form-item label="角色描述" prop="description">
          <el-input
            v-model="roleForm.description"
            type="textarea"
            :rows="3"
            placeholder="请输入角色描述"
          />
        </el-form-item>
        
        <el-form-item label="权限" prop="permissionCodes">
          <div class="permission-tree-wrapper">
            <!-- 全选/取消全选按钮 - 固定在顶部 -->
            <div class="permission-tree-header">
              <div class="header-right">
                <el-button size="small" class="select-btn" @click="handleSelectAll">
                  <el-icon size="12"><Select /></el-icon>
                  全选
                </el-button>
                <el-button size="small" class="unselect-btn" @click="handleUnselectAll">
                  <el-icon size="12"><Close /></el-icon>
                  取消全选
                </el-button>
              </div>
            </div>
            
            <!-- 权限树容器 - 可滚动 -->
            <div class="permission-tree-container">
              <el-tree
                ref="permissionTreeRef"
                :data="permissionTreeData"
                :props="treeProps"
                :default-checked-keys="roleForm.permissionCodes"
                show-checkbox
                node-key="code"
                class="permission-tree"
                @check="handlePermissionCheck"
              >
                <template #default="{ node, data }">
                  <span class="permission-node">
                    <span class="permission-name">{{ data.displayName }}</span>
                    <el-tag v-if="!data.isEnabled" size="small" type="danger" class="disabled-tag">已禁用</el-tag>
                  </span>
                </template>
              </el-tree>
            </div>
          </div>
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
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed, nextTick } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { 
  Plus, 
  Search, 
  Refresh, 
  Delete, 
  Edit, 
  Setting, 
  Timer, 
  Key, 
  Select, 
  Close, 
  Check 
} from '@element-plus/icons-vue'

import { getAllRoles, createRole, updateRole, deleteRole, type RoleInfo, type CreateRoleRequest } from '@/api/role'
import { getPermissionTree, type PermissionGroup, type PermissionItem } from '@/api/permission'

const loading = ref(false)
const submitLoading = ref(false)
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentRoleId = ref('')

const roles = ref<RoleInfo[]>([])
const selectedRoles = ref<RoleInfo[]>([])
const permissionTreeData = ref<PermissionItem[]>([])
const permissionTreeRef = ref()

const searchForm = reactive({
  keyword: ''
})

const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const roleForm = reactive<CreateRoleRequest>({
  name: '',
  description: '',
  permissionCodes: []
})

const roleFormRef = ref<FormInstance>()

const roleRules: FormRules = {
  name: [
    { required: true, message: '请输入角色名称', trigger: 'onBlur' }
  ],
  description: [
    { required: true, message: '请输入角色描述', trigger: 'onBlur' }
  ]
}

const treeProps = {
  children: 'children',
  label: 'displayName'
}

const dialogTitle = computed(() => isEdit.value ? '编辑角色' : '新建角色')

// 加载权限树数据
const loadPermissionTree = async () => {
  try {
    const response = await getPermissionTree()
    // 将权限组转换为树形数据，保持组结构
    permissionTreeData.value = response.data.map(group => ({
      code: group.name,
      displayName: group.name,
      isEnabled: true,
      children: group.permissions
    }))
  } catch (error) {
    ElMessage.error('加载权限数据失败')
  }
}



const loadRoles = async () => {
  loading.value = true
  try {
    const response = await getAllRoles({
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      name: searchForm.keyword || undefined,
      countTotal:true
    })
    roles.value = response.data.items
    pagination.total = response.data.total
  } catch (error: any) {
    // 错误已在全局拦截器中处理
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  pagination.pageIndex = 1
  loadRoles()
}

const handleReset = () => {
  searchForm.keyword = ''
  pagination.pageIndex = 1
  loadRoles()
}

const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  loadRoles()
}

const handleCurrentChange = (page: number) => {
  pagination.pageIndex = page
  loadRoles()
}

const handleSelectionChange = (selection: RoleInfo[]) => {
  selectedRoles.value = selection
}

const handlePermissionCheck = (data: PermissionItem, checkedInfo: any) => {
  // 获取所有选中的权限码
  const checkedKeys = checkedInfo.checkedKeys || []
  
  // 只使用完全选中的权限码，不包含半选中的父节点
  roleForm.permissionCodes = checkedKeys
}

const showCreateDialog = () => {
  isEdit.value = false
  currentRoleId.value = ''
  roleForm.name = ''
  roleForm.description = ''
  roleForm.permissionCodes = []
  dialogVisible.value = true
  
  // 等待DOM更新后清空树的选中状态
  nextTick(() => {
    if (permissionTreeRef.value) {
      permissionTreeRef.value.setCheckedKeys([])
    }
  })
}

const handleEdit = (role: RoleInfo) => {
  isEdit.value = true
  currentRoleId.value = role.roleId
  roleForm.name = role.name
  roleForm.description = role.description
  roleForm.permissionCodes = role.permissionCodes || []
  dialogVisible.value = true
  
  // 等待DOM更新后设置树的选中状态
  nextTick(() => {
    if (permissionTreeRef.value) {
      permissionTreeRef.value.setCheckedKeys(roleForm.permissionCodes)
    }
  })
}

const handleDelete = async (role: RoleInfo) => {
  try {
    await ElMessageBox.confirm(`确定要删除角色"${role.name}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    await deleteRole(role.roleId)
    ElMessage.success('删除成功')
    loadRoles()
  } catch {
    // 用户取消
  }
}

const handleSubmit = async () => {
  if (!roleFormRef.value) return
  
  try {
    await roleFormRef.value.validate()
    submitLoading.value = true
    
    if (isEdit.value) {
      await updateRole({
        roleId: currentRoleId.value,
        ...roleForm
      })
      ElMessage.success('更新成功')
    } else {
      await createRole(roleForm)
      ElMessage.success('创建成功')
    }
    
    dialogVisible.value = false
    loadRoles()
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '操作失败')
  } finally {
    submitLoading.value = false
  }
}

const handleDialogClose = () => {
  roleFormRef.value?.resetFields()
  // 清空树的选中状态
  if (permissionTreeRef.value) {
    permissionTreeRef.value.setCheckedKeys([])
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN')
}

// 递归获取所有权限码
const getAllPermissionCodes = (items: PermissionItem[]): string[] => {
  const codes: string[] = []
  items.forEach(item => {
    if (item.children && item.children.length > 0) {
      codes.push(...getAllPermissionCodes(item.children))
    } else {
      codes.push(item.code)
    }
  })
  return codes
}

const handleSelectAll = () => {
  const allCodes = getAllPermissionCodes(permissionTreeData.value)
  permissionTreeRef.value?.setCheckedKeys(allCodes)
  roleForm.permissionCodes = allCodes
}

const handleUnselectAll = () => {
  permissionTreeRef.value?.setCheckedKeys([])
  roleForm.permissionCodes = []
}

onMounted(() => {
  loadRoles()
  loadPermissionTree()
})
</script>

<style scoped>
.roles-page {
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

.roles-table {
  border-radius: 12px;
  overflow: hidden;
}

.role-cell {
  display: flex;
  align-items: center;
  gap: 12px;
}

.role-avatar {
  border: 2px solid #e2e8f0;
  flex-shrink: 0;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.role-info {
  flex: 1;
  min-width: 0;
}

.role-name {
  font-weight: 600;
  color: #1e293b;
  font-size: 14px;
  line-height: 1.2;
  margin-bottom: 2px;
}

.role-description {
  color: #64748b;
  font-size: 12px;
  line-height: 1.2;
}

.permission-count-tag {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
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

.role-dialog {
  border-radius: 16px;
}

.role-dialog :deep(.el-dialog__header) {
  padding: 24px 24px 0;
  border-bottom: 1px solid #e2e8f0;
}

.role-dialog :deep(.el-dialog__body) {
  padding: 24px;
}

.role-dialog :deep(.el-dialog__footer) {
  padding: 0 24px 24px;
  border-top: 1px solid #e2e8f0;
}

.role-form {
  padding: 0;
}

.permission-count-display {
  font-size: 12px;
  padding: 6px 12px;
  border-radius: 6px;
}

.permission-tree-wrapper {
  width: 100%;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background: #f8fafc;
  overflow: hidden;
}

.permission-tree-header {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  padding: 16px 16px 12px;
  background: #f8fafc;
  border-bottom: 1px solid #e2e8f0;
  position: sticky;
  top: 0;
  z-index: 10;
}

.permission-tree-container {
  max-height: 400px;
  width: 100%;
  overflow-y: auto;
  padding: 16px;
  background: #f8fafc;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.header-title {
  font-weight: 600;
  color: #1e293b;
  font-size: 14px;
}

.header-right {
  display: flex;
  gap: 8px;
}

.select-btn,
.unselect-btn {
  height: 28px;
  padding: 0 12px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 4px;
}

.select-btn {
  color: #059669;
  border-color: #10b981;
  background: #ecfdf5;
}

.select-btn:hover {
  color: #047857;
  border-color: #059669;
  background: #d1fae5;
}

.unselect-btn {
  color: #dc2626;
  border-color: #ef4444;
  background: #fef2f2;
}

.unselect-btn:hover {
  color: #b91c1c;
  border-color: #dc2626;
  background: #fee2e2;
}

.permission-tree {
  background: transparent;
}

.permission-tree :deep(.el-tree-node__content) {
  height: 36px;
  border-radius: 6px;
  margin-bottom: 2px;
  transition: all 0.2s ease;
}

.permission-tree :deep(.el-tree-node__content:hover) {
  background: #f1f5f9;
}

.permission-tree :deep(.el-tree-node.is-current > .el-tree-node__content) {
  background: #e0e7ff;
  color: #3730a3;
}

.permission-node {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
}

.permission-name {
  flex: 1;
  font-size: 13px;
  color: #374151;
}

.disabled-tag {
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 4px;
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
  
  .search-input {
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
  
  .permission-tree-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
    padding: 12px 12px 8px;
  }
  
  .header-right {
    width: 100%;
    justify-content: space-between;
  }
  
  .select-btn,
  .unselect-btn {
    flex: 1;
    justify-content: center;
  }
  
  .permission-tree-container {
    padding: 12px;
  }
}

/* 悬停效果 */
.hover-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  transform: translateY(-1px);
  transition: all 0.3s ease;
}

.hover-button:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
  transition: all 0.3s ease;
}
</style> 