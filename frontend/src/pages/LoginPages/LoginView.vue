<template>
  <!-- Full-page wrapper with background image -->
  <q-page class="flex flex-center">
    <!-- Back button to return to the main login view -->
    <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/MainLoginView" />

    <!-- Logo and slogan section -->
    <div class="content">
      <q-img src="src/assets/pic/logo1.png" class="logo"></q-img>
      <q-img src="src/assets/pic/EverSay.png" class="eversay"></q-img>
      <div class="title">
        <h7>Connecting people. One celebration at a time</h7>
      </div>
    </div>

    <!-- Card container for login form -->
    <q-card class="q-pa-md">
      <q-card-section>
        <div class="text">Sign in</div>
      </q-card-section>

      <q-card-section>
        <!-- Login form -->
        <q-form @submit.prevent="handleLogin" class="form-container">
          <!-- Email input -->
          <q-input
            v-model="email"
            label="Username"
            style="width: 100%; max-width: 350px"
            label-color="accent"
            :rules="[(val) => !!val || 'Username is required']"
          >
            <template v-slot:prepend>
              <q-icon name="email" />
            </template>
          </q-input>

          <!-- Password input -->
          <q-input
            v-model="password"
            label="Password"
            type="password"
            style="width: 100%; max-width: 350px"
            label-color="accent"
            :rules="[(val) => !!val || 'Password is required']"
          >
            <template v-slot:prepend>
              <q-icon name="lock" />
            </template>
          </q-input>

          <!-- Remember me and forgot password links -->
          <div class="remember-container">
            <!-- ❗️Error: binding password to checkbox! Should use separate boolean flag -->
            <q-checkbox v-model="password" label="Remember Me" color="accent" />
            <router-link to="/forgot-password" class="forgot-password">
              Forgot password?
            </router-link>
          </div>

          <!-- Submit button -->
          <q-btn
            type="submit"
            label="Login"
            class="login-btn"
            rounded
            :loading="loading"
            :disable="loading"
          />
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

// Handle login form submission
const handleLogin = async (): Promise<void> => {
  loading.value = true
  try {
    await authStore.login({ email: email.value, password: password.value })

    Notify.create({
      type: 'positive',
      message: 'Login successful!',
      timeout: 1000,
    })

    // Navigate to home after login
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
  background-image: linear-gradient(to bottom, rgba(140, 140, 137, 0.7), rgba(140, 140, 137, 0.7)),
    url('src/assets/pic/login.jpg');
  background-size: cover;
  background-position: center;
  flex-direction: column;
  background-color: #ebedff;
  background-repeat: no-repeat;
  margin-top: -100;
  display: flex;
  justify-content: center;
  align-items: center;
}
.q-card {
  margin-top: -100;
  border-radius: 10px;
  width: 320px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  padding: 5px;
  transition: all 0.3s ease;
}

.logo {
  width: 150px;
  margin-top: 10px;
}

.text {
  color: #8151c4;
  font-size: 1.5rem;
  font-weight: bold;
  text-align: center;
}

.eversay {
  width: 200px;
  margin-top: -50px;
}
.title {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  width: 100%;
}

.title h7 {
  font-weight: bold;
  margin-bottom: 50px;
  color: white;
}
.title p {
  color: white;
  font-size: 18px;
  text-align: center;
  width: 80%;
  max-width: 100%;
  margin-bottom: 20px;
}
.form-container {
  display: flex;
  flex-direction: column;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0px;
  align-items: center;
}
.content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  width: 100%;
  height: 100%;
  padding: 20px;
  margin-top: 10px;
  margin-bottom: -50px;
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

.remember-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  max-width: 350px;
  margin-top: 20px;
}

.separator {
  display: flex;
  align-items: center;
  width: 100%;
  justify-content: space-between;
  margin: 20px 0;
  margin-top: -20px;
}

.line {
  flex-grow: 1;
  height: 1px;
  background-color: #aaa;
}

.or-text {
  margin: 0 10px;
  color: #666;
  font-size: 14px;
}

.social-login {
  display: flex;
  flex-direction: column;
  gap: 10px;
  width: 100%;
  align-items: center;
}

.button-group {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  margin-top: -40px;
}

.email-btn {
  background: #d9d9d9;
  border: 1px solid #ccc;
  font-weight: bold;
  color: black;
  border-radius: 50px;
}

.login-btn {
  margin-top: 30px;
  background: #5b3b8b;
  color: white;
  background: linear-gradient(to right, #6c3baa, #9183f1);
}
</style>
