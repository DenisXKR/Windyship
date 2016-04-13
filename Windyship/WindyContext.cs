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

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			base.OnModelCreating(modelBuilder);
		}
	}
}