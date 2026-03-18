/*
album.value = {
	id: picked.id,
	cover_image: releaseData.images?.[0]?.uri || 'https://placehold.co/300x300?text=No+Cover',
	artistName,
	albumName,
	year: picked.year,
	country: picked.country,
	styles: picked.style || []
}
*/

namespace njcio_api.Models;

public class Album
{
	public int Id { get; set; }
	public string? CoverImage { get; set; }
	public required string ArtistName { get; set; }
	public required string AlbumName { get; set; }
	public int? Year { get; set; }
	public string? Country { get; set; }
	public List<AlbumStyle>? AlbumStyles { get; set; }
	public List<AlbumSession>? AlbumSessions { get; set; }
}
