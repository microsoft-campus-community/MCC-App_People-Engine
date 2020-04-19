using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IUserControllerService
    {
        Task<IEnumerable<BasicUser>> GetAll(UserScope scope);
        Task<BasicUser> GetUserById(Guid id, UserScope scope);
        Task<BasicUser> CreateUser(NewUser user, Guid campusId);
    }
}