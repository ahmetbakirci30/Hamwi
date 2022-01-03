using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Hamwi.Shared.Services.Files.Interfaces
{
    public interface IFileService
    {
        public Task<byte[]> DownloadAsync(string path);
        public Task<string> UploadAsync(IFormFile file);
        public Task<string> UpdateAsync(IFormFile file, string path);
        public Task<string> DeleteAsync(string path);
    }
}