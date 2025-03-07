import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useAuthStore } from 'src/stores/auth'
import { uploadToAzureBlob } from 'src/utils/azureUploader'
const API_URL = import.meta.env.VITE_API_BASE_URL

interface Event {
  eventId: string
  eventTitle: string
  eventDescription: string
  eventDate: string
  duration: string
  createdAt: string
  expiredAt: string
  eventLocation: string
  eventImage: File | string | null
  eventPreview: File | string | null
  eventCategory: string
  createdBy: string
  guests: string[]
}

export const eventStores = defineStore('eventstore', () => {
  const event = ref<Event | null>(null)
  const authStore = useAuthStore()
  const token = authStore.user?.token
  const userId = authStore.user?.userId
  const userEvents = ref<Event[]>([])

  const creation = async (createEvents: {
    eventTitle: string
    eventDescription: string
    eventDate: string
    duration: string
    createdAt: string
    expiredAt: string
    eventLocation: string
    eventImage: File | null
    eventPreview: File | null
    eventCategory: string
  }) => {
    try {
      let posterUrl = ''
      let previewUrl = ''

      if (createEvents.eventImage) {
        posterUrl = await uploadToAzureBlob(createEvents.eventImage)
      }

      if (createEvents.eventPreview) {
        previewUrl = await uploadToAzureBlob(createEvents.eventPreview)
      }
      const eventDateObj = new Date(createEvents.eventDate.replace(' ', 'T') + ':00.000Z')
      const formattedEventDate = eventDateObj.toISOString()
      const createdBy = ''

      const response = await fetch(`${API_URL}Event`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        body: JSON.stringify({
          eventTitle: createEvents.eventTitle,
          eventDescription: createEvents.eventDescription,
          eventDate: formattedEventDate,
          duration: createEvents.duration,
          createdAt: new Date().toISOString(),
          eventLocation: createEvents.eventLocation,
          eventImage: posterUrl,
          eventPreview: previewUrl,
          eventCategory: createEvents.eventCategory,
          createdBy: createdBy,
        }),
      })

      if (!response.ok) {
        throw new Error('Failed to create event')
      }
      return await response.json()
    } catch (error) {
      throw new Error(error instanceof Error ? error.message : 'Failed to create event')
    }
  }

  const getEventsByuser = async () => {
    if (!userId) {
      console.error('User ID not found')
      return
    }

    try {
      const response = await fetch(`${API_URL}event/byguest/${userId}`, {
        method: 'GET',
        headers: { Authorization: `Bearer ${token}` },
      })

      if (!response.ok) {
        throw new Error('Failed to fetch events')
      }

      userEvents.value = await response.json()
    } catch (error) {
      console.error('Error fetching events:', error)
    }
  }

  const fetchEventById = async (eventId: string) => {
    const existingEvent = userEvents.value.find((event) => event.eventId === eventId)
    if (existingEvent) {
      event.value = existingEvent

      return existingEvent
    }

    try {
      const response = await fetch(`${API_URL}Event/${eventId}`, {
        method: 'GET',
        headers: { Authorization: `Bearer ${token}` },
      })

      if (!response.ok) {
        throw new Error('Failed to fetch event details')
      }

      const fetchedEvent = await response.json()
      event.value = fetchedEvent

      userEvents.value.push(fetchedEvent)

      console.log('Event fetched from API:', fetchedEvent)
      return fetchedEvent
    } catch (error) {
      console.error('Error fetching event details:', error)
      return null
    }
  }

  return { event, creation, getEventsByuser, fetchEventById, userEvents }
})
