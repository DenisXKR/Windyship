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

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			base.OnModelCreating(modelBuilder);
		}
	}
}