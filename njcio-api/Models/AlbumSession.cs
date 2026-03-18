namespace njcio_api.Models;

public class AlbumSession
{
	public int AlbumId { get; set; }
	public Album? Album { get; set; }
	public int SessionId { get; set; }
	public Session? Session { get; set; }
}
