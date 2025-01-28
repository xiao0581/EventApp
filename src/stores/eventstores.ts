import { defineStore } from 'pinia'

interface Guest {
  id: number
  name: string
  role: string
  avatar?: string
}

interface Memory {
  id: number
  type: string
  url: string
}

interface Event {
  id: number
  name: string
  date: string
  location: string
  discription: string
  startTime: string
  endTime: string
  image: string
  guests: Guest[]
  memories: Memory[]
}

export const useEventStore = defineStore('event', {
  state: () => ({
    events: [
      {
        id: 1,
        name: "Sarah's and John's Wedding",
        date: '2025-01-29',
        startTime: '9:00 AM',
        endTime: '11:00 PM',
        location: 'High Garden Hotel',
        image: 'src/assets/pic/wedding.jpg',
        discription:
          'Celebrate the union of this wonderful couple with an evening of love, joy, and unforgettable memories. Enjoy a beautiful ceremony, delightful reception, and heartfelt moments as we honor their special day. We look forward to sharing this magical occasion with you!',
        guests: [
          {
            id: 1,
            name: 'Evelyn Carter',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/avatar1.jpg',
          },
          {
            id: 2,
            name: 'Liam Hayes',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/avatar2.jpg',
          },
          {
            id: 3,
            name: 'Liam Hayes',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/avatar2.jpg',
          },
          {
            id: 4,
            name: 'Liam Hayes',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/avatar2.jpg',
          },
          {
            id: 5,
            name: 'Liam Hayes',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/avatar2.jpg',
          },
          {
            id: 6,
            name: 'Evelyn Carter',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/avatar1.jpg',
          },
        ],
        memories: [
          { id: 1, type: 'image', url: '/assets/images/memory1.jpg' },
          { id: 2, type: 'image', url: '/assets/images/memory2.jpg' },
        ],
      },
      {
        id: 2,
        name: 'TeamLab’s Team Building',
        date: '2024-05-17',
        location: 'Celebration Haven',
        image: '/assets/images/team-building.jpg',
        guests: [
          { id: 3, name: 'John Smith', role: 'Team Leader', avatar: '/assets/images/guest3.jpg' },
          {
            id: 4,
            name: 'Alice Johnson',
            role: 'Team Member',
            avatar: '/assets/pic/avatar1.jpg',
          },
        ],
        memories: [
          { id: 3, type: 'image', url: '/assets/images/memory3.jpg' },
          { id: 4, type: 'video', url: '/assets/videos/memory4.mp4' },
        ],
      },
    ] as Event[],
  }),
  actions: {
    getEventById(id: number): Event | undefined {
      return this.events.find((event) => event.id === id)
    },
  },
})
