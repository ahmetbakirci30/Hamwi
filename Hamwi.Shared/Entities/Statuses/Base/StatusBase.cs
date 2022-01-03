using Hamwi.Shared.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamwi.Shared.Entities.Statuses.Base
{
    [Table("Status")]
    public abstract class StatusBase : EntityBase
    {
        [Column(TypeName = "decimal(38, 0)")]
        public decimal ViewsCount { get; set; }

        [Column(TypeName = "decimal(38, 0)")]
        public decimal LikesCount { get; set; }

        [Column(TypeName = "decimal(38, 0)")]
        public decimal SharesCount { get; set; }

        [Column(TypeName = "decimal(38, 0)")]
        public decimal DownloadsCount { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}