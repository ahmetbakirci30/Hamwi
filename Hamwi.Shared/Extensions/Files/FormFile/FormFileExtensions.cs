using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Hamwi.Shared.Extensions.Files.FormFile
{
    public static class FormFileExtensions
    {
        public const int ImageMinimumBytes = 512;

        public static bool IsImage(this IFormFile file)
        {
            if (file.ContentType.ToLower() != "image/jpg" && file.ContentType.ToLower() != "image/jpeg" && file.ContentType.ToLower() != "image/pjpeg" && file.ContentType.ToLower() != "image/gif" && file.ContentType.ToLower() != "image/x-png" && file.ContentType.ToLower() != "image/png")
                return false;

            if (Path.GetExtension(file.FileName).ToLower() != ".jpg" && Path.GetExtension(file.FileName).ToLower() != ".png" && Path.GetExtension(file.FileName).ToLower() != ".gif" && Path.GetExtension(file.FileName).ToLower() != ".jpeg")
                return false;

            try
            {
                if (!file.OpenReadStream().CanRead)
                    return false;

                if (file.Length < ImageMinimumBytes)
                    return false;

                byte[] buffer = new byte[ImageMinimumBytes];
                file.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = Encoding.UTF8.GetString(buffer);

                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}