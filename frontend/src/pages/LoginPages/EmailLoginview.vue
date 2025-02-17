<template>
  <q-page class="flex flex-center">
    <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/MainLoginView" />
    <div class="content">
      <q-img src="src/assets/pic/logo1.png" class="logo"></q-img>

      <div class="title">
        <h5>WELCOME TO EVERSAY</h5>
        <p>Experience effortless event planning and create lasting memories together</p>
      </div>
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
  margin-bottom: 5px;
  color: white;
}
.title p {
  font-size: 18px;
  text-align: center;
  width: 80%;
  max-width: 100%;
  margin-bottom: 20px;
  color: white;
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
  width: 300px;
  gap: 20px;
}
.register-btn {
  width: 200px;
}
.login-btn {
  width: 200px;
}

.back-btn {
  position: absolute;
  top: 16px;
  left: 16px;
  z-index: 10;
  background: transparent;
  box-shadow: none;
  border: none;
  padding: 0;
}
</style>
