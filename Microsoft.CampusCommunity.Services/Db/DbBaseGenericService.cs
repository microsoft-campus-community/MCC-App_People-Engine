using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

namespace Microsoft.CampusCommunity.Services.Db
{
    public abstract class DbBaseGenericService<TEntity> : IDbService<TEntity> where TEntity : BaseEntity
    {
        protected readonly IBaseGenericRepository<TEntity> Repository;

        protected DbBaseGenericService(IBaseGenericRepository<TEntity> repository)
        {
            Repository = repository;
        }


        #region Implementation of IDbService<TEntity>

        /// <inheritdoc />
        public Task<List<TEntity>> GetAll()
        {
            return Repository.GetAll();
        }

        /// <inheritdoc />
        public Task<TEntity> GetById(Guid id)
        {
            return Repository.GetById(id);
        }

        /// <inheritdoc />
        public Task<TEntity> Create(TEntity newEntity, bool modelState)
        {
            if (!modelState)
                throw new MccBadRequestException("New entity does not have a valid model state");
            CheckEntityIsValid(newEntity);


            return Repository.Create(newEntity);
        }

        /// <inheritdoc />
        public Task<TEntity> Update(TEntity entityUpdate)
        {
            CheckEntityIsValid(entityUpdate);

            return Repository.Update(entityUpdate);
        }

        /// <inheritdoc />
        public async Task Delete(Guid id)
        {
            var entityToDelete = await GetById(id);
            if (entityToDelete == null)
                throw new MccNotFoundException($"Could not find entity with id {id}.");

            await Repository.Delete(entityToDelete);
        }

        #endregion

        protected abstract void CheckEntityIsValid(TEntity entity);
    }
}