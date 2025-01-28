<template>
  <q-page class="event-details-page">
    <div class="event-header">
      <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/home" />
      <q-img :src="event?.image" alt="Event Banner" class="event-image" />
      <h1>{{ event?.name }}</h1>
      <div class="event-info">
        <p class="event-description" :class="{ expanded: isDescriptionExpanded }">
          {{ event?.discription }}
        </p>
        <q-btn
          v-if="(event?.discription || '').length > 100"
          flat
          label="Show more"
          v-show="!isDescriptionExpanded"
          @click="toggleDescription"
          class="show-more-btn"
          style="text-transform: none"
        />
        <q-btn
          v-if="(event?.discription || '').length > 100"
          flat
          label="Show less"
          v-show="isDescriptionExpanded"
          @click="toggleDescription"
          class="show-less-btn"
          style="text-transform: none"
        />

        <div class="event-details-time">
          <p class="event-date"><q-icon name="date_range" /> {{ event?.date }}</p>
          <p class="event-time">
            <q-icon name="schedule" /> {{ event?.startTime }} - {{ event?.endTime }}
          </p>
          <p class="event-location"><q-icon name="place" /> {{ event?.location }}</p>
          <div id="map" class="map-container"></div>
        </div>
        <div class="event-preview-video">
          <p class="event-preview">Catch the celebration vibe with a quick preview</p>
          <q-img src="src/assets/pic/eventVideo.png" class="event-video"></q-img>
        </div>
      </div>
    </div>

    <div class="event-section">
      <h5>Our guests</h5>
      <GuestListcompo v-if="event" :guests="event.guests" />
    </div>

    <div class="event-section">
      <h2></h2>
      <div class="memories-grid">
        <q-img
          v-for="memory in event?.memories"
          :key="memory.id"
          :src="memory.url"
          :alt="memory.type"
          class="memory-item"
          @click="viewMemory(memory)"
        />
      </div>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useEventStore } from 'src/stores/eventstores'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import GuestListcompo from 'src/components/GuestListcompo.vue'

const route = useRoute()
const eventStore = useEventStore()

const eventId = computed(() => parseInt(route.params.id as string))
const event = computed(() => eventStore.getEventById(eventId.value))

const isDescriptionExpanded = ref(false)

const toggleDescription = () => {
  isDescriptionExpanded.value = !isDescriptionExpanded.value
}

const viewMemory = (memory: { id: number; type: string; url: string }) => {
  alert(`Viewing memory: ${memory.url}`)
}

onMounted(() => {
  const map = L.map('map').setView([51.505, -0.09], 13)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '',
  }).addTo(map)

  L.marker([55.673, 12.5681]).addTo(map).bindPopup('Event location').openPopup()
})
</script>

<style scoped>
.event-image {
  width: 100%;
  height: 300px;
  border-bottom-right-radius: 20px;
  border-bottom-left-radius: 20px;
  object-fit: cover;
}
.event-details-page {
  display: flex;
  flex-direction: column;
}

.event-header {
  position: relative;
}
.event-date {
  font-weight: bold;
}

.event-time {
  font-weight: bold;
}

.event-location {
  font-weight: bold;
}
.show-more-btn {
  color: gray;
  top: -20px;
  left: -15px;
}
.show-less-btn {
  color: gray;
  top: -20px;
  left: -15px;
}
#map {
  width: 100%;
  height: 150px;
  margin-top: 16px;
  border: 1px solid #ddd;
  border-radius: 20px;
}
.event-description {
  font-weight: bold;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: normal;
  transition: all 0.3s ease-in-out;
  margin-bottom: 8px;
}

.event-description.expanded {
  -webkit-line-clamp: unset;
  overflow: visible;
  white-space: normal;
}
.event-info {
  padding: 16px;
}

.event-video {
  border-radius: 20px;
}

.event-info p {
  color: #666;
  margin-bottom: 16px;
}
.event-preview-video {
  font-weight: bold;
  margin-top: 30px;
}
.event-section {
  margin: 16px;
}

.event-section h2 {
  margin-bottom: 12px;
}

.memories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 8px;
}

.memory-item {
  width: 100%;
  height: 100px;
  object-fit: cover;
  border-radius: 8px;
  cursor: pointer;
}
.event-header h1 {
  position: absolute;
  margin: 8px 0;
  font-size: 1.5rem;
  font-weight: bold;
  top: 210px;
  left: 50px;
  color: white;
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
