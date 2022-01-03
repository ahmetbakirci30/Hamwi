using System.Net.Http;

namespace Hamwi.Shared.Services.Http.Interfaces
{
    public interface IHttpService
    {
        HttpClient Client { get; }
    }
}