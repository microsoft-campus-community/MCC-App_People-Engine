using System;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

namespace Microsoft.CampusCommunity.Services.Db
{
    public class DbHubService : DbBaseGenericService<Hub>
    {
        /// <inheritdoc />
        public DbHubService(IBaseGenericRepository<Hub> repository) : base(repository)
        {
        }

        protected override void CheckEntityIsValid(Hub entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new MccNotValidEntityException($"{nameof(entity.Name)} is not set");

            if (entity.Lead == Guid.Empty)
                throw new MccNotValidEntityException($"{nameof(entity.Lead)} cannot be null guid");

            if (entity.AadGroupId == Guid.Empty)
                throw new MccNotValidEntityException($"{nameof(entity.AadGroupId)} cannot be null guid");
        }
    }
}