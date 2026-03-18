namespace njcio_api.Models;

public class Session
{
	public int Id { get; set; }
	public required string SessionId { get; set; }
	public List<AlbumSession>? AlbumSessions { get; set; }
}
