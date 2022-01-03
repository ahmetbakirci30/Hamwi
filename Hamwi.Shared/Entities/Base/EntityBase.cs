using Hamwi.Shared.Entities.Interfaces;
using System;

namespace Hamwi.Shared.Entities.Base
{
    public abstract class EntityBase : IEntity
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public bool Active { get; set; } = true;
    }
}