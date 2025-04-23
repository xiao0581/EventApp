<template>
  <q-page class="event-details-page">
    <div class="event-header">
      <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/home" />
      <q-btn
        v-if="isEditing"
        flat
        round
        icon="check"
        color="positive"
        class="save-btn"
        @click="updateEvent"
      />
      <q-btn
        v-if="canEditEvent"
        flat
        round
        icon="edit"
        color="primary"
        class="edit-btn"
        @click="toggleEdit"
      />
      <q-img
        v-if="sasToken && !isEditing"
        :src="getEventImage(event?.eventImage)"
        alt="Event Banner"
        class="event-image"
      />

      <div v-else class="q-mt-md">
        <input
          type="file"
          accept="image/*"
          ref="imageInputRef"
          class="hidden-file-input"
          @change="onImageSelected"
        />
        <q-img
          v-if="sasToken"
          :src="getEventImage(editedEvent.eventImage)"
          class="event-image clickable"
          @click="triggerImageInput"
        />
      </div>

      <h1 v-if="!isEditing" class="event-title-display">{{ event?.eventTitle }}</h1>

      <div class="event-info">
        <q-input
          v-if="isEditing"
          v-model="editedEvent.eventTitle"
          label="please enter new event title"
          filled
          class="edit-field"
        />

        <p v-if="!isEditing" class="event-description" :class="{ expanded: isDescriptionExpanded }">
          {{ event?.eventDescription }}
        </p>
        <q-input
          v-else
          v-model="editedEvent.eventDescription"
          label="please enter new description"
          filled
          class="q-mt-md edit-field"
        />
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
          <p v-if="!isEditing" class="event-date">
            <q-icon name="date_range" /> {{ formatDate(event?.eventDate) }}
          </p>
          <q-input v-else filled v-model="editedEvent.eventDate" class="edit-field">
            <template v-slot:prepend>
              <q-icon name="event" class="cursor-pointer">
                <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                  <q-date v-model="editedEvent.eventDate" mask="YYYY-MM-DD HH:mm">
                    <div class="row items-center justify-end">
                      <q-btn v-close-popup label="Close" color="primary" flat />
                    </div>
                  </q-date>
                </q-popup-proxy>
              </q-icon>
            </template>

            <template v-slot:append>
              <q-icon name="access_time" class="cursor-pointer">
                <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                  <q-time v-model="editedEvent.eventDate" mask="YYYY-MM-DD HH:mm" format24h>
                    <div class="row items-center justify-end">
                      <q-btn v-close-popup label="Close" color="primary" flat />
                    </div>
                  </q-time>
                </q-popup-proxy>
              </q-icon>
            </template>
          </q-input>

          <p v-if="!isEditing" class="event-time">
            <q-icon name="schedule" /> {{ formatTime(event?.eventDate) }} -
            {{ formatEndTime(event?.eventDate, event?.duration) }}
          </p>

          <q-input
            v-else
            type="number"
            v-model="editedEvent.duration"
            label="please enter new hours"
            filled
            class="q-mt-md edit-field"
          />

          <p v-if="!isEditing" class="event-location">
            <q-icon name="place" /> {{ event?.eventLocation }}
          </p>
          <q-input
            v-else
            v-model="editedEvent.eventLocation"
            label="please enter new location"
            filled
            class="q-mt-md edit-field"
          />
          <div id="map" class="map-container"></div>
          <p v-if="event && !event.eventLocation" class="text-center text-grey">
            📍 No location provided for this event.
          </p>
        </div>
        <div class="event-preview-video">
          <p class="event-preview">Catch the celebration vibe with a quick preview</p>
          <q-video
            v-if="sasToken && !isEditing"
            :src="getEventvideo(event?.eventPreview)"
            class="event-video"
          ></q-video>

          <div v-else class="q-mt-md">
            <input
              type="file"
              accept="video/*"
              ref="videoInputRef"
              class="hidden-file-input"
              @change="onVideoSelected"
            />
            <div class="video-wrapper q-mt-sm" @click="triggerVideoInput">
              <q-video
                v-if="editedEvent.eventPreview"
                :src="getEventvideo(editedEvent.eventPreview)"
                class="event-video"
              />
              <div class="video-overlay" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="event-section">
      <div class="row items-center justify-between q-pr-sm">
        <h6 class="q-mb-sm">Our guests</h6>
        <q-btn
          v-if="canEditEvent"
          dense
          color="primary"
          icon="link"
          label="invite guests"
          @click="onGenerateInvite"
        />
      </div>

      <GuestListcompo :event-id="eventId" :grouped="false" />
      <q-btn
        flat
        label="View all guests"
        class="View-more-btn"
        :to="`/event/${event?.eventId}/guests`"
        style="text-transform: none"
      />

      <q-dialog v-model="showInviteDialog">
        <q-card style="min-width: 350px; max-width: 500px">
          <q-card-section>
            <div class="text-h6">InviteLink</div>
            <q-input v-model="inviteLink" readonly filled />
          </q-card-section>
          <q-card-actions align="right">
            <q-btn
              flat
              label="Copy link"
              color="primary"
              icon="content_copy"
              @click="copyInviteLink"
            />
            <q-btn flat label="close" color="negative" v-close-popup />
          </q-card-actions>
        </q-card>
      </q-dialog>
    </div>

    <router-view />
  </q-page>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, watch } from 'vue'
