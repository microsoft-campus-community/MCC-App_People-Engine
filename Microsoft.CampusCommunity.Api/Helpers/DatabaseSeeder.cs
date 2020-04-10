using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.CampusCommunity.DataAccess;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.CampusCommunity.Api.Helpers
{
    public static class DatabaseSeeder
    {
        public static readonly Guid TestHubId = Guid.Parse("b8a808ef-3773-4835-a280-0dbb8ca1d71e");
        public static readonly Guid TestHubLeadId = Guid.Parse("28a82a7e-24fe-49dd-b65c-8c36677f4a5c");
        public static readonly Guid TestCampusId = Guid.Parse("0d5d2fdb-d4b0-470f-8a78-2269bbbe34a9");
        public static readonly Guid TestCampusLeadId = Guid.Parse("1e22240f-88da-4e4e-884e-e1f9a3083cb8");


        /// <summary>
        /// Creates initial database config and applies migrations on startup.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="migrate"></param>
        /// <param name="seedDevData"></param>
        public static void Seed(IApplicationBuilder builder, bool migrate=true, bool seedDevData=true)
        {
            

            // get context to seed the data
            using var scope = builder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MccContext>();
                
            if (migrate)
                context.Database.Migrate();

            if (seedDevData)
                SeedDevData(context);
        }

        private static void SeedDevData(MccContext context)
        {
            var testHubs = new Hub[]
            {
                new Hub()
                {
                    Id = TestHubId,
                    AadGroupId = Guid.Parse("5460cdd4-a626-4cda-aa0b-f013e6eefea2"),
                    CreatedAt = DateTime.UtcNow,
                    Name = "Hub Global University",
                    Campus = new List<Campus>(),
                    Lead = TestHubLeadId,
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedBy = Guid.NewGuid()
                },
            };

            var testCampus = new Campus[]
            {
                new Campus()
                {
                    Id = TestCampusId,
                    Hub = testHubs[0],
                    Lead = TestCampusLeadId,
                    UniversityName = "Microsoft Global University",
                    Name = "Campus Microsoft University",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedBy = Guid.NewGuid(),
                    AadGroupId = Guid.Parse("bd8226ca-2e6c-4aa6-adcb-85ce8c871b1d")
                },
            };
            testHubs[0].Campus.Append(testCampus[0]);

            // check if all data is in database
            foreach (var h in testHubs)
            {
                if (context.Hubs.Any(h_ => h_.Id == h.Id)) continue;
                context.Hubs.Add(h);
            }

            foreach (var c in testCampus)
            {
                if (context.Campus.Any(c_ => c_.Id == c.Id)) continue;
                context.Campus.Add(c);
            }

            context.SaveChanges();
        }
    }
}
