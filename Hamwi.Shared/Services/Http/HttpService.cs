using Hamwi.Shared.Services.Http.Interfaces;
using System;
using System.Net.Http;

namespace Hamwi.Shared.Services.Http
{
    public class HttpService : IHttpService
    {
        private static readonly HttpClient _client;

        static HttpService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://abdulrahmanhamwi-001-site1.itempurl.com/api/")
            };
        }

        public HttpClient Client => _client;
    }
}