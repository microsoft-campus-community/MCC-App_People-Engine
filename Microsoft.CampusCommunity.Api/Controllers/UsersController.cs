using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Graph;

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
        public Task<IEnumerable<BasicUser>> Get()
        {
            return _graphService.GetUsers();
        }
    }
}