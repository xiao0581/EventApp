<template>
  <div class="Guestsheader">
    <q-btn flat class="back-btn" icon="arrow_back_ios" to="/event/1"></q-btn>
    <h6>Our guests</h6>
  </div>

  <div class="MyProfile">
    <h6>My profile</h6>
  </div>

  <div class="AllGuests">
    <h6>All guests</h6>
    <div class="guests-icons">
      <q-btn flat icon="sym_o_grid_view"></q-btn>
      <q-btn flat icon="sym_o_swap_vert"></q-btn>
    </div>
  </div>
  <div class="guests-list">
    <GuestList v-if="event" :guests="event.guests" :grouped="true" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useEventStore } from 'src/stores/11'

import GuestList from 'src/components/GuestListcompo.vue'

const route = useRoute()
const eventStore = useEventStore()

const eventId = computed(() => parseInt(route.params.id as string))
const event = computed(() => eventStore.getEventById(eventId.value))
</script>

<style scoped>
.Guestsheader {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 0px 0px;
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
  padding: 0px 0px;
  margin-top: -30px;
  position: relative;
}

.guests-icons .q-btn {
  margin-left: -25px;
  margin-right: 5px;
}
</style>
