using Hamwi.Shared.Entities.Base;
using Hamwi.Shared.Filters.Entity.Interfaces;
using Hamwi.Shared.Services.Http.Interfaces;
using Hamwi.Shared.Services.Hamwi.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Hamwi.Shared.Services.Hamwi
{
    public class HamwiService<T> : IHamwiService<T> where T : EntityBase, new()
    {
        private readonly IHttpService _service;
        private readonly string _requestUri;

        public HamwiService(IHttpService service, string requestUri)
        {
            _service = service;
            _requestUri = requestUri;
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            try
            {
                using var response = await _service.Client.GetAsync(_requestUri);
                return await response.Content.ReadAsAsync<IEnumerable<T>>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetAsync(IFilter<T> filter)
        {
            try
            {
                using var response = await _service.Client.GetAsync($"{_requestUri}/search{filter}");
                return await response.Content.ReadAsAsync<IEnumerable<T>>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<T> GetAsync(Guid id)
        {
            try
            {
                using var response = await _service.Client.GetAsync($"{_requestUri}/{id}");
                return await response.Content.ReadAsAsync<T>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                using var response = await _service.Client.PostAsync(_requestUri, SerializeData(entity));
                return await response.Content.ReadAsAsync<T>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                using var response = await _service.Client.PutAsync(_requestUri, SerializeData(entity));
                return await response.Content.ReadAsAsync<T>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<T> DeleteAsync(Guid id)
        {
            try
            {
                using var response = await _service.Client.DeleteAsync($"{_requestUri}/{id}");
                return await response.Content.ReadAsAsync<T>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<decimal?> CountAsync(IFilter<T> filter = null)
        {
            try
            {
                using var response = await _service.Client.GetAsync($"{_requestUri}/count{filter}");
                return await response.Content.ReadAsAsync<decimal>();
            }
            catch
            {
                return null;
            }
        }

        private static HttpContent SerializeData(T entity)
            => new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}