<template>
  <q-card class="invitations-section">
    <div class="invitation-card" v-for="invite in invitations" :key="invite.id">
      <div class="invitation-header">
        <p>{{ invite.sender }} invites you to</p>
      </div>
      <div class="invitation-content">
        <div class="invitation-image-container">
          <q-img :src="invite.image" alt="Invitation image" class="invitation-image" />
        </div>

        <div class="invitation-details">
          <h5 class="event-title">{{ invite.name }}</h5>
          <p><q-icon name="schedule" /> {{ invite.startTime }} | {{ invite.date }}</p>
          <p><q-icon name="place" /> {{ invite.location }}</p>
        </div>
      </div>

      <div class="invitation-actions">
        <q-btn color="primary" label="I'm going" @click="respondToInvite(invite.id, true)" />
        <q-btn color="negative" label="Can't go" @click="respondToInvite(invite.id, false)" />
        <q-btn color="grey" outline label="Hide" @click="hideInvite(invite.id)" />
      </div>
    </div>
  </q-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useEventStore } from 'src/stores/eventstores'

const eventStore = useEventStore()

const invitations = computed(() => eventStore.invitations || [])

const respondToInvite = (id: number, going: boolean) => {
  console.log(`User response to invitation ${id}:`, going ? 'Going' : 'Not going')
  eventStore.updateInvitationResponse(id, going)
}

const hideInvite = (id: number) => {
  console.log(`Hiding invitation ${id}`)
  eventStore.hideInvitation(id)
}
</script>

<style scoped>
.invitations-section {
  margin: 20px;
  border-radius: 30px;
}

h2 {
  font-size: 1.2rem;
  margin-bottom: 10px;
}

.invitation-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  margin-bottom: 20px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: transform 0.2s ease-in-out;
  padding: 16px;
}

.invitation-card:hover {
  transform: translateY(-5px);
}

.invitation-content {
  display: flex;
  align-items: center;
}

.invitation-image-container {
  flex: 1;
  margin-right: 20px;
}

.invitation-image {
  width: 100%;
  height: 150px;
  object-fit: cover;
  border-radius: 8px;
}

.invitation-details {
  flex: 2;
}

.event-title {
  font-size: 18px;
  font-weight: bold;
  margin-bottom: 5px;
}

.invitation-actions {
  display: flex;
  gap: 10px;
  margin-top: 10px;
}
</style>
