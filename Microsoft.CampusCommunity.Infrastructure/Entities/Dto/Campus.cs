using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    public class Campus : MccGroup
    {
        public Guid HubId { get; set; }

        // TODO: Use graph extension for a nicer location name
        public string CampusLocation => Name.Replace("Campus ", "");


        public static Campus GetFromMccGroup(MccGroup g)
        {
            return new Campus()
            {
                Id = g.Id,
                Name = g.Name,
                HubId = Guid.Empty
            };
        }
    }
}