<template>
  <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/MainLoginView" />
  <q-page class="flex flex-center">
    <div class="content">
      <q-img src="src/assets/pic/logo1.png" class="logo"></q-img>
      <q-img src="src/assets/pic/EverSay.png" class="eversay"></q-img>
      <div class="title">
        <h7>Connecting people. One celebration at a time</h7>
      </div>

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
        <div class="signup-container">
          <span>Don't have an account? </span>
          <router-link to="/registers" class="signup-link">Sign up</router-link>
        </div>
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
  flex-direction: column;
  background-color: #ebedff;
  background-repeat: no-repeat;
  margin: 0;
  display: flex;
  justify-content: center;
  align-items: center;
}

.logo {
  width: 150px;
  margin-top: 10px;
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
  margin-top: 230px;
}

.login-option {
  width: 100%;
  height: 45px;
  font-size: 14px;
  font-weight: 500;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.google-btn {
  background: white;
  border: 1px solid #ccc;
  font-weight: bold;
  color: black;
  border-radius: 50px;
}

.apple-btn {
  background: white;
  border: 1px solid #ccc;
  font-weight: bold;
  color: black;
  border-radius: 50px;
}

.email-btn {
  background: white;
  border: 1px solid #ccc;
  font-weight: bold;
  color: black;
  border-radius: 50px;
}
.signup-container span {
  color: white;
  font-size: 14px;
}
.signup-link {
  color: rgb(108, 59, 170);
}

.back-btn {
  position: absolute;
  top: 16px;

  z-index: 10;
  background: transparent;
  box-shadow: none;
  border: none;
  padding: 0;
}
</style>
