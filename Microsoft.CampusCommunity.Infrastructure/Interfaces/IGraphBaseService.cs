using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IGraphBaseService
    {
        GraphServiceClient Client { get; }
        GraphClientConfiguration Configuration { get; }
        AuthorizationConfiguration AuthorizationConfiguration { get; }
    }
}