using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    public class Hub : BaseEntity
    {
        public string Name { get; set; }
        public Guid Lead { get; set; }
        public string LeadName { get; set; }
        public Guid AadGroupId { get; set; }
        public IEnumerable<Campus> Campus { get; set; }

        public static Hub FromDb(Db.Hub h)
        {
            var campus = h.Campus.Select(Dto.Campus.FromDb);
            return new Hub(h.ModifiedBy)
            {
                Name = h.Name,
                Lead = h.Lead,
                LeadName = "?",
                AadGroupId = h.AadGroupId,
                Campus = campus,
                CreatedAt = h.CreatedAt,
                ModifiedAt = h.ModifiedAt
            };
        }

        /// <inheritdoc />
        public Hub(Guid? modifiedBy) : base(modifiedBy)
        {
        }
    }
}