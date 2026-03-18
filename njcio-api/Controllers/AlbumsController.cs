using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using njcio_api.Data;
using njcio_api.Models;

namespace njcio_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlbumsController : ControllerBase
{
	private readonly NjcioContext _context;

	public AlbumsController(NjcioContext context)
	{
		_context = context;
	}

	[HttpPost]
	public async Task<IActionResult> PostAlbum([FromBody] AlbumRequest request)
	{
		var album = await _context.Albums.FindAsync(request.Id);

		if (album == null)
		{
			album = new Album
			{
				Id = request.Id,
				CoverImage = request.CoverImage,
				ArtistName = request.ArtistName,
				AlbumName = request.AlbumName,
				Year = request.Year,
				Country = request.Country
			};
		_context.Albums.Add(album);
		}

		foreach (var styleName in request.Styles)
		{
			var style = await _context.Styles
				.FirstOrDefaultAsync(s => s.Name == styleName);

			if (style == null)
			{
				style = new Style { Name = styleName };
				_context.Styles.Add(style);
				await _context.SaveChangesAsync();
			}

			var albumStyle = await _context.AlbumStyles
				.FindAsync(request.Id, style.Id);

			if (albumStyle == null)
			{
				_context.AlbumStyles.Add(new AlbumStyle
				{
					AlbumId = request.Id,
					StyleId = style.Id
				});
			}
		}

		await _context.SaveChangesAsync();

		var session = await _context.Sessions
			.FirstOrDefaultAsync(s => s.SessionId == request.SessionId);

		if (session == null)
		{
			session = new Session
			{
				SessionId = request.SessionId
			};

			_context.Sessions.Add(session);
			await _context.SaveChangesAsync();
		}

		var albumSession = await _context.AlbumSessions
			.FindAsync(request.Id, session.Id);
	
		if (albumSession == null)
		{
			albumSession = new AlbumSession
			{
				AlbumId = request.Id,
				SessionId = session.Id
			};
			_context.AlbumSessions.Add(albumSession);
		}

		await _context.SaveChangesAsync();
		return Ok("Album saved!");
	}

	[HttpDelete("{albumId}")]
	public async Task<IActionResult> DeleteAlbum(int albumId, [FromQuery] string sessionId)
	{
		var session = await _context.Sessions
			.FirstOrDefaultAsync(s => s.SessionId == sessionId);

		if (session == null) return NotFound();

		var albumSession = await _context.AlbumSessions
			.FindAsync(albumId, session.Id);

		if (albumSession == null) return NotFound();

		_context.AlbumSessions.Remove(albumSession);
		await _context.SaveChangesAsync();

		return Ok("Album removed");
	}

	[HttpGet]
	public async Task<IActionResult> GetAlbums([FromQuery] string sessionId)
	{
		var session = await _context.Sessions
			.FirstOrDefaultAsync(s => s.SessionId == sessionId);

		if (session == null) return Ok(new List<Album>());

		var albums = await _context.AlbumSessions
			.Where(als => als.SessionId == session.Id)
			.Include(als => als.Album)
			.Select(als => als.Album)
			.ToListAsync();

		return Ok(albums);
	}
}

public record AlbumRequest(
		int Id,
		string? CoverImage,
		string ArtistName,
		string AlbumName,
		int? Year,
		string? Country,
		List<string> Styles,
		string SessionId
);
