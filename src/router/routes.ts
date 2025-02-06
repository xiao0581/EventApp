import type { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      {
        path: '/',
        component: () => import('src/pages/LoginPages/MainLoginView.vue'),
        meta: { hideFooter: true },
      },
      {
        path: '/login',
        component: () => import('src/pages/LoginPages/LoginView.vue'),
        meta: { hideFooter: true },
      },
      {
        path: '/registers',
        component: () => import('src/pages/LoginPages/RegistersView.vue'),
        meta: { hideFooter: true },
      },
      { path: '/home', component: () => import('src/pages/HomePages/HomeView.vue') },
      { path: '/calendar', component: () => import('src/pages/CalendarPages/CalendarView.vue') },
      { path: '/message', component: () => import('src/pages/MessagePages/MessageView.vue') },
      {
        path: '/notification',
        component: () => import('src/pages/NotificationPages/NotifictionView.vue'),
      },

      {
        path: '/event/:id',
        name: 'EventDetail',
        component: () => import('src/pages/HomePages/EventDetailPages/EventDetail.vue'),
        meta: { hideFooter: true },
      },

      {
        path: '/event/:id/guests',
        name: 'GuestsList',
        component: () => import('src/pages/HomePages/EventDetailPages/OurGuests.vue'),
        meta: { hideFooter: true },
      },
    ],
  },
]

export default routes
