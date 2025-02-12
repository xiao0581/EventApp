<template>
  <q-page class="flex flex-center">
    <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/emaillogin" />
    <q-img src="src/assets/pic/logo1.png" class="logo"></q-img>
    <q-card class="q-pa-md">
      <q-card-section>
        <div class="text-h6">Login</div>
      </q-card-section>

      <q-card-section>
        <q-form @submit.prevent="handleLogin" class="form-container">
          <q-input
            v-model="email"
            label="Username"
            outlined
            required
            rounded
            style="width: 100%; max-width: 350px"
            label-color="primary"
            :rules="[(val) => !!val || 'Username is required']"
          />

          <q-input
            v-model="password"
            label="Password"
            type="password"
            outlined
            required
            rounded
            style="width: 100%; max-width: 350px"
            label-color="primary"
            :rules="[(val) => !!val || 'Password is required']"
          />

          <q-btn
            type="submit"
            label="Login"
            color="primary"
            rounded
            :loading="loading"
            :disable="loading"
          />
          <div class="signup-container">
            <span>Don't have an account?</span>
            <router-link to="/registers" class="signup-link">Sign Up</router-link>
          </div>
        </q-form>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAuthStore } from 'src/stores/auth'
import { useRouter } from 'vue-router'
import { Notify } from 'quasar'

const email = ref<string>('')
const password = ref<string>('')
const loading = ref<boolean>(false)

const authStore = useAuthStore()

const router = useRouter()

const handleLogin = async (): Promise<void> => {
  loading.value = true
  try {
    await authStore.login({ email: email.value, password: password.value })

    Notify.create({
      type: 'positive',
      message: 'Login successful!',
      timeout: 1000,
    })

    await router.push('/home')
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
  background-image: url('src/assets/pic/loginHores.png');
  background-size: 90%;
  background-position: bottom;
  background-repeat: no-repeat;
  height: 100vh;
  margin: 0;
  display: flex;
  justify-content: center;
  align-items: center;
}

.q-card {
  margin-bottom: 210px;
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
.logo {
  width: 150px;
  margin-top: 50px;
}
.back-btn {
  position: absolute;
  top: 16px;
  left: -90px;
  z-index: 10;
  background: transparent;
  box-shadow: none;
  border: none;
  padding: 0;
}
</style>
