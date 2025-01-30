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
              <q-avatar size="50px" class="q-mr-sm">
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
          <q-avatar size="50px" class="q-mr-sm">
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
import { computed } from 'vue'

interface Guest {
  id: number
  name: string
  role: string
  avatar?: string
}

const props = defineProps<{ guests: Guest[]; grouped?: boolean }>()

const groupedGuests = computed(() => {
  if (!props.grouped) {
    return {}
  }

  const groups: Record<string, Guest[]> = {}

  props.guests.forEach((guest) => {
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
}
</style>
