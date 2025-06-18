<template>
  <!-- Main layout structure: Header, Page, Footer -->
  <q-layout view="lHh Lpr lFf">
    <!-- Page container renders the current route component -->
    <q-page-container>
      <router-view />
    </q-page-container>

    <!-- Bottom navigation footer (conditionally visible) -->
    <q-footer v-if="!hideFooter" class="custom-footer">
      <q-toolbar class="custom-toolbar">
        <q-btn
          flat
          icon="sym_o_home"
          to="/home"
          class="toolbar-btn"
          :class="{ 'active-btn': isActive('/home') }"
        />

        <!-- Navigation button: Calendar -->
        <q-btn
          flat
          icon="sym_o_calendar_month"
          to="/calendar"
          class="toolbar-btn"
          :class="{ 'active-btn': isActive('/calendar') }"
        />

        <!-- Navigation button: Message -->
        <q-btn
          flat
          icon="sym_o_forum"
          to="/message"
          class="toolbar-btn"
          :class="{ 'active-btn': isActive('/message') }"
        />

        <!-- Navigation button: Notifications -->
        <q-btn
          flat
          icon="sym_o_notifications"
          to="/notification"
          class="toolbar-btn"
          :class="{ 'active-btn': isActive('/notification') }"
        />

        <!-- Navigation button: Profile/Login -->
        <q-btn
          flat
          icon="sym_o_person"
          to="/profile"
          class="toolbar-btn"
          :class="{ 'active-btn': isActive('/MainLoginView') }"
        />
      </q-toolbar>
    </q-footer>
  </q-layout>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { computed } from 'vue'
import { onMounted } from 'vue'
import { useAuthStore } from 'src/stores/auth'
const authStore = useAuthStore()
const route = useRoute()

// Load user data when layout is mounted
onMounted(() => {
  authStore.loadUser()
})

// Whether to hide the footer, controlled via route meta
const hideFooter = computed(() => route.meta.hideFooter)

// Check if a route path is currently active
const isActive = (path: string): boolean => {
  return route.path === path
}
</script>

<style scoped>
.custom-footer {
  background-color: rgba(255, 255, 255, 0.6);
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  border-radius: 150px;
  height: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
  bottom: 10px;
  margin-left: 5px;
  margin-right: 5px;
}

.custom-toolbar {
  height: 100%;
  display: flex;
  justify-content: space-around;
  align-items: center;
}

.toolbar-btn {
  color: #4a4e69;
  font-size: 20px;
  transition: all 0.3s ease;
  border-radius: 40%;
  width: 60px;
  height: 40px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.active-btn {
  background-color: #6c3baa;
  color: white;
  width: 60px;
  height: 30px;
  line-height: 40px;
  border-radius: 40%;
  transition: all 0.3s ease;
}
</style>
