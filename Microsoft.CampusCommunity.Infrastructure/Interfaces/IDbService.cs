using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IDbService<TDtoEntity>
    {
        Task<List<TDtoEntity>> GetAll();
        Task<TDtoEntity> GetById(Guid id);
        Task<TDtoEntity> Create(TDtoEntity newEntity, bool modelState);
        Task<TDtoEntity> Update(TDtoEntity entityUpdate);
        Task Delete(Guid id);
    }
}