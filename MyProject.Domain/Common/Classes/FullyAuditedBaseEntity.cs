using Domain.Common.Interfaces;

namespace Domain.Common.Classes
{
    public class FullyAuditedBaseEntity<TKey> : AuditableBaseEntity<TKey>, IDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
