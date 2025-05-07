<template>
  <q-btn v-if="step === 1" flat class="back-btn" icon="arrow_back_ios" to="/home" />

  <q-page class="event-page">
    <div class="header">
      <div class="title">
        <p>Event details</p>
      </div>
    </div>

    <div class="content">
      <div v-if="step === 1">
        <div class="section-title">Create warm memories</div>

        <div class="upload-area">
          <q-file
            class="file-input"
            v-model="eventData.eventImage"
            accept="image/*"
            borderless
            @update:model-value="updatePosterPreview"
          />
          <div class="upload-content">
            <img v-if="posterPreviewUrl" :src="posterPreviewUrl" class="preview-image" />
            <q-btn v-else label="Add Cover" class="add-cover-button" unelevated />
          </div>
        </div>

        <div class="upload-area">
          <q-file
            class="file-input"
            v-model="eventData.eventPreview"
            accept="video/*"
            borderless
            @update:model-value="updatePreviewVideo"
          />
          <div class="upload-content">
            <video v-if="previewVideoUrl" class="preview-image" autoplay loop muted playsinline>
              <source :src="previewVideoUrl" type="video/mp4" />
              Your browser does not support the video tag.
            </video>
            <q-btn v-else label="Add Preview" class="add-cover-button" unelevated />
          </div>
        </div>

        <div class="input-label">Date</div>
        <div class="EventDate" style="max-width: 300px">
          <q-input filled v-model="eventData.eventDate">
            <template v-slot:prepend>
              <q-icon name="event" class="cursor-pointer" color="primary">
                <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                  <q-date v-model="eventData.eventDate" mask="YYYY-MM-DD HH:mm">
                    <div class="row items-center justify-end">
                      <q-btn v-close-popup label="Close" color="primary" flat />
                    </div>
                  </q-date>
                </q-popup-proxy>
              </q-icon>
            </template>

            <template v-slot:append>
              <q-icon name="access_time" class="cursor-pointer" color="primary">
                <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                  <q-time v-model="eventData.eventDate" mask="YYYY-MM-DD HH:mm" format24h>
                    <div class="row items-center justify-end">
                      <q-btn v-close-popup label="Close" color="primary" flat />
                    </div>
                  </q-time>
                </q-popup-proxy>
              </q-icon>
            </template>
          </q-input>
        </div>

        <div class="input-label">Title</div>
        <q-input
          v-model="eventData.eventTitle"
          label="please enter event title"
          filled
          class="q-mt-md"
        />

        <div class="input-label">Category</div>
        <q-input
          v-model="eventData.eventCategory"
          label="please enter category"
          filled
          class="q-mt-md"
        />
        <div class="input-label">start time</div>
        <q-input filled v-model="eventData.startTime">
          <template v-slot:append>
            <q-icon name="access_time" class="cursor-pointer" color="primary">
              <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                <q-time v-model="eventData.startTime" mask="HH:mm" format24h>
                  <div class="row items-center justify-end">
                    <q-btn v-close-popup label="Close" color="primary" flat />
                  </div>
                </q-time>
              </q-popup-proxy>
            </q-icon>
          </template>
        </q-input>

        <div class="input-label">End time</div>
        <q-input filled v-model="eventData.endTime">
          <template v-slot:append>
            <q-icon name="access_time" class="cursor-pointer" color="primary">
              <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                <q-time v-model="eventData.endTime" mask=" HH:mm" format24h>
                  <div class="row items-center justify-end">
                    <q-btn v-close-popup label="Close" color="primary" flat />
                  </div>
                </q-time>
              </q-popup-proxy>
            </q-icon>
          </template>
        </q-input>

        <div class="input-label">Location</div>
        <q-input
          v-model="eventData.eventLocation"
          label="please enter location"
          filled
          class="q-mt-md"
        />

        <div class="input-label">Description</div>
        <q-input
          v-model="eventData.eventDescription"
          label="please enter description"
          filled
          class="q-mt-md"
        />
      </div>

      <!-- Steps 2 and 3 can remain unchanged -->
      <div v-if="step === 2">
        <div class="section-title">Who will help create warm memories</div>
        <q-input v-model="hostEmail" label="Enter host email" filled class="q-mt-md" />
        <q-btn label="Add Host" color="primary" class="btn" @click="addHost" />
      </div>

      <div v-if="step === 3">
        <div class="section-title">Who will be part of your warm moments</div>
        <q-btn label="Upload guests list" outline class="btn" @click="uploadGuestList" />
        <q-btn label="Select from contact book" outline class="btn" />
        <q-btn label="Add manually" outline class="btn" @click="addGuestManually" />
      </div>
    </div>

    <div class="footer" :class="{ 'footer-right': step === 1, 'footer-default': step > 1 }">
      <q-btn v-if="step > 1" label="Back" outline @click="prevStep" />
      <q-btn
        v-if="step === 1"
        label="Save"
        color="primary"
        class="btn-large"
        @click="eventcreate"
      />
      <q-btn v-if="step > 1" label="Next" color="primary" :disable="step === 3" @click="nextStep" />
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { eventStores } from 'src/stores/eventstores'
import { Notify } from 'quasar'
const useEventStore = eventStores()
const eventData = reactive({
  eventImage: null as File | null,
  eventPreview: null as File | null,
  eventDate: '',
  eventTitle: '',
  startTime: '',
  endTime: '',
  duration: '',
  eventLocation: '',
  eventname: '',
  eventDescription: '',
  createdAt: '',
  expiredAt: '',
  eventCategory: '',
})
const router = useRouter()
const loading = ref<boolean>(false)
const hostEmail = ref<string>('')
const guests = ref<string[]>([])
const step = ref<number>(1)
const posterPreviewUrl = ref<string>('')
const previewVideoUrl = ref<string>('')