import { useAuthStore } from 'src/stores/auth'
import { useRoute } from 'vue-router'
import { eventStores } from 'src/stores/eventstores'
import { getReadSasToken } from 'src/utils/azureUploader'
import { uploadToAzureBlob } from 'src/utils/azureUploader'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import GuestListcompo from 'src/components/GuestListcompo.vue'

const showInviteDialog = ref(false)
const inviteLink = ref('')

const route = useRoute()
const userEvent = eventStores()
const authStore = useAuthStore()
const eventId = computed(() => route.params.id as string)
const event = computed(() => userEvent.event)
const currentUserId = computed(() => authStore.user?.userId)
const canEditEvent = computed(() => currentUserId.value === event.value?.createdBy)
const isEditing = ref(false)
const editedEvent = ref({ ...event.value })
const sasToken = ref('')
const isDescriptionExpanded = ref(false)
const selectedImageFile = ref<File | null>(null)
const selectedVideoFile = ref<File | null>(null)

onMounted(async () => {
  await userEvent.fetchEventById(eventId.value)
  sasToken.value = await getReadSasToken()

  if (event.value?.eventLocation) {
    const coordinates = await getCoordinates(event.value.eventLocation)

    if (coordinates) {
      loadMap(coordinates.lat, coordinates.lon)
    } else {
      loadMap(51.505, -0.09)
    }
  } else {
    loadMap(51.505, -0.09)
  }
})

watch(event, (newEvent) => {
  if (!isEditing.value && newEvent) {
    const date = new Date(newEvent.eventDate)
    const formattedDate = date.toISOString().slice(0, 16).replace('T', ' ')
    editedEvent.value = { ...newEvent, eventDate: formattedDate }
  }
})

//generate invite link
const onGenerateInvite = async () => {
  if (!event.value?.eventId) return

  const code = await userEvent.generateInviteLink(event.value.eventId)
  if (code) {
    inviteLink.value = `${window.location.origin}/#/invite/${code}`
    showInviteDialog.value = true
  }
}

const copyInviteLink = async () => {
  if (!inviteLink.value) return
  await navigator.clipboard.writeText(inviteLink.value)
  alert('copied to clipboard')
}

// clickable image input
const imageInputRef = ref<HTMLInputElement | null>(null)

const triggerImageInput = () => {
  imageInputRef.value?.click()
}

const onImageSelected = (e: Event) => {
  const target = e.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    selectedImageFile.value = file
    editedEvent.value.eventImage = file
  }
}

// clickable video input
const videoInputRef = ref<HTMLInputElement | null>(null)

const triggerVideoInput = () => {
  videoInputRef.value?.click()
}

const onVideoSelected = (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (file) {
    selectedVideoFile.value = file
    editedEvent.value.eventPreview = file
  }
}

const toggleEdit = () => {
  isEditing.value = !isEditing.value
}

const updateEvent = async () => {
  try {
    const updatedData = { ...editedEvent.value }

    if (editedEvent.value.eventImage instanceof File) {
      const uploadedImageUrl = await uploadToAzureBlob(editedEvent.value.eventImage)
      updatedData.eventImage = uploadedImageUrl
    }

    if (editedEvent.value.eventPreview instanceof File) {
      const uploadedVideoUrl = await uploadToAzureBlob(editedEvent.value.eventPreview)
      updatedData.eventPreview = uploadedVideoUrl
    }

    await userEvent.updateEvent(eventId.value, updatedData)
    await userEvent.fetchEventById(eventId.value)
    isEditing.value = false
  } catch (err) {
    console.error('Error updating event:', err)
  }
}

//leaflet map
const loadMap = (latitude: number, longitude: number) => {
  const map = L.map('map').setView([latitude, longitude], 13)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '',
  }).addTo(map)

  L.marker([latitude, longitude]).addTo(map).openPopup().bindPopup('Event location')
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

  try {
    const response = await fetch(
      `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}`,
    )
    const data = await response.json()

    if (data.length > 0) {
      const location = data[0]

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
  color: #4a4e69;
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
  margin-top: 0px;
  margin-left: 12px;
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

.edit-btn {
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 10;
  background: transparent;
  box-shadow: none;
  border: none;
  padding: 0;
}

.save-btn {
  position: absolute;
  top: 16px;
  right: 56px;
  z-index: 10;
  background: transparent;
  box-shadow: none;
  border: none;
  padding: 0;
}

.View-more-btn {
  margin-left: 120px;
}
.hidden-file-input {
  display: none;
}

.clickable {
  cursor: pointer;
}

.video-wrapper {
  position: relative;
  width: 100%;
  height: 200px;
}

.video-wrapper .event-video {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 20px;
  pointer-events: none; /* Prevent interaction with the video */
}

.video-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  cursor: pointer;
  border-radius: 20px;
  background-color: transparent;
  z-index: 2;
}

.edit-field {
  margin-top: 12px;
  margin-bottom: 12px;
}

.text-grey {
  color: #999;
  font-style: italic;
  margin-top: 10px;
}
</style>
