<template>
  <q-page padding>
    <h5 class="calendarTitle">Event Calendar</h5>
    <q-card class="calendar-card">
      <q-card-section>
        <div class="calendar">
          <div class="month-header text-h6 q-mb-md">
            <button class="q-btn q-btn--flat q-btn--dense" @click="previousMonth">←</button>
            <span>{{ monthNames[month] }} {{ year }}</span>
            <button class="q-btn q-btn--flat q-btn--dense" @click="nextMonth">→</button>
          </div>

          <div class="weekdays row justify-between text-subtitle2 q-mb-sm">
            <div v-for="(day, index) in weekDays" :key="index" class="weekday col text-center">
              {{ day }}
            </div>
          </div>

          <div class="days-grid row wrap justify-between">
            <div class="day col q-px-sm q-py-xs" v-for="(day, index) in daysInMonth" :key="index">
              <button
                class="q-btn q-btn--flat q-btn--dense flex flex-center"
                :class="{ selected: selectedDate === day, event: hasEvent(day) }"
                @click="selectDate(day)"
              >
                {{ day }}
              </button>
            </div>
          </div>
        </div>
      </q-card-section>
    </q-card>

    <div>
      <h6>Events this day</h6>
      <q-card-section>
        <div v-if="selectedDate">
          <div><strong>Selected Date:</strong> {{ formatSelectedDate }}</div>
          <div v-if="filteredEvents.length > 0">
            <strong>Events:</strong>
            <ul>
              <li v-for="(event, index) in filteredEvents" :key="index">
                {{ event.name }}
              </li>
            </ul>
          </div>
          <div v-else>No events for the selected date.</div>
        </div>
        <div v-else>Please select a date to see events.</div>
      </q-card-section>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

interface Event {
  name: string
  date: string
}

const year = ref<number>(2024)
const month = ref<number>(4)
const selectedDate = ref<number | null>(null)
const events = ref<Event[]>([
  { name: 'Team Meeting', date: '2024-05-15' },
  { name: 'Project Deadline', date: '2024-05-17' },
  { name: 'Gym Session', date: '2024-05-20' },
])

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

const formatSelectedDate = computed((): string | null => {
  if (!selectedDate.value) return null
  return `${year.value}-${String(month.value + 1).padStart(2, '0')}-${String(
    selectedDate.value,
  ).padStart(2, '0')}`
})

const filteredEvents = computed((): Event[] =>
  events.value.filter((event) => event.date === formatSelectedDate.value),
)

const hasEvent = (day: number): boolean => {
  return events.value.some((event) => event.date === formatDate(day))
}

const formatDate = (day: number): string => {
  return `${year.value}-${String(month.value + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`
}

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
  font-family: 'Roboto', sans-serif;
}

.calendar-title {
  text-align: center;
  font-size: clamp(16px, 3vw, 24px);
  margin-bottom: 1rem;
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
  gap: 0;
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
  font-size: clamp(12px, 2vw, 16px);
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
}

.day button:hover {
  background: #f0f0f0;
}

.day button.selected {
  background: #6c63ff;
  color: white;
  font-weight: bold;
}

.day button.event::after {
  content: '';
  position: absolute;
  bottom: 0px;
  left: 50%;
  transform: translateX(-50%);
  width: 6px;
  height: 6px;
  background-color: #6c63ff;
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
</style>
