<template>
  <v-app>
    <v-main>
      <v-container fluid class="d-flex align-center justify-center flex-column" style="height: 100vh;">
        <v-row v-if="album || loading" style="width: 80%" align="center">
          <v-col cols="9" class="d-flex flex-column align-center">
            <div v-if="album" class="w-100">
              <v-row>
                <v-col cols="6" class="pr-6">
                  <a :href="`https://www.discogs.com/release/${album.id}`" target="_blank">
                    <img :src="album.coverImage" :style="coverStyle"/>
                  </a>
                </v-col>
                <v-col cols="6" class="d-flex flex-column" :style="`
                  transition: opacity 0.4s ease;
                  opacity: ${albumVisible ? 1 : 0};
                `">
                  <p class="text-h4">{{ album.albumName }}</p>
                  <p class="text-h5 mt-2">{{ album.artistName }}</p>
                  <p class="text-h6 mt-4">{{ album.year }} | {{ album.country }}</p>
                  <div class="mt-4">
                    <v-chip 
                      v-for="style in album.styles" 
                      :key="style" class="mr-2 mb-2" size="small" 
                      style="box-shadow: 0 2px 6px rgba(0,0,0,0.2); cursor: pointer;"
                      :href="styleUrl(style)"
                      target="_blank"
                      tag="a"
                    >
                      {{ style }}
                    </v-chip>
                  </div>
                </v-col>
              </v-row>
              <div class="d-flex justify-center mt-12">
                <v-btn class="mx-2 font-weight-bold" @click="favoritesStore.addFavorite(album)">
                  listen later
                </v-btn>
                <v-btn class="mx-2 font-weight-bold" @click="getRandomAlbum" :loading="loading">
                  random!
                </v-btn>
              </div>
            </div>
          </v-col>
          <v-col cols="3" class="d-flex flex-column align-self-start" style="height: 80vh;">
            <div class="d-flex align-center justify-space-between w-100 mb-2">
              <span></span>
              <p class="text-h5 text-center">backlog</p>
              <v-btn size="x-small" @click="exportBacklog">export</v-btn>
            </div>
            <div class="backlog-scroll" style="overflow-y: auto; flex: 1;">
              <div v-for="favorite in favoritesStore.favorites" :key="favorite.id" style="width: 100%" class="mb-3">
                <div class="d-flex align-center justify-space-between w-100">
                  <div class="d-flex align-center">
                    <div class="d-flex flex-column justify-space-between mr-2" style="height: 100%;">
                      <v-icon size="small" color="deep-orange" class="mr-2" style="cursor: pointer;" @click="selectAlbum(favorite)">mdi-cassette</v-icon>
                      <v-icon size="small" color="red" style="cursor: pointer;" @click="favoritesStore.removeFavorite(favorite)">mdi-close</v-icon>
                    </div>
                    <div>
                      <p class="text-body-1 font-weight-bold mb-0">{{ favorite.albumName }}</p>
                      <p class="text-body-2 mb-0">{{ favorite.artistName }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </v-col>
        </v-row>
        <v-btn v-else @click="getRandomAlbum">
          random!
        </v-btn>
      </v-container>
    </v-main>
    <v-snackbar v-model="snackbar" :timeout="3000">
      {{ snackbarMessage }}
    </v-snackbar>
  </v-app>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useFavoritesStore } from './stores/favorites.js'

const coverStyle = computed(() => ({
    width: '100%',
    objectFit: 'contain',
    boxShadow: '0 20px 60px rgba(0,0,0,0.8)',
    transform: `rotate(${rotation.value}deg) scale(${coverVisible.value ? 1 : 0.92})`,
    transition: 'transform 0.6s cubic-bezier(0.34, 1.4, 0.64, 1), opacity 0.6s ease',
    opacity: coverVisible.value ? 1 : 0,
    maskImage: 'linear-gradient(to bottom right, rgba(0,0,0,1) 60%, rgba(0,0,0,0.8) 100%)',
    WebkitMaskImage: 'linear-gradient(to bottom right, rgba(0,0,0,1) 60%, rgba(0,0,0,0.8) 100%)',
}))

const favoritesStore = useFavoritesStore()

onMounted(() => {
  favoritesStore.initSession()
  favoritesStore.fetchFavorites()
})

const albumVisible = ref(false)
const coverVisible = ref(false)
const album = ref(null)
const loading = ref(false)
const rotation = ref(0)
const snackbar = ref(false)
const snackbarMessage = ref('')

function pickRandom(results) {
	const withCovers = results.filter(r => r.coverImage != '')
	const pool = withCovers.length > 0 ? withCovers : results
	return pool[Math.floor(Math.random() * pool.length)]
}	

async function selectAlbum(favorite) {
    albumVisible.value = false
    coverVisible.value = false

    await new Promise(resolve => setTimeout(resolve, 50))
    album.value = favorite
    albumVisible.value = true

    await new Promise(resolve => setTimeout(resolve, 150))
    coverVisible.value = true
    rotation.value = (Math.random() * 4 - 2).toFixed(2)
}

async function getRandomAlbum() {
	loading.value = true
	
	try {
		const randomPage = Math.floor(Math.random() * 500) + 1
		const response = await fetch(
			`https://api.discogs.com/database/search?genre=electronic&format=album&per_page=10&page=${randomPage}`,
			{ headers: { 'User-Agent': 'njc.io/1.0' } }
  		)

		if (!response.ok) throw new Error(`HTTP error: ${response.status}`)
		const data = await response.json()
		const picked = pickRandom(data.results)

		const releaseResponse = await fetch(
    			picked.resource_url,
    			{ headers: { 'User-Agent': 'njc.io/1.0' } }
  		)

  		const releaseData = await releaseResponse.json()
  		const [artistName, albumName] = picked.title.split(' - ')

      await new Promise((resolve) => {
        const img = new Image()
        img.onload = resolve
        img.src = releaseData.images?.[0]?.uri || 'https://placehold.co/300x300?text=No+Cover'
      })

  		album.value = {
			id: picked.id,
    			coverImage: releaseData.images?.[0]?.uri || 'https://placehold.co/300x300?text=No+Cover',
    			artistName,
    			albumName,
    			year: picked.year,
    			country: picked.country,
    			styles: picked.style || []
  		}
      albumVisible.value = false
      coverVisible.value = false
      loading.value = false

      await new Promise(resolve => setTimeout(resolve, 50))
      albumVisible.value = true

      await new Promise(resolve => setTimeout(resolve, 150))
      coverVisible.value = true
      rotation.value = (Math.random() * 4 - 2).toFixed(2)

	} catch (error) {
    console.log(error)
		snackbarMessage.value = error.name === 'TypeError'
		? 'Too many requests - wait a moment and try again (API limited)'
		: 'Something else broke, refresh or try again later'
		snackbar.value = true
	} finally {
	  loading.value = false
	}
}

function exportBacklog() {
  const timestamp = new Date().toLocaleString()
  const text = `backlog export - ${timestamp}\n\n` + favoritesStore.favorites
    .map(a=> `${a.albumName} - ${a.artistName}`)
    .join('\n')

  const blob = new Blob([text], { type: 'text/plain' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = 'backlog.txt'
  a.click()
  URL.revokeObjectUrl(url)
}

function styleUrl(style) {
  return `https://www.discogs.com/style/${style.toLowerCase().replace(/ /g, '+')}`
}
</script>
