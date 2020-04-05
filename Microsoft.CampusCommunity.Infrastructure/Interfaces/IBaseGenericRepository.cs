using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IBaseGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAll();
        IQueryable<TEntity> GetQueryable();
        Task<TEntity> GetById(Guid id);
        Task<TEntity> Create(TEntity newEntity);
        Task<TEntity> Update(TEntity entityUpdate);
        Task Delete(TEntity entity);
    }
}