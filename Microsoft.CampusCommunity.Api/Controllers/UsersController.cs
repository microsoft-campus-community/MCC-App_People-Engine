using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

namespace Microsoft.CampusCommunity.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IGraphService _graphService;

        public UsersController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<IEnumerable<BasicUser>> Get(
            [FromQuery(Name = "scope")] UserScope scope)
        {
            return _graphService.GetUsers();
        }
    }
}