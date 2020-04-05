using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;

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


        /// <summary>
        ///     Get all hubs.
        ///     Requirement: CampusLeads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public Task<IEnumerable<Hub>> GetAll()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Get hub by id
        ///     Requirement: CampusLeads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}")]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        public Task<Hub> GetMyHub(Guid id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Delete hub
        ///     Requirement: GermanLeads
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task Delete([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
