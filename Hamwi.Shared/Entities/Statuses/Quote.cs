using Hamwi.Shared.Entities.Statuses.Base;
using System.ComponentModel.DataAnnotations;

namespace Hamwi.Shared.Entities.Statuses
{
    public class Quote : StatusBase
    {
        [Required]
        public string Content { get; set; }
    }
}