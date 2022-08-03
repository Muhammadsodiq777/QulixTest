using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QulixTest.Core.Domain;
using QulixTest.Persistence.Entities;


namespace QulixTest.Persistence
{
    public class DatabaseDbContext : IdentityDbContext<Author>
    {
        public DatabaseDbContext(DbContextOptions<DatabaseDbContext> options) : base(options)
        { }

        public DbSet<ImageEntity> Images { get; set; }
        public DbSet<Author> Authors{ get; set; }
        public DbSet<Tag> Tags{ get; set; }
        public DbSet<Text> Texts{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfigs());

            //builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
