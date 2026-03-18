namespace njcio_api.Models;

public class Style
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public List<AlbumStyle>? AlbumStyles { get; set; }
}
