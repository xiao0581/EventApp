<template>
  <q-page class="event-details-page">
    <div class="event-header">
      <!-- Back to home button -->
      <q-btn flat round icon="arrow_back_ios" color="primary" class="back-btn" to="/home" />

      <!-- Save button (only shown in editing mode) -->
      <q-btn
        v-if="isEditing"
        flat
        round
        icon="check"
        color="positive"
        class="save-btn"
        @click="updateEvent"
      />

      <!-- Edit button (only visible if user is the creator) -->
      <q-btn
        v-if="canEditEvent"
        flat
        round
        icon="edit"
        color="primary"
        class="edit-btn"
        @click="toggleEdit"
      />

      <!-- Event banner image (view mode) -->
      <q-img
        v-if="sasToken && !isEditing"
        :src="getEventImage(event?.eventImage)"
        alt="Event Banner"
        class="event-image"
      />

      <!-- Image upload and preview (edit mode) -->
      <div v-else class="q-mt-md">
        <input
          type="file"
          accept="image/*"
          ref="imageInputRef"
          class="hidden-file-input"
          @change="onImageSelected"
        />

        <!-- Clickable image preview (edit mode) -->
        <q-img
          v-if="sasToken"
          :src="getEventImage(editedEvent.eventImage)"
          class="event-image clickable"
          @click="triggerImageInput"
        />
      </div>

      <!-- Event title (only shown in view mode) -->
      <h1 v-if="!isEditing" class="event-title-display">{{ event?.eventTitle }}</h1>

      <div class="event-info">
        <!-- Event title input (edit mode) -->
        <q-input
          v-if="isEditing"
          v-model="editedEvent.eventTitle"
          label="please enter new event title"
          filled
          class="edit-field"
        />

        <!-- Event description (view mode) -->
        <p v-if="!isEditing" class="event-description" :class="{ expanded: isDescriptionExpanded }">
          {{ event?.eventDescription }}
        </p>

        <!-- Event description input (edit mode) -->
        <q-input
          v-else
          v-model="editedEvent.eventDescription"
          label="please enter new description"
          filled
          class="q-mt-md edit-field"
        />

        <!-- Show more/less toggle button for long description -->
        <q-btn
          v-if="(event?.eventDescription || '').length > 100 && !isEditing"
          flat
          class="show-more-btn"
          :label="isDescriptionExpanded ? 'Show less' : 'Show more'"
          @click="toggleDescription"
          style="text-transform: none; margin-bottom: 8px"
        />

        <!-- Show/hide details button when event has started -->
        <q-btn
          v-else-if="eventStarted && !isEditing"
          flat
          class="show-less-btn"
          :label="isDetailsCollapsed ? 'Show details' : 'Hide details'"
          @click="toggleDetailsManually"
          style="text-transform: none; margin-bottom: 8px"
        />

        <!-- Event details (date, time, location, map) -->
        <div class="event-details-time" v-show="!isDetailsCollapsed || !eventStarted">
          <div class="event-details-time">
            <!-- Event date display (view mode) -->
            <p v-if="!isEditing" class="event-date">
              <q-icon name="date_range" /> {{ formatDate(event?.eventDate) }}
            </p>

            <!-- Event date & time picker (edit mode) -->
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

            <!-- Time & duration display (view mode) -->
            <p v-if="!isEditing" class="event-time">
              <q-icon name="schedule" /> {{ formatTime(event?.eventDate) }} -
              {{ formatEndTime(event?.eventDate, event?.duration) }}
            </p>

            <!-- Duration input (edit mode) -->
            <q-input
              v-else
              type="number"
              v-model="editedEvent.duration"
              label="please enter new hours"
              filled
              class="q-mt-md edit-field"
            />

            <!-- Location display (view mode) -->
            <p v-if="!isEditing" class="event-location">
              <q-icon name="place" /> {{ event?.eventLocation }}
            </p>

            <!-- Location input (edit mode) -->
            <q-input
              v-else
              v-model="editedEvent.eventLocation"
              label="please enter new location"
              filled
              class="q-mt-md edit-field"
            />

            <!-- Map display using Leaflet -->
            <div id="map" class="map-container"></div>

            <!-- Message if location not set -->
            <p v-if="event && !event.eventLocation" class="text-center text-grey">
              📍 No location provided for this event.
            </p>
          </div>

          <!-- Preview video section -->
          <div class="event-preview-video">
            <p class="event-preview">Catch the celebration vibe with a quick preview</p>

            <!-- Video player (view mode) -->
            <q-video
              v-if="sasToken && !isEditing"
              :src="getEventvideo(event?.eventPreview)"
              class="event-video"
            ></q-video>

            <!-- Upload video input (edit mode) -->
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
    </div>

    <!-- Tabs for Memories and Guests -->
    <div class="event-section">
      <!-- Tabs: Always visible -->
      <q-tabs
        v-model="activeTab"
        dense
        class="text-black"
        active-color="black"
        indicator-color="primary"
      >
        <q-tab name="memories" label="Memories" />
        <q-tab name="guests" label="Our guests" />
      </q-tabs>

      <!-- Tab panels -->
      <q-tab-panels v-model="activeTab" animated class="text-dark text-center">
        <!-- Guests Panel -->
        <q-tab-panel name="guests" class="guests-panel">
          <!-- Button to invite guests (creator only) -->
          <div class="row justify-end q-mt-sm">
            <q-btn
              v-if="canEditEvent"
              dense
              color="primary"
              icon="link"
              label="invite guests"
              @click="onGenerateInvite"
            />
          </div>

          <!-- Component that shows the guest list -->
          <GuestListcompo :event-id="eventId" :grouped="false" />

          <!-- Link to full guest page -->
          <q-btn
            flat
            label="View all guests"
            class="View-more-btn"
            :to="`/event/${event?.eventId}/guests`"
            style="text-transform: none"
          />
        </q-tab-panel>

        <!-- Memories Panel -->
        <q-tab-panel name="memories" class="memories-panel">
          <!-- Floating button to open upload dialog -->
          <q-btn fab color="primary" icon="add" class="upload-fab" @click="triggerUpload" />

          <!-- Masonry layout for displaying uploaded memories -->
          <div class="masonry">
            <div class="masonry-item" v-for="item in memoryList" :key="item.id">
              <!-- If the item is a video -->
              <div v-if="/\.(mp4|webm|mov)(\?|$)/i.test(item.url)" style="position: relative">
                <video
                  :src="item.url"
                  controls
                  class="rounded-borders clickable"
                  style="width: 100%; border-radius: 12px; object-fit: cover"
                  @click="openPreview(item.url)"
                ></video>

                <!-- Video description -->
                <div
                  class="text-white"
                  style="
                    position: absolute;
                    bottom: 8px;
                    left: 8px;
                    background-color: rgba(0, 0, 0, 0.4);
                    padding: 4px 8px;
                    border-radius: 4px;
                    font-size: 14px;
                  "
                >
                  {{ item.description }}
                </div>

                <!-- Video likes -->
                <div
                  style="
                    position: absolute;
                    bottom: 8px;
                    right: 8px;
                    display: flex;
                    align-items: center;
                    background-color: rgba(0, 0, 0, 0.4);
                    padding: 4px 8px;
                    border-radius: 4px;
                  "
                >
                  <q-icon name="favorite" color="white" size="16px" />
                  <span class="q-ml-xs text-white">{{ item.likes }}</span>
                </div>
              </div>

              <!-- If the item is an image -->
              <q-img
                v-else
                :src="item.url"
                class="rounded-borders clickable"
                style="width: 100%; border-radius: 12px"
                @click="openPreview(item.url)"
              >
                <div
                  class="absolute-bottom text-white photo-description"
                  style="background-color: transparent"
                >
                  <div
                    style="
                      position: absolute;
                      left: 8px;
                      bottom: 8px;
                      display: flex;
                      align-items: center;
                    "
                  >
                    {{ item.description }}
                  </div>
                  <div
                    style="
                      position: absolute;
                      right: 8px;
                      bottom: 8px;
                      display: flex;
                      align-items: center;
                    "
                  >
                    <q-icon name="favorite" color="white" size="16px" />
                    <span class="q-ml-xs">{{ item.likes }}</span>
                  </div>
                </div>
              </q-img>
            </div>
          </div>
        </q-tab-panel>
      </q-tab-panels>

      <!-- Dialog for invite link -->
      <q-dialog v-model="showInviteDialog">
        <q-card style="min-width: 350px; max-width: 500px">
          <q-card-section>
            <div class="text-h6">Invite Link</div>
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
            <q-btn flat label="Close" color="negative" v-close-popup />
          </q-card-actions>
        </q-card>
      </q-dialog>

      <!-- Dialog for uploading memories -->
      <q-dialog v-model="showUploadDialog">
        <q-card style="min-width: 300px; max-width: 500px">
          <q-card-section>
            <div class="text-h6">Upload a Memory</div>
            <input type="file" accept="image/*,video/*" @change="onFileSelected" class="q-mt-sm" />
            <q-input
              v-model="newDescription"
              label="Description"
              type="textarea"
              filled
              class="q-mt-md"
            />
          </q-card-section>
          <q-card-actions align="right" class="justify-between">
            <q-btn flat label="Cancel" color="primary" v-close-popup />
            <q-btn flat label="Upload" color="primary" @click="uploadMemory" />
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

