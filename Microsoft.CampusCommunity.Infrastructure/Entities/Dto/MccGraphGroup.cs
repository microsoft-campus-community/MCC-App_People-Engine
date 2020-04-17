using System;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    public class MccGraphGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Description { get; set; }
        public Guid Owner { get; set; }

        public static MccGraphGroup FromGraph(Group g)
        {
            var id = Guid.Parse(g.Id);
            return new MccGraphGroup()
            {
                Id = id,
                Name = g.DisplayName,
                Mail = g.MailEnabled.HasValue && g.MailEnabled.Value ? g.Mail : "no mail",
                Description = g.Description
            };
        }
    }
}