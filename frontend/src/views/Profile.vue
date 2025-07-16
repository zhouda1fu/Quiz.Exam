<template>
  <div class="profile-page">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>个人资料</span>
        </div>
      </template>
      
      <el-row :gutter="40">
        <el-col :span="8">
          <div class="profile-avatar">
            <el-avatar :size="120" icon="UserFilled" />
            <h3>{{ userProfile?.name }}</h3>
            <p>{{ userProfile?.email }}</p>
          </div>
        </el-col>
        
        <el-col :span="16">
          <el-form
            ref="profileFormRef"
            :model="profileForm"
            :rules="profileRules"
            label-width="100px"
          >
            <el-form-item label="用户名" prop="name">
              <el-input v-model="profileForm.name" />
            </el-form-item>
            
            <el-form-item label="邮箱" prop="email">
              <el-input v-model="profileForm.email" />
            </el-form-item>
            
            <el-form-item label="真实姓名" prop="realName">
              <el-input v-model="profileForm.realName" />
            </el-form-item>
            
            <el-form-item label="手机号" prop="phone">
              <el-input v-model="profileForm.phone" />
            </el-form-item>
            
            <el-form-item label="状态">
              <el-tag :type="profileForm.status === 1 ? 'success' : 'danger'">
                {{ profileForm.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </el-form-item>
            
            <el-form-item label="角色">
              <el-tag
                v-for="role in userProfile?.roles"
                :key="role"
                type="info"
                style="margin-right: 5px"
              >
                {{ role }}
              </el-tag>
            </el-form-item>
            
            <el-form-item label="创建时间">
              <span>{{ formatDate(userProfile?.createdAt) }}</span>
            </el-form-item>
            
            <el-form-item>
              <el-button type="primary" :loading="loading" @click="handleUpdate">
                保存修改
              </el-button>
              <el-button @click="handleReset">重置</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { getUserProfile, updateUser, type UserProfileResponse } from '@/api/user'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const profileFormRef = ref<FormInstance>()
const loading = ref(false)
const userProfile = ref<UserProfileResponse | null>(null)

const profileForm = reactive({
  name: '',
  email: '',
  realName: '',
  phone: '',
  status: 1
})

const profileRules: FormRules = {
  name: [
    { required: true, message: '请输入用户名', trigger: 'onBlur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'onBlur' },
    { type: 'email', message: '请输入正确的邮箱格式', trigger: 'onBlur' }
  ],
  realName: [
    { required: true, message: '请输入真实姓名', trigger: 'onBlur' }
  ],
  phone: [
    { required: true, message: '请输入手机号', trigger: 'onBlur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号格式', trigger: 'onBlur' }
  ]
}

const loadUserProfile = async () => {
  if (!authStore.user?.userId) return
  
  try {
    const response = await getUserProfile(authStore.user.userId)
    userProfile.value = response.data
    
    // 填充表单数据
    Object.assign(profileForm, {
      name: userProfile.value.name,
      email: userProfile.value.email,
      realName: userProfile.value.realName,
      phone: userProfile.value.phone,
      status: userProfile.value.status
    })
  } catch (error) {
    ElMessage.error('加载用户资料失败')
  }
}

const handleUpdate = async () => {
  if (!profileFormRef.value || !authStore.user?.userId) return
  
  try {
    await profileFormRef.value.validate()
    loading.value = true
    
    await updateUser(authStore.user.userId, profileForm)
    ElMessage.success('更新成功')
    
    // 重新加载用户资料
    await loadUserProfile()
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '更新失败')
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  if (userProfile.value) {
    Object.assign(profileForm, {
      name: userProfile.value.name,
      email: userProfile.value.email,
      realName: userProfile.value.realName,
      phone: userProfile.value.phone,
      status: userProfile.value.status
    })
  }
}

const formatDate = (dateString?: string) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleString('zh-CN')
}

onMounted(() => {
  loadUserProfile()
})
</script>

<style scoped>
.profile-page {
  padding: 20px;
}

.card-header {
  font-weight: bold;
  color: #333;
}

.profile-avatar {
  text-align: center;
  padding: 20px 0;
}

.profile-avatar h3 {
  margin: 15px 0 5px 0;
  color: #333;
}

.profile-avatar p {
  margin: 0;
  color: #666;
  font-size: 14px;
}
</style> 