// Extract the event ID from route params
const eventId = computed(() => route.params.id as string)

// Reactive reference to the event from the store
const event = computed(() => userEvent.event)

// Get current user's ID from auth store
const currentUserId = computed(() => authStore.user?.userId)

// Check if the user is allowed to edit this event
const canEditEvent = computed(() => currentUserId.value === event.value?.createdBy)

const isEditing = ref(false)
const editedEvent = ref({ ...event.value })
const sasToken = ref('')
const isDescriptionExpanded = ref(false)
const selectedImageFile = ref<File | null>(null)
const selectedVideoFile = ref<File | null>(null)
const activeTab = ref('')
const isDetailsCollapsed = ref(true)
let map: L.Map | null = null
const showUploadDialog = ref(false)
const newDescription = ref('')
const selectedUploadFile = ref<File | null>(null)
const showImageDialog = ref(false)
const previewImageUrl = ref('')

// Open preview dialog for image or video
const openPreview = (url: string) => {
  previewImageUrl.value = url
  showImageDialog.value = true
  console.log('Previewing image:', url)
}

// Memory item interface for media gallery
interface MemoryItem {
  id: string
  url: string
  description: string
  likes: number
  createdAt?: string
}
const memoryList = ref<MemoryItem[]>([])

// Max upload file size in MB
const MAX_FILE_SIZE_MB = 10
const triggerUpload = () => {
  showUploadDialog.value = true
}

