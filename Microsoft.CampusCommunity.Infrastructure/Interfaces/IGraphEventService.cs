using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphEventService
    {
        Task<IEnumerable<Event>> GetAllEvents();
        //Task<Event> CreateEvent(Event event);
    }
}