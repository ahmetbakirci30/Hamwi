using Hamwi.Shared.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamwi.Shared.Entities
{
    [Table("Notification")]
    public class Notification : EntityBase
    {
        [Required]
        [StringLength(255, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        [Column(TypeName = "decimal(38, 0)")]
        public decimal ReceivedCount { get; set; }

        [Column(TypeName = "decimal(38, 0)")]
        public decimal OpenedCount { get; set; }
    }
}