// Show upload dialog
const onFileSelected = (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return

  const sizeInMB = file.size / (1024 * 1024)
  if (sizeInMB > MAX_FILE_SIZE_MB) {
    alert(`File too large. Max size is ${MAX_FILE_SIZE_MB}MB.`)
    return
  }

  selectedUploadFile.value = file
}

// Determine if the event has already started
const eventStarted = computed(() => {
  if (!event.value?.eventDate) return false
  return new Date() >= new Date(event.value.eventDate)
})

// Vue.js Lifecycle   created-hook: fetch event details and initialize map
//created - Access DOM elements, initialize libraries
//updated -hook: react to changes in reactive data
// Lifecycle hook: fetch event, photos, and location on mount
onMounted(async () => {
  await userEvent.fetchEventById(eventId.value)
  sasToken.value = await getReadSasToken()

  activeTab.value = eventStarted.value ? 'memories' : 'guests'
  const photos = await userEvent.getPhotobyEventId(eventId.value)
  memoryList.value = photos.map(
    (p): MemoryItem => ({
      id: p.id,
      url: `${p.imageUrl}${sasToken.value}`,
      description: p.imageDescription,
      likes: 0,
      createdAt: p.createdAt,
    }),
  )

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

// Upload selected image or video to Azure and add to memory list
const uploadMemory = async () => {
  if (!selectedUploadFile.value) {
    alert('Please select an image.')
    return
  }

  try {
    const imageUrl = await uploadToAzureBlob(selectedUploadFile.value)

    await userEvent.postPhoto(eventId.value, imageUrl, newDescription.value)

    memoryList.value.unshift({
      id: Date.now().toString(),
      url: `${imageUrl}${sasToken.value}`,
      description: newDescription.value,
      likes: 0,
      createdAt: new Date().toISOString(),
    })

    selectedUploadFile.value = null
    newDescription.value = ''
    showUploadDialog.value = false
  } catch (error) {
    console.error('Upload failed:', error)
    alert('Upload failed')
  }
}

// Update editedEvent when event changes
watch(event, (newEvent) => {
  if (!isEditing.value && newEvent) {
    const date = new Date(newEvent.eventDate)
    const formattedDate = date.toISOString().slice(0, 16).replace('T', ' ')
    editedEvent.value = { ...newEvent, eventDate: formattedDate }
  }
})

// Redraw map when detail section is expanded
watch(isDetailsCollapsed, (newVal) => {
  if (!newVal) {
    setTimeout(() => {
      if (map) {
        map.invalidateSize()
      }
    }, 300)
  }
})

// Generate an invite link and open the dialog
const onGenerateInvite = async () => {
  if (!event.value?.eventId) return

  const code = await userEvent.generateInviteLink(event.value.eventId)
  if (code) {
    inviteLink.value = `${window.location.origin}/#/invite/${code}`
    showInviteDialog.value = true
  }
}
// Toggle showing event details manually
const toggleDetailsManually = () => {
  isDetailsCollapsed.value = !isDetailsCollapsed.value
}
// Copy the invite link to clipboard
const copyInviteLink = async () => {
  if (!inviteLink.value) return
  await navigator.clipboard.writeText(inviteLink.value)
  alert('copied to clipboard')
}

// Reference and trigger for image input field
const imageInputRef = ref<HTMLInputElement | null>(null)

const triggerImageInput = () => {
  imageInputRef.value?.click()
}

// Handle selected image for preview
const onImageSelected = (e: Event) => {
  const target = e.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    selectedImageFile.value = file
    editedEvent.value.eventImage = file
  }
}

// Reference and trigger for video input field
const videoInputRef = ref<HTMLInputElement | null>(null)
const triggerVideoInput = () => {
  videoInputRef.value?.click()
}

// Handle selected video for preview
const onVideoSelected = (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (file) {
    selectedVideoFile.value = file
    editedEvent.value.eventPreview = file
  }
}

// Toggle edit mode
const toggleEdit = () => {
  isEditing.value = !isEditing.value
}

// Save the updated event to backend
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

// Load map with Leaflet using given coordinates
const loadMap = (latitude: number, longitude: number) => {
  if (map) {
    map.remove()
  }
  map = L.map('map').setView([latitude, longitude], 13)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '',
  }).addTo(map)

  L.marker([latitude, longitude]).addTo(map).openPopup().bindPopup('Event location')
}

