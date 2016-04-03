using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Windyship.Entities;

namespace Windyship.Dal
{
	public class WindyContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Content> Contents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			base.OnModelCreating( modelBuilder );
        }
	}
}