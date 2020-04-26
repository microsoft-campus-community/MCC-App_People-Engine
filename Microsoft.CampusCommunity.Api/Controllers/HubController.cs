using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

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
        private readonly IMccAuthorizationService _authorizationService;
        private readonly IHubControllerService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="authorizationService"></param>
        public HubController(IHubControllerService service, IMccAuthorizationService authorizationService)
        {
            _service = service;
            _authorizationService = authorizationService;
        }


        /// <summary>
        ///     Get all hubs.
        ///     Requirement: <see cref="PolicyNames.HubLeads"/>
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
        ///     Requirement: <see cref="PolicyNames.CampusLeads"/>
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
        ///     Requirement: <see cref="PolicyNames.CampusLeads"/> - Campus Leads can get their own hub - hub leads can get any hub
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}")]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        public async Task<Hub> GetHubById(Guid hubId)
        {
            await _authorizationService.CheckAuthorizationRequirement(User,
                new[]
                {
                    new AuthorizationRequirement(AuthorizationRequirementType.IsGermanLead),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsGeneralHubLead),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsCampusLeadForHub, hubId),
                });
            return await _service.GetHubById(hubId);
        }

        /// <summary>
        ///     Create hub
        ///     Requirement: <see cref="PolicyNames.GermanLeads"/>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<Hub> Create([FromBody] Hub entity)
        {
            var userId = AuthenticationHelper.GetUserIdFromToken(User);
            return _service.Create(userId, entity, ModelState.IsValid);
        }

        /// <summary>
        ///     Update hub
        ///     Requirement: <see cref="PolicyNames.HubLeads"/>
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public async Task<Hub> Update([FromRoute] Guid id, [FromBody] Hub entity)
        {
            var userId = AuthenticationHelper.GetUserIdFromToken(User);
            await _authorizationService.CheckAuthorizationRequirement(User,
                new[]
                {
                    new AuthorizationRequirement(AuthorizationRequirementType.IsGermanLead),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsHubLeadForHub, id),
                });
            return await _service.Update(userId, entity, ModelState.IsValid);
        }

        /// <summary>
        ///     Change Hub Lead
        ///     Requirement: <see cref="PolicyNames.GermanLeads"/>
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}/lead")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<Hub> ChangeHubLead([FromRoute] Guid id, [FromQuery] Guid newLead)
        {
            var userId = AuthenticationHelper.GetUserIdFromToken(User);
            return _service.ChangeHubLead(userId, id, newLead);
        }

        /// <summary>
        ///     Delete hub
        ///     Requirement: <see cref="PolicyNames.GermanLeads"/>
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
