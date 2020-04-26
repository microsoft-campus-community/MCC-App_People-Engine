using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphGroupService
    {
        Task AddUserToGroup(User user, Guid groupId);
        Task<MccGraphGroup> GetGroupById(Guid campusId);
        Task<IEnumerable<User>> GetGroupMembers(Guid groupId);
        Task<IEnumerable<MccGraphGroup>> UserMemberOf(string userId);
        Task<IEnumerable<MccGraphGroup>> UserMemberOf(Guid userId);
        Task<MccGraphGroup> CreateGroup(string name, Guid owner, string description, bool shouldCreateTeamsTeam);
        Task ChangeGroupOwner(Guid groupId, Guid newOwner);
        Task<IEnumerable<MccGraphGroup>> GetAllGroups();
        Task<AuthorizationGroupMembers> GetGroupMembersOfAuthorizationGroups();
    }
}