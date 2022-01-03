using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Hamwi.Shared.Extensions.Files.FormFile;
using Hamwi.Shared.Managers.Files.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hamwi.Shared.Managers.Files
{
    public class FileManager : IFileManager
    {
        private readonly string _imagesFolder;
        private readonly string _videosFolder;

        public FileManager(IHostingEnvironment hostEnvironment)
        {
            var statusesFolder = Path.Combine(hostEnvironment.WebRootPath, "statuses");
            _imagesFolder = Path.Combine(statusesFolder, "images");
            _videosFolder = Path.Combine(statusesFolder, "videos");
        }

        public async Task<byte[]> DownloadAsync(string name)
        {
            string image = Path.Combine(_imagesFolder, name);
            string video = Path.Combine(_videosFolder, name);

            if (File.Exists(image))
                return await File.ReadAllBytesAsync(image);

            else if (File.Exists(video))
                return await File.ReadAllBytesAsync(video);

            return null;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var storagePath = file.IsImage() ? _imagesFolder : _videosFolder;

            if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);

            var fileName = GenerateUniqueName(file.FileName);
            var fullPath = Path.Combine(storagePath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public async Task<string> UpdateAsync(IFormFile file, string name)
        {
            await DeleteAsync(name);
            return await UploadAsync(file);
        }

        public async Task<string> DeleteAsync(string name)
        {
            string image = Path.Combine(_imagesFolder, name);
            string video = Path.Combine(_videosFolder, name);

            if (File.Exists(image))
            {
                File.Delete(image);
                return await Task.FromResult(name);
            }
            else if (File.Exists(video))
            {
                File.Delete(video);
                return await Task.FromResult(name);
            }

            return await Task.FromResult(string.Empty);
        }

        private static string GenerateUniqueName(string fileName)
        {
            var uniqueName = Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);
            var currentUtcDateTime = DateTime.UtcNow;

            return string.Join("-", currentUtcDateTime.Year,
                                    currentUtcDateTime.Month,
                                    currentUtcDateTime.Day,
                                    currentUtcDateTime.Hour,
                                    currentUtcDateTime.Minute,
                                    currentUtcDateTime.Second,
                                    currentUtcDateTime.Millisecond,
                                    uniqueName);
        }
    }
}