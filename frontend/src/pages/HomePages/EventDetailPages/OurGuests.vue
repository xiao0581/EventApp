<template>
  <div class="Guestsheader">
    <q-btn flat class="back-btn" icon="arrow_back_ios" :to="`/event/${eventId}`" />
    <h6>Our guests</h6>
  </div>

  <div class="MyProfile">
    <h6>My profile</h6>
  </div>

  <div class="AllGuests">
    <h6>All guests</h6>
    <div class="guests-icons">
      <q-btn flat icon="sym_o_grid_view" />
      <q-btn flat icon="sym_o_swap_vert" />
    </div>
  </div>

  <div class="guests-list">
    <GuestListcompo :event-id="eventId" :grouped="false" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import GuestListcompo from 'src/components/GuestListcompo.vue'

const route = useRoute()
const eventId = computed(() => route.params.id as string)
</script>

<style scoped>
.Guestsheader {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: -30px;
  position: relative;
}
.back-btn {
  position: absolute;
  background-color: transparent;
  left: 0;
}
.MyProfile {
  margin-top: -80px;
}
.AllGuests {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: -30px;
  position: relative;
}
.guests-icons .q-btn {
  margin-left: -25px;
  margin-right: 5px;
}
</style>
