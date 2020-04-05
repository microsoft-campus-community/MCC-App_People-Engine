using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Api.Controllers
{
    /// <summary>
    /// Controller for Hub Operations
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/hubs")]
    public class HubController : ControllerBase
    {
        private readonly AuthorizationConfiguration _authConfig;
        private readonly IHubControllerService _service;

        public HubController(IHubControllerService service)
        {
            _service = service;
        }


        /// <summary>
        ///     Get all hubs.
        ///     Requirement: HubLeads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public Task<IEnumerable<Hub>> GetAll()
        {
            return _service.GetAll();
        }

        /// <summary>
        ///     Get my hub
        ///     Requirement: CampusLeads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("my")]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        public Task<Hub> GetMyHub()
        {
            var userId = AuthenticationHelper.GetUserIdFromToken(User);
            return _service.GetMyHub(userId);
        }

        /// <summary>
        ///     Get hub by id
        ///     Requirement: CampusLeads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}")]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        public Task<Hub> GetHubById(Guid id)
        {
            var userId = AuthenticationHelper.GetUserIdFromToken(User);
            return _service.GetHubById(userId, id);
        }

        /// <summary>
        ///     Create hub
        ///     Requirement: GermanLeads
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<Hub> Create([FromBody] Hub entity)
        {
            return _service.Create(entity, ModelState.IsValid);
        }

        /// <summary>
        ///     Update hub
        ///     Requirement: HubLeads
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public Task<Hub> Update([FromRoute] Guid id, [FromBody] Hub entity)
        {
            return _service.Update(entity, ModelState.IsValid);
        }

        /// <summary>
        ///     Change Hub Lead
        ///     Requirement: GermanLeads
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/lead")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<Hub> ChangeHubLead([FromRoute] Guid id, [FromQuery] Guid newLead)
        {
            return _service.ChangeHubLead(id, newLead);
        }

        /// <summary>
        ///     Delete hub
        ///     Requirement: GermanLeads
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _service.Delete(id);
            return NoContent();
        }
    }
}
