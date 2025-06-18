<template>
  <div class="q-pa-md flex flex-center">
    <q-spinner v-if="loading" size="50px" color="primary" />

    <q-dialog v-model="showDialog" persistent transition-show="jump-down" transition-hide="jump-up">
      <q-card style="max-width: 400px; width: 100%">
        <q-card-section class="q-pa-none">
          <q-img :src="eventImageUrl" :ratio="16 / 9" class="event-img" />
        </q-card-section>

        <q-card-section class="q-pt-sm q-px-md">
          <div class="text-h6">{{ eventInfo?.eventTitle }}</div>
          <div class="text-subtitle2 q-mt-xs">{{ eventInfo?.eventLocation }}</div>
          <div class="text-caption q-mt-xs">
            <q-icon name="schedule" class="q-mr-xs" />
            {{ formattedDate }} at {{ formattedTime }}
          </div>
        </q-card-section>

        <q-separator />

        <q-card-actions align="around" class="q-pb-md q-pt-sm">
          <q-btn color="positive" label="Accept" @click="handleAccept" />
          <q-btn color="negative" label="Decline" @click="handleDecline" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from 'src/stores/auth'
import { eventStores } from 'src/stores/eventstores'
import { getReadSasToken } from 'src/utils/azureUploader'
import type { Event } from 'src/stores/eventstores'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const eventStore = eventStores()

// Extract invite code from route parameters
const inviteCode = route.params.inviteCode as string
const loading = ref(true)
const showDialog = ref(false)
const eventInfo = ref<Event | null>(null)
const sasToken = ref<string>('')

// Compute the final event image URL with SAS token
const eventImageUrl = computed(() => {
  if (!eventInfo.value?.eventImage) return '/assets/pic/luca.png'

  if (typeof eventInfo.value.eventImage === 'string') {
    return `${eventInfo.value.eventImage}${sasToken.value}`
  }

  return '/assets/pic/luca.png' // fallback for File/null
})

// Lifecycle: triggered when component is mounted
onMounted(async () => {
  if (!authStore.user) {
    await router.replace({ path: '/MainLoginView', query: { redirect: route.fullPath } })
    return
  }

  try {
    // Accept the invitation using invite code
    const data = await eventStore.acceptInvite(inviteCode)
    if (!data) {
      await router.push('/home')
      return
    }

    // Store event data and fetch SAS token
    eventInfo.value = data.event
    sasToken.value = await getReadSasToken()
    showDialog.value = true
  } catch (err) {
    console.error('Invite accept error:', err)
    await router.push('/home')
  } finally {
    loading.value = false
  }
})

// Handle accept button click
const handleAccept = async () => {
  showDialog.value = false

  const e = eventInfo.value
  if (e && !eventStore.userEvents.find((ev) => ev.eventId === e.eventId)) {
    eventStore.userEvents.push(e)
  }

  await router.replace({ path: '/home', query: { invited: 'accepted' } })
}

// Handle decline button click
const handleDecline = async () => {
  await eventStore.declineInvite(inviteCode)
  showDialog.value = false
  await router.replace({ path: '/home', query: { invited: 'declined' } })
}

// Format date as YYYY-MM-DD

const formattedDate = computed(() =>
  eventInfo.value?.eventDate ? eventInfo.value.eventDate.split('T')[0] : '',
)

// Format time as HH:mm
const formattedTime = computed(() =>
  eventInfo.value?.eventDate ? eventInfo.value.eventDate.split('T')[1]?.slice(0, 5) : '',
)
</script>

<style scoped>
.event-img {
  border-top-left-radius: 12px;
  border-top-right-radius: 12px;
  object-fit: cover;
}
</style>
