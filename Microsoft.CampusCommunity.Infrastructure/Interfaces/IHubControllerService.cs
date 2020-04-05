using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IHubControllerService
    {
        Task<IEnumerable<Hub>> GetAll();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">used to check if user is allowed to get hub</param>
        /// <returns></returns>
        Task<Hub> GetMyHub(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">used to check if user is allowed to get hub</param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Hub> GetHubById(Guid userId, Guid id);

        Task<Hub> Create(Hub entity, bool modelState);

        Task<Hub> Update(Hub entity, bool modelState);

        Task<Hub> ChangeHubLead(Guid id, Guid newLead);

        Task Delete(Guid id);

    }
}