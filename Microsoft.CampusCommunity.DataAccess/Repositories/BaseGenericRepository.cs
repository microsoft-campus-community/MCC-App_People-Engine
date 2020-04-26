using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.CampusCommunity.DataAccess.Repositories
{
    public abstract class BaseGenericRepository<TEntity> : IBaseGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly MccContext Context;
        protected readonly DbSet<TEntity> DbSet;

        protected BaseGenericRepository(MccContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        public Task<List<TEntity>> GetAll()
        {
            return GetQueryable().ToListAsync();
        }

        /// <summary>
        /// Searches either by db id or by Aad Group Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> GetById(Guid id)
        {
            return await GetIncludes().FirstOrDefaultAsync(e => e.Id == id || e.AadGroupId == id);
        }

        public async Task Delete(TEntity entity)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }

        public async Task<TEntity> Create(TEntity newEntity)
        {
            var createdEntity = await DbSet.AddAsync(newEntity);
            await Context.SaveChangesAsync();
            return createdEntity.Entity;
        }

        public async Task<TEntity> Update(TEntity entityUpdate)
        {
            var updatedEntity = DbSet.Update(entityUpdate);
            await Context.SaveChangesAsync();
            return updatedEntity.Entity;
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return GetIncludes();
        }

        protected abstract IQueryable<TEntity> GetIncludes();
    }
}