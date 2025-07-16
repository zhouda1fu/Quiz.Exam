<template>
  <div class="dashboard">
    <el-row :gutter="20">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon user-icon">
              <el-icon size="40"><User /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.userCount }}</div>
              <div class="stat-label">用户总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon role-icon">
              <el-icon size="40"><Setting /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.roleCount }}</div>
              <div class="stat-label">角色总数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon active-icon">
              <el-icon size="40"><UserFilled /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.activeUsers }}</div>
              <div class="stat-label">活跃用户</div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon system-icon">
              <el-icon size="40"><Monitor /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-number">{{ stats.systemStatus }}</div>
              <div class="stat-label">系统状态</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>最近登录用户</span>
            </div>
          </template>
          <el-table :data="recentUsers" style="width: 100%">
            <el-table-column prop="name" label="用户名" />
            <el-table-column prop="email" label="邮箱" />
            <el-table-column prop="lastLoginTime" label="最后登录时间" />
          </el-table>
        </el-card>
      </el-col>
      
      <el-col :span="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>系统信息</span>
            </div>
          </template>
          <div class="system-info">
            <div class="info-item">
              <span class="label">系统版本：</span>
              <span class="value">Quiz Exam v1.0.0</span>
            </div>
            <div class="info-item">
              <span class="label">运行时间：</span>
              <span class="value">{{ uptime }}</span>
            </div>
            <div class="info-item">
              <span class="label">服务器状态：</span>
              <span class="value status-normal">正常</span>
            </div>
            <div class="info-item">
              <span class="label">数据库状态：</span>
              <span class="value status-normal">正常</span>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const stats = ref({
  userCount: 0,
  roleCount: 0,
  activeUsers: 0,
  systemStatus: '正常'
})

const recentUsers = ref([
  {
    name: 'admin',
    email: 'admin@example.com',
    lastLoginTime: '2024-01-15 10:30:00'
  },
  {
    name: 'user1',
    email: 'user1@example.com',
    lastLoginTime: '2024-01-15 09:15:00'
  }
])

const uptime = ref('0天 0小时 0分钟')

onMounted(() => {
  // 模拟数据加载
  stats.value = {
    userCount: 1250,
    roleCount: 8,
    activeUsers: 89,
    systemStatus: '正常'
  }
  
  // 模拟运行时间
  const startTime = new Date('2024-01-01')
  const now = new Date()
  const diff = now.getTime() - startTime.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
  const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
  
  uptime.value = `${days}天 ${hours}小时 ${minutes}分钟`
})
</script>

<style scoped>
.dashboard {
  padding: 20px;
}

.stat-card {
  margin-bottom: 20px;
}

.stat-content {
  display: flex;
  align-items: center;
}

.stat-icon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 20px;
}

.user-icon {
  background-color: #e3f2fd;
  color: #1976d2;
}

.role-icon {
  background-color: #f3e5f5;
  color: #7b1fa2;
}

.active-icon {
  background-color: #e8f5e8;
  color: #388e3c;
}

.system-icon {
  background-color: #fff3e0;
  color: #f57c00;
}

.stat-info {
  flex: 1;
}

.stat-number {
  font-size: 32px;
  font-weight: bold;
  color: #333;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 14px;
  color: #666;
}

.card-header {
  font-weight: bold;
  color: #333;
}

.system-info {
  padding: 10px 0;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  border-bottom: 1px solid #f0f0f0;
}

.info-item:last-child {
  border-bottom: none;
}

.label {
  color: #666;
  font-weight: 500;
}

.value {
  color: #333;
}

.status-normal {
  color: #52c41a;
}
</style> 