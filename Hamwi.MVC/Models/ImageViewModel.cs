using Microsoft.AspNetCore.Http;
using System;

namespace Hamwi.MVC.Models
{
    public class ImageViewModel
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public bool Active { get; set; } = true;
        public string ImagePath { get; set; }
        public decimal ViewsCount { get; set; }
        public decimal LikesCount { get; set; }
        public decimal SharesCount { get; set; }
        public decimal DownloadsCount { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}