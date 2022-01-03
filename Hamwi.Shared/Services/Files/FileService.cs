using Microsoft.AspNetCore.Http;
using Hamwi.Shared.Services.Files.Interfaces;
using Hamwi.Shared.Services.Http.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hamwi.Shared.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IHttpService _service;
        private readonly string _requestUri;

        public FileService(IHttpService service, string requestUri)
        {
            _service = service;
            _requestUri = requestUri;
        }

        public async Task<byte[]> DownloadAsync(string path)
        {
            try
            {
                _service.Client.DefaultRequestHeaders.Add("path", path);
                return await _service.Client.GetByteArrayAsync(_requestUri);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            try
            {
                using var response = await _service.Client.PostAsync(_requestUri, CofigureFileToSend(file));
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> UpdateAsync(IFormFile file, string path)
        {
            try
            {
                _service.Client.DefaultRequestHeaders.Add("path", path);
                using var response = await _service.Client.PutAsync(_requestUri, CofigureFileToSend(file));
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> DeleteAsync(string path)
        {
            try
            {
                _service.Client.DefaultRequestHeaders.Add("path", path);
                using var response = await _service.Client.DeleteAsync(_requestUri);
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }

        private HttpContent CofigureFileToSend(IFormFile file)
            => new MultipartFormDataContent
            {
                {
                    new StreamContent(file.OpenReadStream())
                    {
                        Headers =
                        {
                            ContentLength = file.Length,
                            ContentType = new MediaTypeHeaderValue(file.ContentType)
                        }
                    },
                    "File",
                    file.FileName
                }
            };
    }
}