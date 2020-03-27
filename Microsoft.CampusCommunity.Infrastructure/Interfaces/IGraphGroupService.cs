using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphGroupService
    {
        Task AddUserToGroup(User user, Guid groupId);
        Task<MccGroup> GetGroupById(Guid campusId);
        Task<IEnumerable<User>> GetGroupMembers(Guid groupId);
        Task<IEnumerable<MccGroup>> UserMemberOf(string userId);
        Task<IEnumerable<MccGroup>> UserMemberOf(Guid userId);
    }
}