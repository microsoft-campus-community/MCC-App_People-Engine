using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    public class NewUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondaryMail { get; set; }
        public Guid CampusId { get; set; }
    }
}