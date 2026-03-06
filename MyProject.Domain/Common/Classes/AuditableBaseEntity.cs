using Domain.Common.Interfaces;

namespace Domain.Common.Classes
{
    public class AuditableBaseEntity<TKey> : BaseEntity<TKey>, IAuditable
    {
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
