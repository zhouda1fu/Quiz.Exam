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
          v-model:current="pagination.pageIndex"
          v-model:page-size="pagination.pageSize"
          :total="pagination.total"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>
    
    <!-- 创建/编辑角色对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
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
          <el-checkbox-group v-model="roleForm.permissionCodes">
            <el-checkbox
              v-for="permission in permissions"
              :key="permission.code"
              :value="permission.code"
            >
              {{ permission.name }}
            </el-checkbox>
          </el-checkbox-group>
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
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { getAllRoles, createRole, updateRole, type RoleInfo, type CreateRoleRequest } from '@/api/role'

const loading = ref(false)
const submitLoading = ref(false)
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentRoleId = ref('')

const roles = ref<RoleInfo[]>([])
const selectedRoles = ref<RoleInfo[]>([])

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

// 模拟权限数据
const permissions = ref([
  { code: 'user.read', name: '用户查看' },
  { code: 'user.create', name: '用户创建' },
  { code: 'user.update', name: '用户更新' },
  { code: 'user.delete', name: '用户删除' },
  { code: 'role.read', name: '角色查看' },
  { code: 'role.create', name: '角色创建' },
  { code: 'role.update', name: '角色更新' },
  { code: 'role.delete', name: '角色删除' }
])

const dialogTitle = computed(() => isEdit.value ? '编辑角色' : '新建角色')

const loadRoles = async () => {
  loading.value = true
  try {
    const response = await getAllRoles({
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      keyword: searchForm.keyword
    })
    roles.value = response.data.items
    pagination.total = response.data.totalCount
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

const showCreateDialog = () => {
  isEdit.value = false
  currentRoleId.value = ''
  roleForm.name = ''
  roleForm.description = ''
  roleForm.permissionCodes = []
  dialogVisible.value = true
}

const handleEdit = (role: RoleInfo) => {
  isEdit.value = true
  currentRoleId.value = role.roleId
  roleForm.name = role.name
  roleForm.description = role.description
  roleForm.permissionCodes = role.permissionCodes || []
  dialogVisible.value = true
}

const handleDelete = async (role: RoleInfo) => {
  try {
    await ElMessageBox.confirm(`确定要删除角色"${role.name}"吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    // TODO: 调用删除API
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
      await updateRole(currentRoleId.value, roleForm)
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
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN')
}

onMounted(() => {
  loadRoles()
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
</style> 