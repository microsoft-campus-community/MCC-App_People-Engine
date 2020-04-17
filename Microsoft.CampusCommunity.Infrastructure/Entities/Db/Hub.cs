using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Db
{
    public class Hub : BaseEntity
    {
        public string Name { get; set; }
        public Guid Lead { get; set; }
        public List<Campus> Campus { get; set; }

        /// <inheritdoc />
        public Hub(string name, Guid lead, Guid aadGroupId, Guid modifiedBy) : base(modifiedBy)
        {
            Name = name;
            Lead = lead;
            AadGroupId = aadGroupId;
            Campus = new List<Campus>();
        }

        /// <inheritdoc />
        public Hub() : base()
        {
        }
    }
}
