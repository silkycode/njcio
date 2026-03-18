import { defineStore } from 'pinia'
import { ref } from 'vue'
import { albumService } from '../services/albumService'

export const useFavoritesStore = defineStore('favorites', () => {
	const favorites = ref([])
	const sessionId = ref(null)
	
	function initSession() {
		let id = localStorage.getItem('sessionId')
		if (!id) {
			id = typeof crypto.randomUUID === 'function'
				? crypto.randomUUID()
				: Math.random().toString(36).substring(2) + Date.now().toString(36)
			localStorage.setItem('sessionId', id)
		}
		sessionId.value = id
	}

	async function addFavorite(album) {
		const exists = favorites.value.some(a => a.id === album.id)
		if (exists) return

		try {
			await albumService.saveAlbum(album, sessionId.value)
			favorites.value.push(album)
		} catch (error) {
			console.error('Failed to save:', error)
		}
	}

	async function removeFavorite(album) {
		try {			
			await albumService.removeAlbum(album.id, sessionId.value)
			favorites.value = favorites.value.filter(a => a.id != album.id)
		} catch (error) {
			console.error('Failed to remove:', error)
		}
	}

	async function fetchFavorites() {
		try {
			const response = await albumService.getAlbums(sessionId.value)
			const data = JSON.parse(response)
			favorites.value = data
		} catch (error) {
			console.error('Failed to fetch favorites:', error)
		}
	}

	return { favorites, sessionId, initSession, addFavorite, removeFavorite, fetchFavorites }
})

