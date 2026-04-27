<template>
  <q-page class="flex flex-center">
    <div class="content">
      <div class="title">
        <h7>Connecting people. One celebration at a time</h7>
      </div>

      <div class="button-group">
        <q-btn label="LOG IN" rounded class="login-btn" to="/login" />

        <q-btn label="SIGN UP" rounded to="/registers" class="register-btn" />
      </div>
    </div>
  </q-page>
</template>

<script lang="ts"></script>

<style scoped>
.q-page {
  min-height: 100vh;
  background-image: linear-gradient(to bottom, rgba(140, 140, 137, 0.7), rgba(140, 140, 137, 0.7)),
    url('src/assets/pic/login.jpg');
  background-size: cover;
  background-position: center;
  background-color: #ebedff;
  background-repeat: no-repeat;
  margin: 0;
  display: flex;
  justify-content: center;
  align-items: center;
}

.logo {
  width: 150px;
  margin-top: -80px;
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
.content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  width: 100%;
  height: 100%;
  padding: 20px;
  margin-top: 50px;
}

.button-group {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 350px;
  gap: 20px;
  margin-top: 250px;
}
.register-btn {
  background: white;
  color: #5b3b8b;
  width: 100%;
}
.login-btn {
  background: #5b3b8b;
  color: white;
  background: linear-gradient(to right, #6c3baa, #9183f1);
  width: 100%;
}
</style>
