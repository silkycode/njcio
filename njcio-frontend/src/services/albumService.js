const API_BASE_URL = 'http://192.168.1.223:5001'

export const albumService = {

  async saveAlbum(album, sessionId) {
    const response = await fetch(`${API_BASE_URL}/api/albums`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        id: album.id,
        coverImage: album.coverImage,
        artistName: album.artistName,
        albumName: album.albumName,
        year: album.year,
        country: album.country,
        styles: album.styles,
        sessionId: sessionId
      })
    })

    if (!response.ok) {
      throw new Error(`failed: ${response.status}`)
    }

    return response.text()
  },

  async removeAlbum(albumId, sessionId) {
    const response = await fetch(`${API_BASE_URL}/api/albums/${albumId}?sessionId=${sessionId}`, {
      method: 'DELETE'
    })

    if (!response.ok) {
      throw new Error(`failed: ${response.status}`)
    }

    return response.text()
  },

  async getAlbums(sessionId) {
    const response = await fetch (`${API_BASE_URL}/api/albums?sessionId=${sessionId}`)

    if (!response.ok) {
      throw new Error(`failed: ${response.status}`)
    }

    return response.text()
  }
}
