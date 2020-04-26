using System;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    public class BasicUser
    {
        public bool? AccountEnabled { get; set; }

        /// <summary>
        /// City of user
        /// </summary>
        /// <example>Munich</example>
        public string City { get; set; }

        //public string University { get; set; } //Company
        public DateTimeOffset? HireDate { get; set; }
        public string University { get; set; } // Department
        public string DisplayName { get; set; }
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }
        public string Location { get; set; }

        public  static BasicUser FromGraphUser(User user)
        {
            return new BasicUser()
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