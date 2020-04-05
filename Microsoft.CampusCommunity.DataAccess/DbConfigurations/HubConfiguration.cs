using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.CampusCommunity.DataAccess.DbConfigurations
{
    public class HubConfiguration : IEntityTypeConfiguration<Hub>
    {
        public const string TableName = "Hub";

        #region Implementation of IEntityTypeConfiguration<Hub>

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Hub> b)
        {
            b.ToTable(TableName);
            b.HasKey(k => k.Id);

            b.Property(e => e.Name).IsRequired();
            b.Property(e => e.Lead).IsRequired();
            b.Property(e => e.AadGroupId).IsRequired();
        }

        #endregion
    }
}