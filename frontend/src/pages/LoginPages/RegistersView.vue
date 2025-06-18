<template>
  <!-- Back button to return to login -->
  <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/MainLoginView" />

  <!-- Page wrapper with background and centered content -->
  <q-page class="flex flex-center">
    <div class="content">
      <q-img src="src/assets/pic/logo1.png" class="logo"></q-img>
      <q-img src="src/assets/pic/EverSay.png" class="eversay"></q-img>
      <div class="title">
        <h7>Connecting people. One celebration at a time</h7>
      </div>
    </div>

    <!-- Registration card -->
    <q-card class="q-pa-md">
      <q-card-section>
        <div class="text">Sign up</div>
      </q-card-section>

      <q-card-section>
        <!-- Registration form -->
        <q-form @submit.prevent="handleRegister" class="form-container">
          <q-input
            v-model="form.email"
            label="Email"
            type="email"
            label-color="accent"
            style="width: 100%; max-width: 350px"
            :rules="[(val) => !!val || 'Email is required']"
          >
            <template v-slot:prepend>
              <q-icon name="email" />
            </template>
          </q-input>

          <!-- Name field -->
          <q-input
            v-model="form.Name"
            label="Name"
            type="text"
            label-color="accent"
            style="width: 100%; max-width: 350px"
            :rules="[(val) => !!val || 'Name is required']"
          >
            <template v-slot:prepend>
              <q-icon name="email" />
            </template>
          </q-input>

          <!-- Password field -->
          <q-input
            v-model="form.password"
            label="Password"
            type="password"
            label-color="accent"
            style="width: 100%; max-width: 350px"
            :rules="[(val) => !!val || 'Email is required']"
          >
            <template v-slot:prepend>
              <q-icon name="lock" />
            </template>
          </q-input>

          <!-- Confirm password field -->
          <q-input
            v-model="form.confirmPassword"
            label="Confirm Password"
            type="password"
            label-color="primary"
            style="width: 100%; max-width: 350px"
            :rules="[(val) => !!val || 'Password is required']"
          >
            <template v-slot:prepend>
              <q-icon name="lock" />
            </template>
          </q-input>

          <!-- Submit button -->
          <q-btn
            type="submit"
            label="Register"
            class="Register-btn"
            rounded
            style="width: 100%; max-width: 350px"
            :loading="loading"
            :disable="loading"
          />

          <!-- Link to login page -->
          <div class="signup-container">
            <span>Joined us before? </span>
            <router-link to="/login" class="signup-link">Log in</router-link>
          </div>
        </q-form>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useAuthStore } from 'src/stores/auth'
import { Notify } from 'quasar'

const form = reactive({
  password: '',
  email: '',
  Name: '',
  confirmPassword: '',
})

const loading = ref<boolean>(false)

const authStore = useAuthStore()

// Register user
const handleRegister = async (): Promise<void> => {
  loading.value = true
  try {
    await authStore.register({
      password: form.password,
      Name: form.Name,
      email: form.email,
      confirmPassword: form.confirmPassword,
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
.text-h6 {
  color: #8151c4;
  font-size: 1.5rem;
  font-weight: bold;
  text-align: center;
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

.Register-btn {
  background: #5b3b8b;
  color: white;
  background: linear-gradient(to right, #6c3baa, #9183f1);
}

.signup-container span {
  margin-right: 10px;
}
</style>
