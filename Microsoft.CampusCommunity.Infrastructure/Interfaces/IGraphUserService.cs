using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphUserService
    {
        Task<IEnumerable<BasicUser>> GetAllUsers();
		Task<BasicUser> GetCurrentUser(Guid userId);
		Task<BasicUser> CreateUser(NewUser user);
		Task DefineCampusLead(Guid userId, Guid campusId);
    }
}