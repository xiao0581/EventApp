import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useAuthStore } from 'src/stores/auth'
import { uploadToAzureBlob } from 'src/utils/azureUploader'

interface Event {
  eventTitle: string
  description: string
  eventDate: string
  duration: string
  createdAt: string
  expiredAt: string
  eventLocation: string
  eventImage: File | null
  eventPreview: File | null
  eventCategory: string
}

export const eventCreation = defineStore('eventCreation', () => {
  const event = ref<Event | null>(null)

  const creation = async (createEvents: {
    eventTitle: string
    description: string
    eventDate: string
    duration: string
    createdAt: string
    expiredAt: string
    eventLocation: string
    eventImage: File | null
    eventPreview: File | null
    eventCategory: string
  }) => {
    const authStore = useAuthStore()
    const token = authStore.user?.token

    try {
      let posterUrl = ''
      let previewUrl = ''

      if (createEvents.eventImage) {
        posterUrl = await uploadToAzureBlob(createEvents.eventImage)
      }
      if (createEvents.eventPreview) {
        previewUrl = await uploadToAzureBlob(createEvents.eventPreview)
      }

      const response = await fetch('http://localhost:5102/api/Event', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        body: JSON.stringify({
          eventTitle: createEvents.eventTitle,
          description: createEvents.description,
          eventDate: createEvents.eventDate,
          duration: createEvents.duration,
          createdAt: createEvents.createdAt,
          expiredAt: createEvents.expiredAt,
          eventLocation: createEvents.eventLocation,
          eventImage: posterUrl,
          eventPreview: previewUrl,
          eventCategory: createEvents.eventCategory,
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

  return { event, creation }
})
