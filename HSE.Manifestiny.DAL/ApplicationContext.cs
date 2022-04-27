using Microsoft.EntityFrameworkCore;
using HSE.Manifestiny.DAL.DalModels;

namespace HSE.Manifestiny.DAL;

public class ApplicationContext : DbContext
{
	public DbSet<UserDAL> Users { get; set; } = null!;
	public DbSet<PlaceDAL> Places { get; set; } = null!;
	public DbSet<TripDAL> Trips { get; set; } = null!;
	public DbSet<CityDAL> Cities { get; set; } = null!;
	public DbSet<TravelTimeDAL> TravelTime { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		new TripConfiguration().Configure(modelBuilder.Entity<TripDAL>());
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=manifestiny;Username=postgres;Password=postgres");
	}
}