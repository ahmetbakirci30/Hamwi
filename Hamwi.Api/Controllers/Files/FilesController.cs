using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hamwi.Shared.Extensions.Files.FormFile;
using Hamwi.Shared.Managers.Files.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Hamwi.Api.Controllers.Files
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileManager _fileManager;

        public FilesController(IFileManager fileManager) => _fileManager = fileManager;

        [HttpGet]
        public async Task<ActionResult> DownloadAsync([Required][FromHeader] string path)
        {
            try
            {
                var name = Path.GetFileName(path);
                var file = await _fileManager.DownloadAsync(name);

                return (file != null && file.Length > 0) ?
                    File(file, MediaTypeNames.Application.Octet, name) :
                    NotFound($"File named {name} not found!");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error downloading file!");
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> UploadAsync([Required][FromForm] IFormFile file)
        {
            try
            {
                return (file.Length > 0) ?
                    Ok(GetUploadedFilePath(file, await _fileManager.UploadAsync(file))) :
                    BadRequest("File is required!");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error uploading file!");
            }
        }

        [HttpPut]
        public async Task<ActionResult<string>> UpdateAsync([Required][FromHeader] string path,
            [Required][FromForm] IFormFile file)
        {
            try
            {
                return (file.Length > 0) ?
                    Ok(GetUploadedFilePath(file, await _fileManager.UpdateAsync(file, Path.GetFileName(path)))) :
                    BadRequest("File is required!");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating file!");
            }
        }

        [HttpDelete]
        public async Task<ActionResult<string>> DeleteAsync([Required][FromHeader] string path)
        {
            try
            {
                var name = Path.GetFileName(path);
                var deleted = await _fileManager.DeleteAsync(name);

                return string.IsNullOrWhiteSpace(deleted) ?
                    NotFound($"File named {name} not found!") :
                    Ok($"The file named {deleted} has been successfully deleted!");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting file!");
            }
        }

        private string GetUploadedFilePath(IFormFile file, string fileName)
            => string.Format("{0}://{1}{2}/{3}/{4}/{5}",
                             Request.Scheme,
                             Request.Host,
                             Request.PathBase,
                             "statuses",
                             file.IsImage() ? "images" : "videos",
                             fileName);
    }
}