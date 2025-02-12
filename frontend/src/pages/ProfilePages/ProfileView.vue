<template>
  <q-page class="profile-page">
    <q-card class="profile-card">
      <q-card-section class="avatar-section">
        <q-avatar size="100px">
          <img src="src/assets/pic/avatar1.jpg" alt="User Avatar" />
        </q-avatar>
      </q-card-section>

      <q-card-section class="user-info">
        <div class="user-email">{{ authStore.user?.email }}</div>
      </q-card-section>

      <q-card-section class="action-buttons">
        <q-btn
          label="Logout"
          color="negative"
          icon="logout"
          @click="handleLogout"
          class="full-width"
        />
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from 'src/stores/auth'
const router = useRouter()
const authStore = useAuthStore()

const handleLogout = async () => {
  authStore.logout()
  await router.push('/MainLoginView')
}
</script>

<style scoped>
.profile-page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(to right, #6a7bff, #9c6eff);
}

.profile-card {
  width: 350px;
  text-align: center;
  border-radius: 20px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  background: white;
}

.avatar-section {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}

.user-info {
  font-size: 18px;
  font-weight: bold;
  color: #333;
  margin-bottom: 10px;
}

.user-email {
  font-size: 14px;
  color: #666;
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 20px;
}

.full-width {
  width: 100%;
}
</style>
