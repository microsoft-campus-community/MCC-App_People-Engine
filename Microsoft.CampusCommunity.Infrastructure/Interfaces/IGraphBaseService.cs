using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphBaseService
    {
        GraphServiceClient Client { get; }
    }
}