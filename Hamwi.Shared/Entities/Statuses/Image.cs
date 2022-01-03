using Hamwi.Shared.Entities.Statuses.Base;
using System.ComponentModel.DataAnnotations;

namespace Hamwi.Shared.Entities.Statuses
{
    public class Image : StatusBase
    {
        [Required]
        public string ImagePath { get; set; }
    }
}