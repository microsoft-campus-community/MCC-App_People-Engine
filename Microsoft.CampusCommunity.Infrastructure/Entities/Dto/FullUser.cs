using System;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    public class FullUser : BasicUser
    {
        public bool IsCampusLead { get; set; }
        public bool IsHubLead { get; set; }
        public bool IsAdmin { get; set; }

        public new static FullUser FromGraphUser(User user)
        {
            return new FullUser()
            {
                AccountEnabled = user.AccountEnabled,
                City = user.City,
                HireDate = user.HireDate,
                University = user.Department,
                DisplayName = user.DisplayName,
                Id = Guid.Parse(user.Id),
                JobTitle = user.JobTitle,
                Mail = user.Mail,
                Location = user.OfficeLocation
            };
        }
    }
}