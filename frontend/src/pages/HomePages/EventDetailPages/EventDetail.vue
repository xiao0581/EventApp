<template>
  <q-page class="event-details-page">
    <div class="event-header">
      <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/home" />
      <q-img
        v-if="sasToken"
        :src="getEventImage(event?.eventImage)"
        alt="Event Banner"
        class="event-image"
      />
      <h1>{{ event?.eventTitle }}</h1>
      <div class="event-info">
        <p class="event-description" :class="{ expanded: isDescriptionExpanded }">
          {{ event?.eventDescription }}
        </p>
        <q-btn
          v-if="(event?.eventDescription || '').length > 100"
          flat
          label="Show more"
          v-show="!isDescriptionExpanded"
          @click="toggleDescription"
          class="show-more-btn"
          style="text-transform: none"
        />
        <q-btn
          v-if="(event?.eventDescription || '').length > 100"
          flat
          label="Show less"
          v-show="isDescriptionExpanded"
          @click="toggleDescription"
          class="show-less-btn"
          style="text-transform: none"
        />

        <div class="event-details-time">
          <p class="event-date"><q-icon name="date_range" /> {{ formatDate(event?.eventDate) }}</p>
          <p class="event-time">
            <q-icon name="schedule" /> {{ formatTime(event?.eventDate) }} -
            {{ formatEndTime(event?.eventDate, event?.duration) }}
          </p>
          <p class="event-location"><q-icon name="place" /> {{ event?.eventLocation }}</p>
          <div id="map" class="map-container"></div>
        </div>
        <div class="event-preview-video">
          <p class="event-preview">Catch the celebration vibe with a quick preview</p>
          <q-video
            v-if="sasToken"
            :src="getEventvideo(event?.eventPreview)"
            class="event-video"
          ></q-video>
        </div>
      </div>
    </div>

    <!-- <div class="event-section">
      <h6>Our guests ({{ event?.guests.length || 0 }})</h6>
      <GuestListcompo v-if="event" :guests="limitedGuests" :grouped="false" />
      <q-btn
        flat
        label="View all guests"
        class="View-more-btn"
        :to="`/event/${event?.eventId}/guests`"
        style="text-transform: none"
      />
    </div> -->

    <router-view />
  </q-page>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { eventStores } from 'src/stores/eventstores'
import { getReadSasToken } from 'src/utils/azureUploader'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
/* import GuestListcompo from 'src/components/GuestListcompo.vue' */

const route = useRoute()
const userEvent = eventStores()
const eventId = computed(() => route.params.id as string)
const event = computed(() => userEvent.event)
const sasToken = ref('')
const isDescriptionExpanded = ref(false)

onMounted(async () => {
  await userEvent.fetchEventById(eventId.value)
  sasToken.value = await getReadSasToken()

  if (event.value?.eventLocation) {
    const coordinates = await getCoordinates(event.value.eventLocation)

    if (coordinates) {
      loadMap(coordinates.lat, coordinates.lon)
    }
  }
})

//leaflet map
const loadMap = (latitude: number, longitude: number) => {
  const map = L.map('map').setView([latitude, longitude], 13)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '',
  }).addTo(map)

  L.marker([latitude, longitude]).addTo(map).bindPopup('Event location').openPopup()
}

const getEventImage = (image: string | File | null | undefined): string => {
  if (!image) return 'default-event.jpg'

  if (typeof image === 'string') {
    return `${image}${sasToken.value}`
  }

  if (image instanceof File) {
    return URL.createObjectURL(image)
  }

  return 'default-event.jpg'
}

const formatDate = (isoString: string | undefined) => {
  if (!isoString) return ''
  const date = new Date(isoString)
  return date.toLocaleDateString('en-GB', { year: 'numeric', month: '2-digit', day: '2-digit' })
}

const formatTime = (isoString: string | undefined) => {
  if (!isoString) return ''
  const date = new Date(isoString)
  return date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })
}

const formatEndTime = (isoString: string | undefined, duration: string | undefined) => {
  if (!isoString || !duration) return ''
  const durationFloat = parseFloat(duration)
  if (isNaN(durationFloat)) return ''
  const hours = Math.floor(durationFloat)
  const minutes = Math.round((durationFloat - hours) * 60)
  const date = new Date(isoString)
  date.setHours(date.getHours() + hours)
  date.setMinutes(date.getMinutes() + minutes)
  return date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })
}

const getEventvideo = (video: string | File | null | undefined): string => {
  if (!video) return 'default-event.jpg'

  if (typeof video === 'string') {
    return `${video}${sasToken.value}`
  }

  if (video instanceof File) {
    return URL.createObjectURL(video)
  }

  return 'default-event.jpg'
}
/* const limitedGuests = computed(() => event.value?.guests.slice(0, 6) || []) */

const toggleDescription = () => {
  isDescriptionExpanded.value = !isDescriptionExpanded.value
}

//OpenStreetMap Nominatim API
const getCoordinates = async (address: string) => {
  if (!address) {
    console.warn('No address provided')
    return null
  }

  console.log(`Fetching coordinates for: ${address}`)

  try {
    const response = await fetch(
      `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}`,
    )
    const data = await response.json()

    console.log('Nominatim API Response:', data)

    if (data.length > 0) {
      const location = data[0]
      console.log(`Coordinates found: lat=${location.lat}, lon=${location.lon}`)
      return { lat: parseFloat(location.lat), lon: parseFloat(location.lon) }
    } else {
      console.error('Geocoding failed: No results found')
      return null
    }
  } catch (error) {
    console.error('Error fetching coordinates:', error)
    return null
  }
}
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

.event-info p {
  color: #666;
  margin-bottom: 16px;
}
.event-video {
  width: 100%;
  height: 200px;
  object-fit: cover;
  border-radius: 20px;
}
.event-preview-video {
  font-weight: bold;
  margin-top: 30px;
}

.event-section h6 {
  margin-bottom: 10px;
  font-weight: bold;
}

.event-section h2 {
  margin-bottom: 12px;
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

.View-more-btn {
  margin-left: 120px;
}
</style>
