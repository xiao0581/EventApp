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
  padding: 10px;
  border-radius: 20px;
  max-height: 350px;
  overflow-y: auto;
}

.invitation-header p {
  font-size: 20px;
}

.invitation-card {
  padding: 10px;
  margin-bottom: 8px;
  border-radius: 10px;
}

.invitation-content {
  display: flex;
  align-items: center;
  gap: 10px;
}

.invitation-image-container {
  flex: 1;
  margin-right: 10px;
}

.invitation-image {
  width: 100%;
  height: 100px;
  object-fit: cover;
  border-radius: 20px;
}

.invitation-details {
  flex: 2;
  margin-top: -30px;
}

.event-title {
  font-size: 20px;
  font-weight: bold;
  margin-bottom: 3px;
}

.invitation-details p {
  font-size: 12px;
  margin: 1px 0;
}

.invitation-actions {
  display: flex;
  gap: 5px;
  margin-top: 10px;
  justify-content: space-between;
}

.q-btn {
  flex: 1;
  padding: 6px 10px;
  font-size: 12px;
  min-width: 80px;
  border-radius: 20px;
}
</style>
