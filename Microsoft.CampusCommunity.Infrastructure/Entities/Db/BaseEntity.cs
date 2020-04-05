using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Db
{
    public abstract class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }

        protected BaseEntity(Guid? modifiedBy)
        {
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = Guid.Empty;
        }
    }
}