// Get full image URL from either string or File
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

// Format date in DD/MM/YYYY
const formatDate = (isoString: string | undefined) => {
  if (!isoString) return ''
  const date = new Date(isoString)
  return date.toLocaleDateString('en-GB', { year: 'numeric', month: '2-digit', day: '2-digit' })
}

// Format time in HH:mm (24h)
const formatTime = (isoString: string | undefined) => {
  if (!isoString) return ''
  const date = new Date(isoString)
  return date.toLocaleTimeString('en-GB', {
    hour: '2-digit',
    minute: '2-digit',
    timeZone: 'UTC',
  })
}

// Calculate end time based on start time and duration
const formatEndTime = (isoString: string | undefined, duration: string | undefined) => {
  if (!isoString || !duration) return ''
  const durationFloat = parseFloat(duration)
  if (isNaN(durationFloat)) return ''
  const hours = Math.floor(durationFloat)
  const minutes = Math.round((durationFloat - hours) * 60)
  const date = new Date(isoString)
  date.setUTCHours(date.getUTCHours() + hours)
  date.setUTCMinutes(date.getUTCMinutes() + minutes)
  return date.toLocaleTimeString('en-GB', {
    hour: '2-digit',
    minute: '2-digit',
    timeZone: 'UTC',
  })
}

// Get full video URL from either string or File
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

// Toggle event description expansion
const toggleDescription = () => {
  isDescriptionExpanded.value = !isDescriptionExpanded.value

  if (isDescriptionExpanded.value) {
    isDetailsCollapsed.value = false
  } else {
    isDetailsCollapsed.value = true
  }
}

// Geocoding using OpenStreetMap's Nominatim API
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

.memories-panel {
  background-color: #f3f3f5;
}

.guests-panel {
  background-color: #f3f3f5;
}

.q-tab {
  flex: 1;
  min-width: 0;
  text-align: center;
}

.q-tabs__content {
  width: 100%;
}

.masonry {
  column-count: 2;
  column-gap: 12px;
}
.masonry-item {
  break-inside: avoid;
  margin-bottom: 12px;
}

.upload-fab {
  position: fixed;
  bottom: 10px;
  right: 20px;
  z-index: 999;
}

.clickable {
  cursor: pointer;
}
</style>
