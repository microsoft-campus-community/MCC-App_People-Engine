using Microsoft.EntityFrameworkCore;

namespace Microsoft.CampusCommunity.DataAccess
{
    public class MccContext : DbContext
    {
        public MccContext()
        {
        }

        public MccContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
        }
    }
}