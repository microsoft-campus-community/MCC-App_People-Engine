using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphCampusService
    {
        Task<IEnumerable<Campus>> GetAllCampus();
        Task<IEnumerable<Campus>> GetAllCampusForhub(Guid hubId);
        Task<Campus> GetCampus(Guid id);
        Task<IEnumerable<BasicUser>> GetCampusUsers(Guid campusId);
        Task<Campus> CreateCampus(Campus campus);
    }
}