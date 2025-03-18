<template>
  <q-page class="flex flex-center">
    <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/MainLoginView2" />
    <div class="content">
      <q-img src="src/assets/pic/logo1.png" class="logo"></q-img>
      <q-img src="src/assets/pic/EverSay.png" class="eversay"></q-img>
      <div class="title">
        <h7>Connecting people. One celebration at a time</h7>
      </div>
    </div>
    <q-card class="q-pa-md">
      <q-card-section>
        <div class="text">Sign in</div>
      </q-card-section>

      <q-card-section>
        <q-form @submit.prevent="handleLogin" class="form-container">
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
          <div class="remember-container">
            <q-checkbox v-model="password" label="Remember Me" color="accent" />
            <router-link to="/forgot-password" class="forgot-password">
              Forgot password?
            </router-link>
          </div>

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

      <q-card-section class="separator">
        <q-separator class="line" />
        <span class="or-text">or</span>
        <q-separator class="line" />
      </q-card-section>

      <q-card-section>
        <div class="button-group">
          <q-btn unelevated class="login-option google-btn">
            <q-icon name="img:/src/assets/pic/google.png" size="20px" />
            <span>Continue with Google</span>
          </q-btn>

          <q-btn unelevated class="login-option apple-btn">
            <q-icon name="img:/src/assets/pic/apple.png" size="20px" />
            <span>Continue with Apple</span>
          </q-btn>

          <q-btn unelevated class="login-option email-btn" to="/login">
            <q-icon name="mail" size="20px" />
            <span>Continue with Email</span>
          </q-btn>
        </div>
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
  margin-top: -20px;
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

.google-btn {
  background: #d9d9d9;
  border: 1px solid #ccc;
  font-weight: bold;
  color: black;
  border-radius: 50px;
}

.apple-btn {
  background: #d9d9d9;
  border: 1px solid #ccc;
  font-weight: bold;
  color: black;
  border-radius: 50px;
}

.email-btn {
  background: #d9d9d9;
  border: 1px solid #ccc;
  font-weight: bold;
  color: black;
  border-radius: 50px;
}

.login-btn {
  background: #5b3b8b;
  color: white;
  background: linear-gradient(to right, #6c3baa, #9183f1);
}
</style>
