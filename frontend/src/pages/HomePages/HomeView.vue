<template>
  <div class="homecontainer">
    <p class="hometitle">Warm memories start here.</p>
    <q-btn class="homebutton" to="/EventForm" unelevated rounded label="+ Create an event" />
  </div>

  <div class="events-section">
    <h2>Celebration in 2 days</h2>
    <div class="event-card" v-for="event in events" :key="event.id" @click="goToEvent(event.id)">
      <div class="event-card-image">
        <q-img :src="event.image" alt="Wedding image" />
      </div>
      <div class="event-card-content">
        <h5 class="event-title">{{ event.name }}</h5>
        <div class="event-details">
          <p class="event-time">
            <q-icon name="schedule" /> {{ event.startTime }} | {{ event.date }}
          </p>
          <p class="event-location"><q-icon name="place" /> {{ event.location }}</p>
        </div>
        <div class="event-guests">
          <div class="guest-avatars">
            <q-avatar v-for="guest in event.guests.slice(0, 5)" :key="guest.id" size="32px">
              <q-img :src="guest.avatar" alt="gest avatar" />
            </q-avatar>
            <span class="additional-guests" v-if="event.guests.length > 5"
              >+{{ event.guests.length - 5 }}</span
            >
          </div>
          <q-btn
            flat
            label="View all guests"
            class="all-guests"
            :to="`/event/${event?.id}/guests`"
            style="text-transform: none"
            @click.stop
          />
        </div>
        <div class="custom-preview-button">
          <span class="button-text">Catch the celebration vibe with a quick preview</span>
          <div class="button-icon">
            <q-icon name="double_arrow" />
          </div>
        </div>
      </div>
    </div>
  </div>

  <div class="invitations-section">
    <h2>My Invitations ({{ invitations.length }})</h2>
    <InvitationsCompo />
  </div>

  <div class="upcoming-events-section">
    <h2>Upcoming Events</h2>
    <UpcomingEvent />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useEventStore } from 'src/stores/eventstores'
import { useRouter } from 'vue-router'
import InvitationsCompo from 'src/components/InvitationsCom.vue'
import UpcomingEvent from 'src/components/UpcomingEvent.vue'

const eventStore = useEventStore()
const router = useRouter()
const today = new Date()
const dayAfterTomorrow = new Date()
dayAfterTomorrow.setDate(today.getDate() + 2)
const invitations = computed(() => eventStore.invitations || [])

const events = computed(() => {
  return eventStore.events
    .map((event) => ({
      ...event,
      guests: event.guests || [],
    }))
    .filter((event) => {
      const eventDate = new Date(event.date)
      return eventDate >= today && eventDate < dayAfterTomorrow
    })
})

const goToEvent = async (id: number) => {
  await router.push(`/event/${id}`)
}
</script>

<style scoped>
.homecontainer {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background-size: cover;
  background-position: center;
  background-image: linear-gradient(to bottom, rgba(74, 78, 105, 0.7), rgba(74, 78, 105, 0.7)),
    url('src/assets/pic/createBackgroundPic.png');
  border-bottom-left-radius: 2rem;
  border-bottom-right-radius: 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  height: 210px;
  padding-top: 4rem;
  gap: 1rem;
}

.hometitle {
  font-size: 1.7rem;
  font-weight: bold;
  color: white;
  margin-bottom: 1rem;
  text-align: center;
  z-index: 10;
  margin: 0;
}

.homebutton {
  background-color: #6a7bff;
  color: white;
  font-weight: bold;
  padding: 0.5rem 5rem;
  border-radius: 9999px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: all 0.2s;
  z-index: 10;
  margin: 0;
  width: 350px;
  height: 10px;
}

.homebutton:hover {
  box-shadow: 0 6px 10px rgba(0, 0, 0, 0.15);
}

.events-section {
  margin: 0 2px;
  padding: 0 10px;
}

.events-section h2 {
  font-size: 1rem;
  margin-bottom: 0px;
}

.event-card {
  display: flex;
  flex-direction: column;
  background: white;
  border-radius: 12px;
  overflow: hidden;
  margin-bottom: 20px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: transform 0.2s ease-in-out;
}

.event-card-image {
  height: 150px;
  overflow: hidden;
  border-radius: 10%;
  margin: 16px 16px;
}

.event-card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.event-card-content {
  padding: 16px;
}

.event-title {
  margin-top: -30px;
  font-size: 20px;
  font-weight: bold;
  margin-bottom: 0;
}

.event-details {
  display: flex;
  flex-direction: column;
  font-size: 14px;
  color: #757575;
  margin-bottom: 0;
}
.event-time {
  margin-bottom: 0;
}
.event-location {
  margin-bottom: 0;
}

.event-details q-icon {
  margin-right: 8px;
  color: #42a5f5;
}

.event-guests {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 2px;
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
  font-size: 14px;
  color: #757575;
}

.custom-preview-button {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  padding: 10px 16px;
  font-size: 14px;
  font-weight: normal;
  color: #4a4e69;
  border: 2px solid #6a7bff;
  border-radius: 30px;
  background-color: transparent;
  transition: all 0.3s ease;
  position: relative;
}

.button-text {
  white-space: nowrap;
}

.button-icon {
  position: absolute;
  top: 50%;
  font-size: 23px;
  right: -2px;
  transform: translateY(-50%);
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #6a7bff;
  color: white;
  border-radius: 50%;
  width: 42px;
  height: 42px;
  transition: all 0.3s ease;
}

.custom-preview-button:hover {
  background-color: #f3f4f6;
}

.custom-preview-button:hover .button-icon {
  background-color: #4a63d9;
}

.invitations-section h2 {
  margin-bottom: -20px;
}
h2 {
  font-size: 1.2rem;
  margin-bottom: 10px;
}

.all-guests {
  font-size: 11px;
  color: #42a5f5;
  cursor: pointer;
}

.upcoming-events-section h2 {
  margin-bottom: -20px;
}
</style>
