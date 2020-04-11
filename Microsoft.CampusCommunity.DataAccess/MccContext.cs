using Microsoft.CampusCommunity.DataAccess.DbConfigurations;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.CampusCommunity.DataAccess
{
    /// <summary>
    /// Default database context for MCC app
    /// </summary>
    public class MccContext : DbContext
    {
        public MccContext()
        {
        }

        public MccContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Hub> Hubs { get; set; }
        public DbSet<Campus> Campus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
        }

        #region Overrides of DbContext

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new HubConfiguration());
            modelBuilder.ApplyConfiguration(new CampusConfiguration());
        }

        #endregion
    }
}