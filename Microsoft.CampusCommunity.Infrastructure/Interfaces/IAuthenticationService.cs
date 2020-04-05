using System;
using System.Threading.Tasks;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> IsUserHubLeadForCampus(Guid userId, Guid campusId);
    }
}