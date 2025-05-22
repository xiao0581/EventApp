<template>
  <q-page class="profile-page">
    <div class="banner">
      <q-img src="src/assets/pic/wedding.jpg" class="banner-img" />
      <q-avatar class="profile-avatar" size="100px">
        <q-img
          :src="avatarUrl"
          class="avatar-img"
          style="width: 100%; height: 100%; object-fit: cover; position: absolute; top: 0; left: 0"
        />
      </q-avatar>
    </div>

    <div class="user-info">
      <h5 class="user-name" @click="editNameDialog = true" style="cursor: pointer">
        {{ user.fullName }}
      </h5>
      <p class="user-email">{{ user.email }}</p>
      <p class="user-phone">{{ user.phone }}</p>

      <!--  <div class="stats-row">
        <div class="stat">
          <div class="stat-number">122</div>
          <div class="stat-label">Relations</div>
        </div>
        <div class="stat">
          <div class="stat-number">67</div>
          <div class="stat-label">Events</div>
        </div>
        <div class="stat">
          <div class="stat-number">37K</div>
          <div class="stat-label">Likes</div>
        </div>
      </div> -->

      <div class="action-buttons">
        <q-btn
          flat
          color="primary"
          icon="logout"
          label="Logout"
          @click="handleLogout"
          class="q-ml-sm"
        />
      </div>

      <q-tabs
        v-model="activeTab"
        dense
        class="text-black"
        active-color="black"
        indicator-color="primary"
      >
        <q-tab name="memories" label="My Memories" />
        <q-tab name="albums" label="My albums" />
      </q-tabs>
      <q-tab-panels v-model="activeTab" animated class="text-dark text-center">
        <q-tab-panel name="albums" class="albums-panel bg-grey-2"> </q-tab-panel>

        <q-tab-panel name="memories" class="memories-panel">
          <div class="masonry">
            <div class="masonry-item" v-for="item in memoryList" :key="item.id">
              <div v-if="/\\.(mp4|webm|mov)(\\?|$)/i.test(item.url)" style="position: relative">
                <video
                  :src="item.url"
                  controls
                  class="rounded-borders"
                  style="width: 100%; border-radius: 12px; object-fit: cover"
                ></video>
                <div
                  class="text-white"
                  style="
                    position: absolute;
                    bottom: 8px;
                    left: 8px;
                    background-color: rgba(0, 0, 0, 0.4);
                    padding: 4px 8px;
                    border-radius: 4px;
                    font-size: 14px;
                  "
                >
                  {{ item.description }}
                </div>
                <div
                  style="
                    position: absolute;
                    bottom: 8px;
                    right: 8px;
                    display: flex;
                    align-items: center;
                    background-color: rgba(0, 0, 0, 0.4);
                    padding: 4px 8px;
                    border-radius: 4px;
                  "
                >
                  <q-icon name="favorite" color="white" size="16px" />
                  <span class="q-ml-xs text-white">{{ item.likes }}</span>
                </div>
              </div>

              <q-img
                v-else
                :src="item.url"
                class="rounded-borders"
                style="width: 100%; border-radius: 12px"
              >
                <div
                  class="absolute-bottom text-white photo-description"
                  style="background-color: transparent"
                >
                  <div style="position: absolute; left: 8px; bottom: 8px">
                    {{ item.description }}
                  </div>
                  <div
                    style="
                      position: absolute;
                      right: 8px;
                      bottom: 8px;
                      display: flex;
                      align-items: center;
                    "
                  >
                    <q-icon name="favorite" color="white" size="16px" />
                    <span class="q-ml-xs">{{ item.likes }}</span>
                  </div>
                </div>
              </q-img>
            </div>
          </div>
        </q-tab-panel>
      </q-tab-panels>
    </div>
  </q-page>

  <q-dialog v-model="editAvatarDialog">
    <q-card style="min-width: 300px">
      <q-card-section>
        <div class="text-h6">Change Avatar</div>
      </q-card-section>

      <q-card-section>
        <q-file v-model="avatarFile" label="Choose New Avatar" accept="image/*" filled />
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat label="Cancel" v-close-popup />
        <q-btn flat label="Upload" color="primary" @click="updateUserAvatar" />
      </q-card-actions>
    </q-card>
  </q-dialog>

  <q-dialog v-model="editNameDialog">
    <q-card style="min-width: 300px">
      <q-card-section>
        <div class="text-h6">Change Name</div>
      </q-card-section>

      <q-card-section>
        <q-input v-model="editName" label="Full Name" />
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat label="Cancel" v-close-popup />
        <q-btn flat label="Save" color="primary" @click="updateUserName" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from 'src/stores/auth'