const updatePosterPreview = (file: File | null) => {
  if (file) {
    posterPreviewUrl.value = URL.createObjectURL(file)
  } else {
    posterPreviewUrl.value = ''
  }
}
const calculateDuration = (): number => {
  const [startHour = 0, startMinute = 0] = eventData.startTime.split(':').map(Number)
  const [endHour = 0, endMinute = 0] = eventData.endTime.split(':').map(Number)

  const startTotalMinutes = startHour * 60 + startMinute
  const endTotalMinutes = endHour * 60 + endMinute

  let durationMinutes = endTotalMinutes - startTotalMinutes

  if (durationMinutes < 0) {
    durationMinutes += 24 * 60
  }

  return durationMinutes / 60
}
const eventcreate = async (): Promise<void> => {
  loading.value = true
  try {
    const calulated = calculateDuration()
    const createdEvent = await useEventStore.creation({
      eventTitle: eventData.eventTitle,
      eventDescription: eventData.eventDescription,
      eventDate: eventData.eventDate,
      duration: calulated.toString(),
      createdAt: eventData.createdAt,
      expiredAt: eventData.eventDate,
      eventLocation: eventData.eventLocation,
      eventImage: eventData.eventImage,
      eventPreview: eventData.eventPreview,
      eventCategory: eventData.eventCategory,
    })
    Notify.create({
      type: 'positive',
      message: 'Create successful!',
    })

    await router.push(`/event/${createdEvent.eventId}`)
  } catch (error: unknown) {
    const message = (error as Error).message || 'An unknown error occurred.'
    Notify.create({
      type: 'negative',
      message,
    })
  } finally {
    loading.value = false
  }
}

const updatePreviewVideo = (file: File | null) => {
  if (file) {
    previewVideoUrl.value = URL.createObjectURL(file)
  } else {
    previewVideoUrl.value = ''
  }
}

const addHost = () => {
  if (hostEmail.value) {
    console.log('Added host:', hostEmail.value)
  }
}

const uploadGuestList = () => {
  console.log('Uploading guest list...')
}

const addGuestManually = () => {
  const guestName = prompt('Enter guest name:')
  if (guestName) {
    guests.value.push(guestName)
  }
}

const nextStep = () => {
  if (step.value < 3) step.value++
}

const prevStep = () => {
  if (step.value > 1) step.value--
}
</script>

<style scoped>
.event-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  background-color: #f9f9f9;
}

.header {
  width: 100%;
  text-align: center;
  padding: 10px 0;
}

.title {
  font-size: 1.5rem;
  font-weight: bold;
  color: #6c3baa;
}

.content {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  width: 100%;
  padding-top: 20px;
}

.section-title {
  font-size: 1.2rem;
  font-weight: bold;
  margin-bottom: 10px;
  color: #6c3baa;
}

.input-label {
  margin-top: 15px;
  font-size: 0.9rem;
  color: #6c3baa;
}

.upload-area {
  position: relative;
  width: 100%;
  height: 150px;
  border-radius: 20px;
  background: linear-gradient(135deg, #a77ce7, #c3a5ff);
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 20px;
}

.upload-content {
  position: relative;
  z-index: 1;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.add-cover-button {
  border-radius: 999px;
  background: linear-gradient(90deg, #6d28d9, #a78bfa);
  color: white;
  text-transform: none;
  padding: 10px 20px;
}

.file-input {
  position: absolute;
  width: 100%;
  height: 100%;
  z-index: 2;
  opacity: 0;
  cursor: pointer;
}

.preview-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  position: absolute;
  top: 0;
  left: 0;
  z-index: 0;
}

.EventDate {
  margin-top: 15px;
}

.footer {
  width: 100%;
  display: flex;
  padding: 20px;
}
.footer-right {
  justify-content: flex-end;
}
.footer-default {
  justify-content: space-between;
}
.btn-large {
  font-size: 1rem;
  width: 160px;
}
.btn {
  width: 100%;
  margin-top: 15px;
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
