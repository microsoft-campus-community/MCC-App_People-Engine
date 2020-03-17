using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphService
    {
        GraphServiceClient Client { get; }
    }
}