import { getReadSasToken, uploadToAzureBlob } from 'src/utils/azureUploader'
import { eventStores } from 'src/stores/eventstores'

const router = useRouter()
const authStore = useAuthStore()
const eventStore = eventStores()
const activeTab = ref('memories')
const user = ref({
  fullName: '',
  email: '',
  phone: '',
  about: '',
  avatarUrl: '',
})

const avatarUrl = ref('')
const editAvatarDialog = ref(false)
const editNameDialog = ref(false)
const avatarFile = ref<File | null>(null)
const editName = ref('')

const memoryList = ref<MemoryItem[]>([])

interface MemoryItem {
  id: string
  url: string
  description: string
  likes: number
  createdAt?: string
}

const updateUserAvatar = async () => {
  if (!avatarFile.value) return
  const newAvatarUrl = await uploadToAzureBlob(avatarFile.value)
  await eventStore.updateUser({ userName: user.value.fullName, profilePicture: newAvatarUrl })
  const sas = await getReadSasToken()
  user.value.avatarUrl = newAvatarUrl
  avatarUrl.value = `${newAvatarUrl}${sas}`
  editAvatarDialog.value = false
}

const updateUserName = async () => {
  await eventStore.updateUser({ userName: editName.value, profilePicture: user.value.avatarUrl })
  user.value.fullName = editName.value
  editNameDialog.value = false
}

const handleLogout = async () => {
  authStore.logout()
  await router.push('/MainLoginView')
}

onMounted(async () => {
  try {
    const currentUserId = authStore.user?.userId
    if (!currentUserId) throw new Error('User not authenticated')

    const userInfo = await eventStore.getUserInfoById(currentUserId)
    if (!userInfo) throw new Error('User data not found')

    const sas = await getReadSasToken()

    user.value = {
      fullName: userInfo.userName,
      email: userInfo.email || '',
      phone: '',
      about: '',
      avatarUrl: userInfo.profilePicture || '',
    }

    avatarUrl.value = userInfo.profilePicture
      ? `${userInfo.profilePicture}${sas}`
      : '/assets/pic/luca.png'
  } catch (error) {
    console.error('Error loading user info:', error)
  }

  const photos = await eventStore.getPhotobyUserId()
  const sas = await getReadSasToken()
  memoryList.value = photos.map((p) => ({
    id: p.id,
    url: `${p.imageUrl}${sas}`,
    description: p.imageDescription,
    likes: 0,
    createdAt: p.createdAt,
  }))
})
</script>

<style scoped>
.profile-page {
  background: #fff;
  min-height: 100vh;
}
.banner {
  position: relative;
  height: 200px;
}
.banner-img {
  height: 100%;
  object-fit: cover;
  width: 100%;
}
.profile-avatar {
  position: absolute;
  bottom: -40px;
  left: 50%;
  transform: translateX(-50%);
  border: 4px solid white;
  border-radius: 50%;
  overflow: hidden;
}
.avatar-img {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  object-fit: cover;
  border-radius: 50%;
}
.user-info {
  margin-top: 60px;
  padding: 16px;
  text-align: center;
}
.user-name {
  margin: 4px 0;
  font-size: 22px;
  font-weight: bold;
}
.user-email,
.user-phone {
  font-size: 14px;
  color: #666;
}
.about-section {
  margin: 16px 0;
  background: #f5f5f5;
  padding: 12px;
  border-radius: 10px;
  text-align: left;
}
.about-header {
  display: flex;
  align-items: center;
  font-weight: bold;
}
.about-text {
  margin-top: 6px;
  font-size: 14px;
}
.stats-row {
  display: flex;
  justify-content: space-around;
  margin: 12px 0;
}
.stat {
  text-align: center;
}
.stat-number {
  font-size: 18px;
  font-weight: bold;
}
.stat-label {
  font-size: 12px;
  color: #666;
}
.action-buttons {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  margin-top: 10px;
  gap: 8px;
}
.photo-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  justify-content: center;
  padding: 10px;
}
.photo-card {
  width: 120px;
  height: 120px;
  position: relative;
}
.photo-card img,
.photo-card .q-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.likes {
  position: absolute;
  bottom: 4px;
  right: 6px;
  background: rgba(255, 255, 255, 0.7);
  padding: 2px 6px;
  border-radius: 12px;
  font-size: 12px;
  display: flex;
  align-items: center;
  gap: 4px;
}
.masonry {
  column-count: 2;
  column-gap: 12px;
}
.masonry-item {
  break-inside: avoid;
  margin-bottom: 12px;
}
</style>
