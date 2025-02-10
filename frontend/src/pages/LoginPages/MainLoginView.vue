<template>
  <q-page class="flex flex-center">
    <div class="content">
      <q-img src="src/assets/pic/logo.png" class="logo"></q-img>

      <div class="button-group">
        <q-btn label="LOG IN" color="primary" class="login-btn" to="/login" />
        <q-btn
          label="SIGN UP"
          color="secondary"
          text-color="primary"
          to="/registers"
          class="register-btn"
        />
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
  background-repeat: no-repeat;
  height: 100vh;
  margin: 0;
  display: flex;
  justify-content: center;
  align-items: center;
}

.content {
  text-align: center;
}

.logo {
  width: 300px;
  margin-top: -50px;
  filter: invert(1) brightness(2);
}
.button-group {
  display: flex;
  justify-content: center;
  width: 300px;
  margin-left: 100px;
  margin-top: 150px;
  flex-direction: column;
  gap: 20px;
}

.register-btn {
  width: 200px;
}
.login-btn {
  width: 200px;
}
</style>
