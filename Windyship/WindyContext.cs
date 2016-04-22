using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Windyship.Entities;

namespace Windyship.Dal
{
	public class WindyContext : DbContext
	{
		public WindyContext()
			: base("name=WindyContext")
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Content> Contents { get; set; }
		public DbSet<Faq> Faq { get; set; }
		public DbSet<UserRequest> UserRequest { get; set; }
		public DbSet<Category> Category { get; set; }
		public DbSet<LocationTo> LocationTo { get; set; }
		public DbSet<LocationFrom> LocationFrom { get; set; }
		public DbSet<Shipment> Shipment { get; set; }
		public DbSet<DisabledCategories> DisabledCategories { get; set; }
		public DbSet<TravelFrom> TravelFrom { get; set; }
		public DbSet<TravelTo> TravelTo { get; set; }
		public DbSet<CarryTravel> CarryTravel { get; set; }
		public DbSet<CarrierReview> CarrierReview { get; set; }
		public DbSet<UserPhone> UserPhone { get; set; }
		public DbSet<Notification> Notification { get; set; }
		public DbSet<DeviceToken> DeviceToken { get; set; }
		public DbSet<InterestedShipment> InterestedShipment { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			modelBuilder.Entity<LocationTo>()
				.Property(x => x.Lat)
				.HasPrecision(9, 6);

			modelBuilder.Entity<LocationTo>()
				.Property(x => x.Long)
				.HasPrecision(9, 6);

			modelBuilder.Entity<LocationFrom>()
				.Property(x => x.Lat)
				.HasPrecision(9, 6);

			modelBuilder.Entity<LocationFrom>()
				.Property(x => x.Long)
				.HasPrecision(9, 6);

			modelBuilder.Entity<TravelFrom>()
				.Property(x => x.Lat)
				.HasPrecision(9, 6);

			modelBuilder.Entity<TravelFrom>()
				.Property(x => x.Long)
				.HasPrecision(9, 6);

			modelBuilder.Entity<TravelTo>()
				.Property(x => x.Lat)
				.HasPrecision(9, 6);

			modelBuilder.Entity<TravelTo>()
				.Property(x => x.Long)
				.HasPrecision(9, 6);

			base.OnModelCreating(modelBuilder);
		}
	}
}