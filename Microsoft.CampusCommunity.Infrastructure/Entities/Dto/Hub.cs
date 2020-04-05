using System;
using System.Collections.Generic;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    public class Hub
    {
        public string Name { get; set; }
        public Guid Lead { get; set; }
        public string LeadName { get; set; }
        public Guid AadGroupId { get; set; }
        public IEnumerable<Campus> Campus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}