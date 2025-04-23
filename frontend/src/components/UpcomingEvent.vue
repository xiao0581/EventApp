<template>
  <div class="event-card" v-for="event in events" :key="event.id" @click="goToEvent(event.id)">
    <div class="event-card-image">
      <q-img v-if="sasToken" :src="event.image" alt="Event image" />
    </div>
    <div class="event-card-content">
      <h5 class="event-title">{{ event.name }}</h5>
      <p><q-icon name="schedule" /> {{ event.startTime }} | {{ event.date }}</p>
      <p><q-icon name="place" /> {{ event.location }}</p>

      <div class="event-guests">
        <div class="guest-avatars">
          <q-avatar v-for="guest in event.guests?.slice(0, 5)" :key="guest.id" size="32px">
            <q-img :src="guest.avatar || '/assets/pic/luca.png'" alt="Guest avatar" />
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
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { eventStores } from 'src/stores/eventstores'
import { getReadSasToken } from 'src/utils/azureUploader'

const router = useRouter()
const eventStore = eventStores()

interface Guest {
  id: string
  name: string
  avatar: string
}

interface EventWithGuests {
  id: string
  name: string
  description: string
  date: string
  startTime: string
  image: string
  location: string
  category: string
  createdBy: string
  guests: Guest[]
}

const sasToken = ref<string>('')

const events = ref<EventWithGuests[]>([])

const today = new Date()
const dayAfterTomorrow = new Date()
dayAfterTomorrow.setDate(today.getDate() + 2)

const buildImageUrl = (url: string | undefined): string => {
  if (!url) return '/assets/pic/luca.png'
  return `${url}${sasToken.value}`
}

const goToEvent = async (id: string) => {
  try {
    await router.push(`/event/${id}`)
  } catch (error) {
    console.error('Navigation error:', error)
  }
}

onMounted(async () => {
  try {
    sasToken.value = await getReadSasToken()
    await eventStore.getEventsByuser()

    const upcomingEvents = eventStore.userEvents.filter((event) => {
      if (!event.eventDate) return false
      const eventDate = new Date(event.eventDate)
      return !isNaN(eventDate.getTime()) && eventDate >= today
    })

    const withGuests = await Promise.all(
      upcomingEvents.map(async (event) => {
        try {
          const guestIds = await eventStore.getGuestListByEventId(event.eventId)
          const uniqueIds = [...new Set(guestIds.filter((id) => id?.trim() !== ''))]
          const users = await Promise.all(uniqueIds.map((id) => eventStore.getUserInfoById(id)))
          const guests: Guest[] = users.filter(Boolean).map((u) => ({
            id: u!.userId,
            name: u!.userName,
            avatar: buildImageUrl(u!.profilePicture),
          }))

          return {
            id: event.eventId,
            name: event.eventTitle,
            description: event.eventDescription,
            date: event.eventDate?.split('T')[0] ?? '',
            startTime: event.eventDate?.split('T')[1]?.slice(0, 5) ?? '',
            image:
              typeof event.eventImage === 'string'
                ? `${event.eventImage}?${sasToken.value}`
                : 'default-event.jpg',
            location: event.eventLocation,
            category: event.eventCategory,
            createdBy: event.createdBy,
            guests,
          }
        } catch (err) {
          console.error(`Error loading guests for event ${event.eventId}:`, err)
          return {
            id: event.eventId,
            name: event.eventTitle,
            description: event.eventDescription,
            date: event.eventDate?.split('T')[0] ?? '',
            startTime: event.eventDate?.split('T')[1]?.slice(0, 5) ?? '',
            image:
              typeof event.eventImage === 'string'
                ? `${event.eventImage}?${sasToken.value}`
                : 'default-event.jpg',
            location: event.eventLocation,
            category: event.eventCategory,
            createdBy: event.createdBy,
            guests: [],
          }
        }
      }),
    )

    events.value = withGuests
  } catch (error) {
    console.error('Error loading events with guests:', error)
  }
})
</script>
<style scoped>
.event-card {
  display: flex;
  width: auto;
  height: 145px;
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
