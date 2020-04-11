using System.Linq;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.CampusCommunity.DataAccess.Repositories
{
    public class HubRepository : BaseGenericRepository<Hub>
    {
        /// <inheritdoc />
        public HubRepository(MccContext context) : base(context)
        {
        }

        #region Overrides of BaseGenericRepository<Hub>

        /// <inheritdoc />
        protected override IQueryable<Hub> GetIncludes()
        {
            return DbSet.Include(e => e.Campus);
        }

        #endregion
    }
}