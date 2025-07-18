<template>
  <div v-if="hasPermission">
    <slot />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { usePermissionStore } from '@/stores/permission'

interface Props {
  permissions?: string[]
  any?: boolean // 是否只需要满足任一权限即可
}

const props = withDefaults(defineProps<Props>(), {
  permissions: () => [],
  any: false
})

const permissionStore = usePermissionStore()

const hasPermission = computed(() => {
  if (!props.permissions || props.permissions.length === 0) {
    return true
  }
  
  if (props.any) {
    // 满足任一权限即可
    return props.permissions.some(permission => 
      permissionStore.hasPermission([permission])
    )
  } else {
    // 需要满足所有权限
    return permissionStore.hasPermission(props.permissions)
  }
})
</script> 