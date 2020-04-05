using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface ICampusControllerService
    {
        Task<IEnumerable<Campus>> GetAll();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hubId"></param>
        /// <returns></returns>
        Task<IEnumerable<Campus>> GetAllCampusForHub(Guid hubId);

        Task<Campus> GetMyCampus(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">used to check if user is allowed to get campus. Member and Campus Lead level is checked. Service needs to perform Hub lead level check</param>
        /// <param name="campusId"></param>
        /// <param name="isHubLead">true if user is hub lead -> additional check</param>
        /// <returns></returns>
        Task<Campus> GetById(Guid userId, Guid campusId, bool isHubLead);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">used to check if user is allowed to get campus</param>
        /// <param name="campusId"></param>
        /// <param name="isHubLead">true if user is hub lead -> additional check</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<IEnumerable<BasicUser>> GetUsers(Guid userId, Guid campusId, bool isHubLead, UserScope scope);

        Task<Campus> CreateCampus(Guid hubId, Campus campus, bool modelState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">used to check if user is allowed to get campus</param>
        /// <param name="campusId"></param>
        /// <param name="newLeadId"></param>
        /// <returns></returns>
        Task DefineCampusLead(Guid userId, Guid campusId, Guid newLeadId);

        Task Delete(Guid campusId);
    }
}