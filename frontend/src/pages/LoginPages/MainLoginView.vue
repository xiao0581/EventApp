<template>
  <q-page class="flex flex-center">
    <div class="content">
      <q-img src="src/assets/pic/logo1.png" class="logo"></q-img>

      <div class="title">
        <h5>WELCOME TO EVERSAY</h5>
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
      </div>
    </div>
  </q-page>
</template>

<script lang="ts"></script>

<style scoped>
.q-page {
  min-height: 100vh;
  background-image: url('src/assets/pic/login.jpg');
  background-size: cover;
  background-position: center;

  background-color: #ebedff;

  background-repeat: no-repeat;
  height: 100vh;
  margin: 0;
  display: flex;
  justify-content: center;
  align-items: center;
}

.logo {
  width: 150px;
  margin-top: 50px;
}

.title {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  width: 100%;
}

.title h5 {
  font-size: 30px;
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
}

.button-group {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 250px;
  gap: 20px;
}

.register-btn {
  width: 200px;
}
.login-btn {
  width: 200px;
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
</style>
