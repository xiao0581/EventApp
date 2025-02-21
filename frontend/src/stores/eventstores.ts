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
  description: string
  startTime: string
  endTime: string
  image: string
  guests: Guest[]
  memories: Memory[]
  isInvitation?: boolean
  sender?: string
}

export const useEventStore = defineStore('event', {
  state: () => ({
    events: [
      {
        id: 1,
        name: "Sarah's and John's Wedding",
        date: '2025-02-20',
        startTime: '9:00 AM',
        endTime: '11:00 PM',
        location: 'High Garden Hotel',
        image: 'src/assets/pic/wedding.jpg',
        description:
          'Celebrate the union of this wonderful couple with an evening of love, joy, and unforgettable memories. Enjoy a beautiful ceremony, delightful reception, and heartfelt moments as we honor their special day. We look forward to sharing this magical occasion with you!',
        guests: [
          {
            id: 1,
            name: 'Evelyn Carter',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/Evelyn.jpg',
          },
          {
            id: 2,
            name: 'Liam Hayes',
            role: 'groom’s family',
            avatar: 'src/assets/pic/liam.jpg',
          },
          {
            id: 3,
            name: 'Maria',
            role: 'friend',
            avatar: 'src/assets/pic/maria.jpg',
          },
          {
            id: 4,
            name: 'Alex',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/alex.jpg',
          },

          {
            id: 5,
            name: 'William',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/William.jpg',
          },
          {
            id: 6,
            name: 'Emily',
            role: 'Bride’s family',
            avatar: 'src/assets/pic/Emily.jpg',
          },
          {
            id: 7,
            name: 'Emma',
            role: 'groom’s family',
            avatar: 'src/assets/pic/Emma.jpg',
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
    invitations: [
      {
        id: 3,
        name: 'Luca Everett’s Birthday Party',
        date: '2024-08-16',
        startTime: '6:00 PM',
        endTime: '10:00 PM',
        location: 'Harran Hall',
        image: 'src/assets/pic/wedding.jpg',
        description: 'Join us for an evening of fun and celebration!',
        guests: [],
        memories: [],
        isInvitation: true,
        sender: 'Dylan Miller',
      },
    ] as Event[],
  }),
  actions: {
    getEventById(id: number): Event | undefined {
      return this.events.find((event) => event.id === id)
    },
    updateGuestRole(eventId: number, guestId: number, newRole: string) {
      const event = this.getEventById(eventId)
      if (event) {
        const guest = event.guests.find((g) => g.id === guestId)
        if (guest) {
          guest.role = newRole
        }
      }
    },
    receiveInvitation(invite: Event) {
      this.invitations.push(invite)
    },
    acceptInvitation(inviteId: number) {
      const acceptedInvite = this.invitations.find((invite) => invite.id === inviteId)

      if (acceptedInvite) {
        acceptedInvite.isInvitation = false
        this.events.push(acceptedInvite)
        this.invitations = this.invitations.filter((invite) => invite.id !== inviteId)
      }
    },

    declineInvitation(inviteId: number) {
      this.invitations = this.invitations.filter((invite) => invite.id !== inviteId)
    },

    hideInvitation(inviteId: number) {
      console.log(`Hiding invitation with ID: ${inviteId}`)
    },
    updateInvitationResponse(inviteId: number, going: boolean) {
      const inviteIndex = this.invitations.findIndex((invite) => invite.id === inviteId)

      if (inviteIndex !== -1) {
        const invite = this.invitations[inviteIndex]

        if (invite) {
          if (going) {
            invite.isInvitation = false
            this.events.push(invite)
          }
          this.invitations.splice(inviteIndex, 1)
        }
      }
    },
  },
})
