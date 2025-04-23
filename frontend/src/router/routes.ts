import type { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/home',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      {
        path: '/MainLoginView',
        component: () => import('src/pages/LoginPages/MainLoginView.vue'),
        meta: { hideFooter: true },
      },
      {
        path: '/MainLoginView2',
        component: () => import('src/pages/LoginPages/MainLoginView2.vue'),
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
      {
        path: '/home',
        component: () => import('src/pages/HomePages/HomeView.vue'),
        meta: { requiresAuth: true },
      },
      {
        path: '/calendar',
        component: () => import('src/pages/CalendarPages/CalendarView.vue'),
        meta: { requiresAuth: true },
      },
      {
        path: '/message',
        component: () => import('src/pages/MessagePages/MessageView.vue'),
        meta: { requiresAuth: true },
      },
      {
        path: '/notification',
        component: () => import('src/pages/NotificationPages/NotifictionView.vue'),
        meta: { requiresAuth: true },
      },
      {
        path: '/profile',
        component: () => import('src/pages/ProfilePages/ProfileView.vue'),
        meta: { requiresAuth: true },
      },

      {
        path: '/EventForm',
        name: 'EventForm',
        component: () => import('src/pages/HomePages/EventCreatPages/EventForm.vue'),
        meta: { hideFooter: true, requiresAuth: true },
      },

      {
        path: '/event/:id',
        name: 'EventDetail',
        component: () => import('src/pages/HomePages/EventDetailPages/EventDetail.vue'),
        meta: { hideFooter: true, requiresAuth: true },
      },

      {
        path: '/event/:id/guests',
        name: 'GuestsList',
        component: () => import('src/pages/HomePages/EventDetailPages/OurGuests.vue'),
        meta: { hideFooter: true, requiresAuth: true },
      },

      {
        path: '/invite/:inviteCode',
        name: 'InviteHandler',
        component: () => import('src/pages/HomePages/InvitePages/InviteHandler.vue'),
      },
    ],
  },
]

export default routes
