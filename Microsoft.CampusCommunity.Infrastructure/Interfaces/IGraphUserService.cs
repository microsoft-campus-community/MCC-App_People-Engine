using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphUserService
    {
        Task<IEnumerable<BasicUser>> GetAllUsers(UserScope scope);
        Task<IEnumerable<FullUser>> AddFullScope(IEnumerable<User> graphUsers);
        Task<BasicUser> GetBasicUserById(Guid userId, UserScope scope);
        Task<User> GetGraphUserById(Guid userId);
        Task<BasicUser> CreateUser(NewUser user, Guid campusId);
        Task<Guid> GetCampusIdForUser(Guid userId);
        Task DefineCampusLead(Guid userId, Guid campusId);
        Task DefineHubLead(Guid newLead, IEnumerable<Guid> campusLeads, Guid hubId);
        Task AssignManager(User user, string managerId);
        Task SendMail(string subject, string body, User fromUser, string to);
    }
}