using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Db
{
    public class Campus : BaseEntity
    {
        public string Name { get; set; }
        public Hub Hub { get; set; }
        public Guid Lead { get; set; }
        public Guid AadGroupId { get; set; }
        public string UniversityName { get; set; }

        /// <inheritdoc />
        public Campus(string name, Guid lead, Guid aadGroupId, string universityName, Guid modifiedBy) : base(modifiedBy)
        {
            Name = name;
            Lead = lead;
            AadGroupId = aadGroupId;
            UniversityName = universityName;
        }

        /// <inheritdoc />
        public Campus() :base()
        {
        }
    }
}