using System;
using System.Collections.Generic;
using System.Security.Claims;
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
        /// <param name="user">used to check if user is allowed to get campus. Member and Campus Lead level is checked. Service needs to perform Hub lead level check</param>
        /// <param name="campusId"></param>
        /// <returns></returns>
        Task<Campus> GetById(Guid campusId, ClaimsPrincipal user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">used to check if user is allowed to get campus</param>
        /// <param name="campusId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<IEnumerable<BasicUser>> GetUsers(Guid campusId, ClaimsPrincipal user, UserScope scope);

        Task<Campus> CreateCampus(ClaimsPrincipal user, Guid hubId, Campus campus, bool modelState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">used to check if user is allowed to get campus</param>
        /// <param name="campusId"></param>
        /// <param name="newLeadId"></param>
        /// <returns></returns>
        Task DefineCampusLead(ClaimsPrincipal user, Guid campusId, Guid newLeadId);

        Task Delete(Guid campusId);
    }
}