using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphUserService
    {
        Task<IEnumerable<BasicUser>> GetAllUsers();
        Task<BasicUser> GetCurrentUser(Guid userId);
        Task<BasicUser> CreateUser(NewUser user, Guid campusId);
        Task<Guid> GetCampusIdForUser(Guid userId);
        Task DefineCampusLead(Guid userId, Guid campusId);
        Task DefineHubLead(Guid newLead, IEnumerable<Guid> campusLeads, Guid hubId);
        Task SendMail(string subject, string body, User fromUser, string to);
    }
}