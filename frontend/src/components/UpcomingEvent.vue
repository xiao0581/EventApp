<template>
  <div
    class="event-card"
    v-for="event in upcomingEvents"
    :key="event.id"
    @click="goToEvent(event.id)"
  >
    <div class="event-card-image">
      <q-img :src="event.image" alt="Event image" />
    </div>
    <div class="event-card-content">
      <h5 class="event-title">{{ event.name }}</h5>
      <p><q-icon name="schedule" /> {{ event.startTime }} | {{ event.date }}</p>
      <p><q-icon name="place" /> {{ event.location }}</p>

      <div class="event-guests">
        <div class="guest-avatars">
          <q-avatar v-for="guest in event.guests.slice(0, 5)" :key="guest.id" size="32px">
            <q-img :src="guest.avatar" alt="Guest avatar" />
          </q-avatar>
          <span class="additional-guests" v-if="event.guests.length > 5">
            +{{ event.guests.length - 5 }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useEventStore } from 'src/stores/eventstores'
import { useRouter } from 'vue-router'

const eventStore = useEventStore()
const router = useRouter()
const today = new Date()
const dayAfterTomorrow = new Date()
dayAfterTomorrow.setDate(today.getDate() + 2)

const upcomingEvents = computed(() => {
  return eventStore.events
    .map((event) => ({
      ...event,
      guests: event.guests || [],
    }))
    .filter((event) => {
      const eventDate = new Date(event.date)
      return eventDate >= today
    })
})

const goToEvent = async (id: number) => {
  await router.push(`/event/${id}`)
}
</script>

<style scoped>
.event-card {
  display: flex;
  flex-direction: row;
  align-items: center;
  margin: 12px;
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: transform 0.2s ease-in-out;
}

.event-card-image {
  width: 100px;
  height: 120px;
  border-radius: 12px;
  overflow: hidden;
  margin-left: 12px;
  margin-right: 12px;
  margin-bottom: 5px;
  flex-shrink: 0;
}

.event-card-image img,
.event-card-image .q-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 12px;
}

.event-card-content {
  flex: 1;
  margin-bottom: 30px;
}

.event-card-content p {
  margin: 2px 0;
}

.event-title {
  font-size: 16px;
  font-weight: bold;
  margin-bottom: 3px;
}

.guest-avatars {
  display: flex;
  align-items: center;
  gap: 4px;
}

.q-avatar {
  margin-right: 4px;
}

.additional-guests {
  margin-left: 8px;
  font-size: 12px;
  color: #757575;
}

.all-guests {
  font-size: 12px;
  color: #42a5f5;
  cursor: pointer;
}
</style>
