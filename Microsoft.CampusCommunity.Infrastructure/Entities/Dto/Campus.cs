using System;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    public class Campus : BaseEntity
    { public string Name { get; set; }
        public Guid HubId { get; set; }
        public string HubName { get; set; }
        public Guid AadGroupId { get; set; }

        // TODO: Use graph extension for a nicer location name
        public string CampusLocation => Name.Replace("Campus ", "");
        public string University { get; set; }
        public Guid Lead { get; set; }

        public static Campus FromMccGroup(MccGraphGroup g)
        {
            return new Campus(null)
            {
                Id = g.Id,
                Name = g.Name,
                HubId = Guid.Empty,
                University = "?",
                Lead = Guid.Empty,
                HubName = "?",
                AadGroupId = g.Id
            };
        }

        public static Campus FromDb(Db.Campus c)
        {
            return new Campus(c.ModifiedBy)
            {
                Id = c.Id,
                Name = c.Name,
                HubId = c.Hub.Id,
                HubName = c.Hub.Name,
                University = c.UniversityName,
                Lead = c.Lead,
                CreatedAt = c.CreatedAt,
                ModifiedAt = c.ModifiedAt,
                AadGroupId = c.AadGroupId
            };
        }

        /// <inheritdoc />
        public Campus(Guid? modifiedBy) : base(modifiedBy)
        {
        }

        public Campus()
        {
        }
    }
}