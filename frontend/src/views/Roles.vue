<template>
  <div class="roles-page">
    <div class="page-header">
      <h2>角色管理</h2>
      <el-button type="primary" @click="showCreateDialog">
        <el-icon><Plus /></el-icon>
        新建角色
      </el-button>
    </div>
    
    <!-- 搜索栏 -->
    <el-card class="search-card">
      <el-form :inline="true" :model="searchForm">
        <el-form-item label="角色名称">
          <el-input
            v-model="searchForm.keyword"
            placeholder="请输入角色名称"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 角色列表 -->
    <el-card>
      <el-table
        v-loading="loading"
        :data="roles"
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="name" label="角色名称" />
        <el-table-column prop="description" label="角色描述" />
        <el-table-column prop="permissionCount" label="权限数量">
          <template #default="{ row }">
            <el-tag type="info">{{ row.permissionCodes?.length || 0 }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button size="small" @click="handleEdit(row)">编辑</el-button>
            <el-button size="small" type="danger" @click="handleDelete(row)">删除</el-button>
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
      @close="handleDialogClose"
    >
      <el-form
        ref="roleFormRef"
        :model="roleForm"
        :rules="roleRules"
        label-width="100px"
      >
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="roleForm.name" placeholder="请输入角色名称" />
        </el-form-item>
        
        <el-form-item label="角色描述" prop="description">
          <el-input
            v-model="roleForm.description"
            type="textarea"
            :rows="3"
            placeholder="请输入角色描述"
          />
        </el-form-item>
        
        <el-form-item label="权限" prop="permissionCodes">
          <div class="permission-tree-container">
            <!-- 全选/取消全选按钮 -->
            <div class="permission-tree-header">
              <el-button size="small" @click="handleSelectAll">全选</el-button>
              <el-button size="small" @click="handleUnselectAll">取消全选</el-button>
            </div>
            
            <el-tree
              ref="permissionTreeRef"
              :data="permissionTreeData"
              :props="treeProps"
              :default-checked-keys="roleForm.permissionCodes"
              show-checkbox
              node-key="code"
              @check="handlePermissionCheck"
            >
              <template #default="{ node, data }">
                <span class="permission-node">
                  <span class="permission-name">{{ data.displayName }}</span>
                  <el-tag v-if="!data.isEnabled" size="small" type="danger">已禁用</el-tag>
                </span>
              </template>
            </el-tree>
          </div>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" :loading="submitLoading" @click="handleSubmit">
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed, nextTick } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'

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
      name: searchForm.keyword
    })
    roles.value = response.data.items
    pagination.total = response.data.total
  } catch (error) {
    ElMessage.error('加载角色列表失败')
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
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  color: #333;
}

.search-card {
  margin-bottom: 20px;
}

.pagination-wrapper {
  margin-top: 20px;
  text-align: right;
}

.dialog-footer {
  text-align: right;
}

.permission-tree-container {
  max-height: 400px;
  overflow-y: auto;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  padding: 10px;
}

.permission-tree-header {
  display: flex;
  gap: 10px;
  margin-bottom: 10px;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
}

.permission-node {
  display: flex;
  align-items: center;
  gap: 8px;
}

.permission-name {
  flex: 1;
}
</style> 