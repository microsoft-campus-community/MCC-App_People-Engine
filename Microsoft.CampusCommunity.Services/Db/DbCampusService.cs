using System;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

namespace Microsoft.CampusCommunity.Services.Db
{
    public class DbCampusService : DbBaseGenericService<Campus>
    {
        /// <inheritdoc />
        public DbCampusService(IBaseGenericRepository<Campus> repository) : base(repository)
        {
        }

        #region Overrides of DbBaseGenericService<Campus>

        /// <inheritdoc />
        protected override void CheckEntityIsValid(Campus entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new MccNotValidEntityException($"{nameof(entity.Name)} is not set");

            if (entity.Lead == Guid.Empty)
                throw new MccNotValidEntityException($"{nameof(entity.Lead)} cannot be null guid");

            if (entity.AadGroupId == Guid.Empty)
                throw new MccNotValidEntityException($"{nameof(entity.AadGroupId)} cannot be null guid");
        }

        #endregion
    }
}