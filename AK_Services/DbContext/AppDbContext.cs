using AFKAT_Servies;
using Microsoft.EntityFrameworkCore;


	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		// public DbSet<Leaderboards> Leaderboards { get; set; }
		// public DbSet<LeaderboardEntries> LeaderboardEntries { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// modelBuilder.Entity<Leaderboards>().ToTable("Leaderboards");
			modelBuilder.Entity<LeaderboardEntries>(
				
			).HasNoKey().ToTable("LeaderboardEntries");
		}
	}

