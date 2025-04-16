<template>
  <q-page padding>
    <h5 class="calendarTitle">Event Calendar</h5>

    <q-card class="calendar-card">
      <q-card-section>
        <div class="calendar">
          <!-- Month Header -->
          <div class="month-header text-h6 q-mb-md">
            <button class="q-btn q-btn--flat q-btn--dense" @click="previousMonth">←</button>
            <span>{{ monthNames[month] }} {{ year }}</span>
            <button class="q-btn q-btn--flat q-btn--dense" @click="nextMonth">→</button>
          </div>

          <!-- Weekday Row -->
          <div class="weekdays row justify-between text-subtitle2 q-mb-sm">
            <div v-for="(day, index) in weekDays" :key="index" class="weekday col text-center">
              {{ day }}
            </div>
          </div>

          <!-- Day Grid -->
          <div class="days-grid row wrap justify-between">
            <div class="day col q-px-sm q-py-xs" v-for="(day, index) in daysInMonth" :key="index">
              <button
                class="q-btn q-btn--flat q-btn--dense flex flex-center"
                :class="{
                  selected: selectedDate === day,
                  event: hasEvent(day),
                  today: formatDate(day) === todayDate,
                }"
                @click="selectDate(day)"
              >
                {{ day }}
              </button>
            </div>
          </div>
        </div>
      </q-card-section>
    </q-card>

    <!-- Event List Section -->
    <div v-if="filteredEvents.length > 0">
      <div class="calendar-events-list">
        <div
          class="event-card"
          v-for="event in filteredEvents"
          :key="event.id"
          @click="$router.push(`/event/${event.id}`)"
        >
          <div class="event-card-image">
            <q-img :src="event.image" alt="Event image" />
          </div>
          <div class="event-card-content">
            <h5 class="event-title">{{ event.name }}</h5>
            <p><q-icon name="schedule" /> {{ event.startTime }} | {{ event.date }}</p>
            <p><q-icon name="place" /> {{ event.location }}</p>
          </div>
        </div>
      </div>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { eventStores } from 'src/stores/eventstores'
import { getReadSasToken } from 'src/utils/azureUploader'

interface Event {
  id: string
  name: string
  date: string
  startTime: string
  image: string
  location: string
}

const eventStore = eventStores()

const events = ref<Event[]>([])

const year = ref<number>(new Date().getFullYear())
const month = ref<number>(new Date().getMonth())
const selectedDate = ref<number | null>(null)

const monthNames = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December',
]
const weekDays = ['M', 'T', 'W', 'T', 'F', 'S', 'S']

const daysInMonth = computed((): number[] => {
  const days = new Date(year.value, month.value + 1, 0).getDate()
  return Array.from({ length: days }, (_, i) => i + 1)
})

const formatDate = (day: number): string => {
  return `${year.value}-${String(month.value + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`
}

const formatSelectedDate = computed((): string | null => {
  if (!selectedDate.value) return null
  return formatDate(selectedDate.value)
})

const hasEvent = (day: number): boolean => {
  return events.value.some((event) => event.date === formatDate(day))
}

const filteredEvents = computed((): Event[] =>
  events.value.filter((event) => event.date === formatSelectedDate.value),
)

const selectDate = (day: number): void => {
  selectedDate.value = day
}

const previousMonth = (): void => {
  if (month.value === 0) {
    month.value = 11
    year.value--
  } else {
    month.value--
  }
  selectedDate.value = null
}

const nextMonth = (): void => {
  if (month.value === 11) {
    month.value = 0
    year.value++
  } else {
    month.value++
  }
  selectedDate.value = null
}
const today = new Date()
const todayDate = formatDate(today.getDate())
onMounted(async () => {
  await eventStore.getEventsByuser()

  const token = await getReadSasToken()

  events.value = eventStore.userEvents.map((e) => {
    const dateStr = typeof e.eventDate === 'string' ? e.eventDate : ''
    const [date, time] = dateStr.split('T')

    return {
      id: e.eventId,
      name: e.eventTitle,
      date: date || '',
      startTime: time?.slice(0, 5) || '',
      image: typeof e.eventImage === 'string' ? `${e.eventImage}?${token}` : 'default-event.jpg',
      location: e.eventLocation || 'No location',
    }
  })
})
</script>

<style scoped>
.calendarTitle {
  text-align: left;
  font-weight: bold;
}
.calendar {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.month-header {
  display: flex;
  justify-content: space-between;
  width: 100%;
  font-weight: bold;
}

.weekdays {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  width: 100%;
  text-align: center;
  font-weight: bold;
  color: #666;
}

.weekday {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 40px;
}

.days-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 5px;
  width: 100%;
}

.day button {
  width: 100%;
  aspect-ratio: 1;
  border: none;
  border-radius: 50%;
  background: none;
  cursor: pointer;
  transition: all 0.3s;
  position: relative;
}

.day button:hover {
  background: #f0f0f0;
}

.day button.selected {
  background: #6c3baa;
  color: white;
  font-weight: bold;
}

.day button.event::after {
  content: '';
  position: absolute;
  bottom: 4px;
  left: 50%;
  transform: translateX(-50%);
  width: 6px;
  height: 6px;
  background-color: #6c3baa;
  border-radius: 50%;
}
.day button.selected::after {
  background-color: white;
}
.calendar-card {
  max-width: 600px;
  margin: 0 auto;
  border-radius: 20px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.calendar-events-list {
  margin-top: 16px;
}

.event-card {
  display: flex;
  max-width: 600px;
  height: 145px;
  flex-direction: row;
  align-items: center;
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
  font-weight: bold;
  margin-bottom: 6px;
}
.day button.today {
  box-shadow: 0 0 0 2px #6c63ff inset;
  font-weight: bold;
}
</style>
