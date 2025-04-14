<template>
  <q-list>
    <template v-if="grouped">
      <q-expansion-item
        v-for="(group, role) in groupedGuests"
        :key="role"
        :label="role"
        default-opened
      >
        <q-list>
          <q-card v-for="guest in group" :key="guest.id" class="q-mb-md q-pa-sm">
            <q-card-section class="row items-center">
              <q-avatar size="80px" class="q-mr-sm">
                <q-img :src="guest.avatar || '/assets/images/default-avatar.png'" />
              </q-avatar>

              <div class="col text-left">
                <div class="text-weight-medium">{{ guest.name }}</div>
              </div>

              <q-btn round icon="sym_o_chat_bubble" @click="sendMessage(guest)" />
            </q-card-section>
          </q-card>
        </q-list>
      </q-expansion-item>
    </template>

    <template v-else>
      <q-card v-for="guest in guests" :key="guest.id" class="q-mb-md q-pa-sm">
        <q-card-section class="row items-center">
          <q-avatar size="70px" class="q-mr-sm">
            <q-img :src="guest.avatar || '/assets/images/default-avatar.png'" />
          </q-avatar>

          <div class="col text-left">
            <div class="text-weight-medium">{{ guest.name }}</div>
          </div>

          <q-btn round icon="sym_o_chat_bubble" @click="sendMessage(guest)" />
        </q-card-section>
      </q-card>
    </template>
  </q-list>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { eventStores } from 'src/stores/eventstores'
import { getReadSasToken } from 'src/utils/azureUploader'

const sasToken = ref<string>('')

interface Guest {
  id: string
  name: string
  role: string
  avatar?: string
}

const props = defineProps<{
  eventId: string
  grouped?: boolean
}>()

const eventStore = eventStores()
const guests = ref<Guest[]>([])

onMounted(async () => {
  try {
    sasToken.value = await getReadSasToken()
    const guestIds = await eventStore.getGuestListByEventId(props.eventId)

    const validIds = guestIds.filter((id) => id && id.trim() !== '')

    const uniqueIds = [...new Set(validIds)]

    const users = await Promise.all(uniqueIds.map((id) => eventStore.getUserInfoById(id)))

    guests.value = users.filter(Boolean).map((u) => ({
      id: u!.userId,
      name: u!.userName,
      role: u!.userRole,
      avatar: buildImageUrl(u!.profilePicture),
    }))
  } catch (err) {
    console.error('Failed to load guests:', err)
  }
})

const buildImageUrl = (url: string | undefined): string => {
  if (!url) return '/assets/pic/luca.png'
  return `${url}${sasToken.value}`
}

const groupedGuests = computed(() => {
  if (!props.grouped) return {}

  const groups: Record<string, Guest[]> = {}

  guests.value.forEach((guest) => {
    const role = guest.role || 'Unknown'
    if (!groups[role]) {
      groups[role] = []
    }
    groups[role].push(guest)
  })

  return groups
})

const sendMessage = (guest: Guest) => {
  alert(`Send message to ${guest.name}`)
}
</script>

<style scoped>
.q-btn {
  color: #1e1e1e;
  background-color: #eaecff;
}
.q-card {
  border-radius: 20px;
  box-shadow: none;
  margin: 12px 12px;
  padding: 1px;
}
.text-weight-medium {
  margin-top: -10px;
  font-size: 18px;
  font-weight: bold;
}
.tag-button {
  font-size: 10px;
  padding: 2px 6px;
  height: 22px;
  min-width: 60px;
  border: 1px solid #98a4dd;
  color: #4a4a4a;
}
</style>
