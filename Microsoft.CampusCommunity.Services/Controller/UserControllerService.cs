using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

namespace Microsoft.CampusCommunity.Services.Controller
{
    public class UserControllerService : IUserControllerService
    {
        private readonly IGraphUserService _graphUserService;

        public UserControllerService(IGraphUserService graphUserService)
        {
            _graphUserService = graphUserService;
        }

        public Task<IEnumerable<BasicUser>> GetAll(UserScope scope)
        {
            return _graphUserService.GetAllUsers(scope);
        }

        public Task<BasicUser> GetUserById(Guid id, UserScope scope)
        {
            return _graphUserService.GetBasicUserById(id, scope);
        }

        public async Task<BasicUser> CreateUser(NewUser user, Guid campusId)
        {
            return await _graphUserService.CreateUser(user, campusId);
        }
    }
}