using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Db
{
    public class Campus : BaseEntity
    {
        public string Name { get; set; }
        public Guid Lead { get; set; }
        public Guid AadGroupId { get; set; }

        /// <inheritdoc />
        public Campus(string name, Guid lead, Guid aadGroupId, Guid modifiedBy) : base(modifiedBy)
        {
            Name = name;
            Lead = lead;
            AadGroupId = aadGroupId;
        }
    }
}