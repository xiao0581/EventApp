<template>
  <q-page class="flex flex-center">
    <q-card class="q-pa-md">
      <q-card-section>
        <div class="text-h6">Register</div>
      </q-card-section>

      <q-card-section>
        <q-form @submit.prevent="handleRegister" class="form-container">
          <q-input
            v-model="email"
            label="Email"
            type="email"
            outlined
            rounded
            required
            label-color="primary"
            style="width: 100%; max-width: 350px"
            :rules="[(val) => !!val || 'Email is required']"
          />
          <q-input
            v-model="username"
            label="Username"
            outlined
            required
            rounded
            label-color="primary"
            style="width: 100%; max-width: 350px"
            :rules="[(val) => !!val || 'Username is required']"
          />

          <q-input
            v-model="password"
            label="Password"
            type="password"
            outlined
            rounded
            required
            label-color="primary"
            style="width: 100%; max-width: 350px"
            :rules="[(val) => !!val || 'Password is required']"
          />

          <q-input
            v-model="confirmPassword"
            label="Confirm Password"
            type="password"
            outlined
            rounded
            required
            label-color="primary"
            style="width: 100%; max-width: 350px"
            :rules="[(val) => !!val || 'Password is required']"
          />

          <q-btn
            type="submit"
            label="Register"
            color="primary"
            rounded
            style="width: 100%; max-width: 350px"
            :loading="loading"
            :disable="loading"
          />
          <div class="signup-container">
            <span>Joind us before?</span>
            <router-link to="/login" class="signup-link">Log in</router-link>
          </div>
        </q-form>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAuthStore } from 'src/stores/auth'
import { Notify } from 'quasar'

const username = ref<string>('')
const password = ref<string>('')
const email = ref<string>('')
const confirmPassword = ref<string>('')
const loading = ref<boolean>(false)

const authStore = useAuthStore()

const handleRegister = async (): Promise<void> => {
  loading.value = true
  try {
    await authStore.register({
      username: username.value,
      password: password.value,
      email: email.value,
      confirmPassword: confirmPassword.value,
    })

    Notify.create({
      type: 'positive',
      message: 'Registration successful!',
    })
  } catch (error: unknown) {
    const message = (error as Error).message || 'An unknown error occurred.'
    Notify.create({
      type: 'negative',
      message,
    })
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.q-page {
  min-height: 100vh;
  background-image: url('src/assets/pic/login.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  height: 100vh;
  margin: 0;
  display: flex;
  justify-content: center;
  align-items: center;
}
.q-card {
  margin-top: 200px;
  border-radius: 20px;
  width: 350px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  padding: 20px;
  transition: all 0.3s ease;
}
.text-h6 {
  color: #6a7bff;
  font-size: 1.5rem;
  font-weight: bold;
  text-align: left;
}
.form-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
  align-items: center;
}

.q-btn {
  width: 100%;
  max-width: 250px;
}
</style>
