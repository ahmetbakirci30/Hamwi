using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hamwi.MVC.Models
{
    public class VideoViewModel
    {
        [Required]
        [StringLength(255, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public string Title { get; set; }
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public bool Active { get; set; } = true;
        public string VideoPath { get; set; }
        public string CoverPath { get; set; }
        public decimal ViewsCount { get; set; }
        public decimal LikesCount { get; set; }
        public decimal SharesCount { get; set; }
        public decimal DownloadsCount { get; set; }
        public IFormFile VideoFile { get; set; }
        public IFormFile CoverFile { get; set; }
    }
}