using Microsoft.EntityFrameworkCore;
using njcio_api.Models;

namespace njcio_api.Data;

public class NjcioContext : DbContext
{
	public DbSet<Album> Albums { get; set; } = null!;
	public DbSet<AlbumStyle> AlbumStyles { get; set; } = null!;
	public DbSet<Style> Styles { get; set; } = null!;
	public DbSet<AlbumSession> AlbumSessions {get; set; } = null!;
	public DbSet<Session> Sessions {get; set; } = null!;

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Data Source=njcio.db");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) 
	{
		modelBuilder.Entity<Album>()
			.Property(a => a.Id)
			.ValueGeneratedNever();
		modelBuilder.Entity<AlbumStyle>()
			.HasKey(a => new { a.AlbumId, a.StyleId });
		modelBuilder.Entity<AlbumSession>()
			.HasKey(a => new { a.AlbumId, a.SessionId});
	}
}
