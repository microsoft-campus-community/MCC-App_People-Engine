using System.Linq;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;

namespace Microsoft.CampusCommunity.DataAccess.Repositories
{
    public class CampusRepository: BaseGenericRepository<Campus>
    {
        /// <inheritdoc />
        public CampusRepository(MccContext context) : base(context)
        {
        }

        #region Overrides of BaseGenericRepository<Campus>

        /// <inheritdoc />
        protected override IQueryable<Campus> GetIncludes()
        {
            return DbSet;
        }

        #endregion
    }
}