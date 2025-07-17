# Quiz Exam 前端样式指南

## 概述

本文档描述了 Quiz Exam 前端项目的样式优化方案，采用现代化的设计风格，参考 ElementPlus 官方示例，提供优秀的用户体验。

## 设计系统

### 色彩方案

#### 主色调
- **主色**: `#667eea` (蓝色)
- **主色渐变**: `linear-gradient(135deg, #667eea 0%, #764ba2 100%)`
- **主色浅色**: `#8b9df0`
- **主色深色**: `#4f46e5`

#### 中性色
- **Gray 50**: `#f8fafc` (背景色)
- **Gray 100**: `#f1f5f9`
- **Gray 200**: `#e2e8f0` (边框色)
- **Gray 300**: `#cbd5e1`
- **Gray 400**: `#94a3b8`
- **Gray 500**: `#64748b` (次要文本)
- **Gray 600**: `#475569`
- **Gray 700**: `#334155`
- **Gray 800**: `#1e293b` (主要文本)
- **Gray 900**: `#0f172a`

#### 语义色
- **成功色**: `#10b981` (绿色)
- **警告色**: `#f59e0b` (橙色)
- **错误色**: `#ef4444` (红色)
- **信息色**: `#3b82f6` (蓝色)

### 字体系统

#### 字体族
- **主要字体**: `Inter` (Google Fonts)
- **后备字体**: `-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif`

#### 字体大小
- **XS**: `12px`
- **SM**: `14px`
- **Base**: `16px`
- **LG**: `18px`
- **XL**: `20px`
- **2XL**: `24px`
- **3XL**: `28px`
- **4XL**: `32px`

#### 字体粗细
- **Light**: `300`
- **Normal**: `400`
- **Medium**: `500`
- **Semibold**: `600`
- **Bold**: `700`

### 间距系统

- **1**: `4px`
- **2**: `8px`
- **3**: `12px`
- **4**: `16px`
- **5**: `20px`
- **6**: `24px`
- **8**: `32px`
- **10**: `40px`
- **12**: `48px`

### 圆角系统

- **SM**: `4px`
- **MD**: `8px`
- **LG**: `12px`
- **XL**: `16px`
- **2XL**: `20px`

### 阴影系统

- **SM**: `0 1px 2px 0 rgba(0, 0, 0, 0.05)`
- **MD**: `0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)`
- **LG**: `0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)`
- **XL**: `0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)`

## 组件样式

### 按钮 (Button)

#### 主要按钮
```css
.btn-primary {
  background: var(--primary-gradient);
  color: white;
  box-shadow: var(--shadow-md);
  border-radius: var(--radius-md);
  padding: var(--space-3) var(--space-4);
  font-weight: var(--font-medium);
  transition: all var(--transition-fast);
}

.btn-primary:hover {
  transform: translateY(-1px);
  box-shadow: var(--shadow-lg);
}
```

#### 次要按钮
```css
.btn-secondary {
  background: white;
  color: var(--gray-700);
  border: 1px solid var(--gray-300);
  border-radius: var(--radius-md);
  padding: var(--space-3) var(--space-4);
  font-weight: var(--font-medium);
  transition: all var(--transition-fast);
}

.btn-secondary:hover {
  background: var(--gray-50);
  border-color: var(--gray-400);
}
```

### 卡片 (Card)

```css
.card {
  background: white;
  border-radius: var(--radius-lg);
  border: 1px solid var(--gray-200);
  box-shadow: var(--shadow-sm);
  transition: all var(--transition-normal);
}

.card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}
```

### 表单 (Form)

```css
.form-input {
  width: 100%;
  padding: var(--space-3) var(--space-4);
  border: 1px solid var(--gray-300);
  border-radius: var(--radius-md);
  font-size: var(--text-base);
  transition: all var(--transition-fast);
}

.form-input:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}
```

### 标签 (Tag)

```css
.tag {
  display: inline-flex;
  align-items: center;
  gap: var(--space-1);
  padding: var(--space-1) var(--space-2);
  border-radius: var(--radius-sm);
  font-size: var(--text-xs);
  font-weight: var(--font-medium);
}

.tag-primary {
  background: rgba(102, 126, 234, 0.1);
  color: var(--primary-color);
}
```

## 页面布局

### 主布局 (MainLayout)

- **侧边栏**: 可折叠的深色渐变背景
- **顶部导航**: 白色背景，包含面包屑和用户信息
- **主内容区**: 浅灰色背景，居中布局

### 响应式设计

#### 断点
- **移动端**: `max-width: 768px`
- **平板**: `max-width: 1024px`
- **桌面**: `min-width: 1025px`

#### 移动端适配
- 侧边栏自动隐藏
- 表格操作按钮垂直排列
- 搜索表单垂直布局
- 卡片内容简化

## 动画效果

### 过渡动画
- **快速**: `0.15s ease`
- **正常**: `0.3s ease`
- **缓慢**: `0.5s ease`

### 悬停效果
- 卡片悬停时上移 2-4px
- 按钮悬停时上移 1-2px
- 阴影加深效果

### 页面切换动画
- 淡入淡出效果
- 滑入滑出效果

## 功能特性

### 用户管理
- **搜索功能**: 支持用户名和邮箱搜索
- **筛选功能**: 按状态筛选用户
- **分页功能**: 支持多种页面大小
- **批量操作**: 支持批量删除
- **角色分配**: 可视化角色管理

### 仪表盘
- **统计卡片**: 带趋势指标的统计信息
- **最近活动**: 用户登录记录
- **系统信息**: 实时系统状态
- **快速操作**: 常用功能快捷入口

### 登录页面
- **现代化设计**: 渐变背景和浮动动画
- **社交登录**: 支持微信和邮箱登录
- **记住我**: 用户偏好设置
- **响应式布局**: 移动端友好

## 使用指南

### CSS 变量
项目使用 CSS 变量来管理设计令牌，便于主题切换和维护：

```css
:root {
  --primary-color: #667eea;
  --gray-800: #1e293b;
  --radius-md: 8px;
  --transition-fast: 0.15s ease;
}
```

### 工具类
提供丰富的工具类，类似 Tailwind CSS：

```html
<div class="flex items-center justify-between p-6 bg-white rounded-lg shadow-md">
  <h2 class="text-2xl font-bold text-gray-800">标题</h2>
  <button class="btn btn-primary">按钮</button>
</div>
```

### 组件使用
所有组件都遵循 ElementPlus 的设计规范，并在此基础上进行了样式优化：

```vue
<template>
  <el-button type="primary" class="hover-button">
    <el-icon><Plus /></el-icon>
    新建用户
  </el-button>
</template>
```

## 浏览器支持

- **Chrome**: 90+
- **Firefox**: 88+
- **Safari**: 14+
- **Edge**: 90+

## 性能优化

- 使用 CSS 变量减少重复代码
- 优化动画性能，使用 `transform` 和 `opacity`
- 响应式图片和图标
- 代码分割和懒加载

## 维护指南

### 添加新组件
1. 使用设计系统的颜色、间距和字体
2. 添加适当的悬停和焦点状态
3. 确保响应式兼容性
4. 添加必要的动画效果

### 修改主题
1. 更新 CSS 变量值
2. 测试所有组件的视觉效果
3. 确保对比度符合可访问性标准

### 代码规范
- 使用 BEM 命名约定
- 优先使用 CSS 变量
- 添加适当的注释
- 保持代码简洁和可维护性 