import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useAuthStore } from 'src/stores/auth'
import { uploadToAzureBlob } from 'src/utils/azureUploader'
import axios from 'axios'
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
interface PublicUser {
  userId: string
  userName: string
  email: string
  profilePicture: string
  userRole: string
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

      const response = await axios.post(
        `${API_URL}Event`,
        {
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
        },
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        },
      )

      return response.data
    } catch (error) {
      if (axios.isAxiosError(error)) {
        const message = error.response?.data?.message || error.message
        throw new Error(message)
      } else if (error instanceof Error) {
        throw new Error(error.message)
      } else {
        throw new Error('Failed to create event')
      }
    }
  }

  const getEventsByuser = async () => {
    if (!userId) {
      console.error('User ID not found')
      return
    }

    try {
      const response = await axios.get(`${API_URL}event/byguest/${userId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })

      userEvents.value = response.data
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Axios error fetching events:', error.response?.data || error.message)
      } else {
        console.error('Unexpected error fetching events:', error)
      }
    }
  }

  const fetchEventById = async (eventId: string) => {
    const existingEvent = userEvents.value.find((event) => event.eventId === eventId)
    if (existingEvent) {
      event.value = existingEvent
      return existingEvent
    }

    try {
      const response = await axios.get<Event>(`${API_URL}Event/${eventId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })

      const fetchedEvent = response.data
      event.value = fetchedEvent
      userEvents.value.push(fetchedEvent)

      return fetchedEvent
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Axios error:', error.response?.data || error.message)
      } else {
        console.error('Unexpected error:', error)
      }
      return null
    }
  }

  const updateEvent = async (eventId: string, updatedEvent: Partial<Event>) => {
    try {
      let imageUrl = typeof updatedEvent.eventImage === 'string' ? updatedEvent.eventImage : ''
      let previewUrl =
        typeof updatedEvent.eventPreview === 'string' ? updatedEvent.eventPreview : ''

      if (updatedEvent.eventImage instanceof File) {
        imageUrl = await uploadToAzureBlob(updatedEvent.eventImage)
      }

      if (updatedEvent.eventPreview instanceof File) {
        previewUrl = await uploadToAzureBlob(updatedEvent.eventPreview)
      }

      let formattedDate = ''
      if (updatedEvent.eventDate) {
        formattedDate = new Date(
          updatedEvent.eventDate.replace(' ', 'T') + ':00.000Z',
        ).toISOString()
      }

      const response = await axios.put<Event>(
        `${API_URL}Event/${eventId}`,
        {
          eventTitle: updatedEvent.eventTitle,
          eventDescription: updatedEvent.eventDescription,
          eventDate: formattedDate,
          duration: updatedEvent.duration,
          eventLocation: updatedEvent.eventLocation,
          eventImage: imageUrl,
          eventPreview: previewUrl,
          eventCategory: updatedEvent.eventCategory,
        },
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
        },
      )

      const updated = response.data
      event.value = updated

      const index = userEvents.value.findIndex((e) => e.eventId === updated.eventId)
      if (index !== -1) {
        userEvents.value[index] = updated
      }

      return updated
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Axios error:', error.response?.data || error.message)
        throw new Error(error.message)
      } else {
        throw new Error('Failed to update event')
      }
    }
  }

  const getGuestListByEventId = async (eventId: string) => {
    try {
      const response = await axios.get<string[]>(`${API_URL}GuestList/${eventId}/userids`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })

      const guestUserIds = response.data

      if (event.value && event.value.eventId === eventId) {
        event.value.guests = guestUserIds
      }

      const index = userEvents.value.findIndex((e) => e.eventId === eventId)
      if (index !== -1 && userEvents.value[index]) {
        userEvents.value[index].guests = guestUserIds
      }

      return guestUserIds
    } catch (error) {
      console.error('Failed to fetch guest list:', error)
      return []
    }
  }

  const getUserInfoById = async (userId: string): Promise<PublicUser | null> => {
    try {
      const response = await axios.get<PublicUser>(`${API_URL}user/id/${userId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })

      return response.data
    } catch (error) {
      console.error(`Failed to fetch user info for ID ${userId}:`, error)
      return null
    }
  }

  return {
    event,
    creation,
    getEventsByuser,
    fetchEventById,
    userEvents,
    updateEvent,
    getGuestListByEventId,
    getUserInfoById,
  }
})
