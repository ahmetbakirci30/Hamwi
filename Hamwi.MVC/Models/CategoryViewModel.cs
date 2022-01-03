using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hamwi.MVC.Models
{
    public class CategoryViewModel
    {
        [Required]
        [StringLength(255, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string CoverPath { get; set; }
        public bool Active { get; set; } = true;
        public decimal StatusCount { get; set; }
        public IFormFile CoverImageFile { get; set; }
    }
}