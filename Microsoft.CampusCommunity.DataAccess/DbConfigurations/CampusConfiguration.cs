using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.CampusCommunity.DataAccess.DbConfigurations
{
    public class CampusConfiguration : IEntityTypeConfiguration<Campus>
    {
        public const string TableName = "Campus";


        #region Implementation of IEntityTypeConfiguration<Campus>

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Campus> b)
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