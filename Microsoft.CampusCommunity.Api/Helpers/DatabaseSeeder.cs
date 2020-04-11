using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.CampusCommunity.DataAccess;
using Microsoft.CampusCommunity.Infrastructure.Entities.Db;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
        public static void Seed(IApplicationBuilder builder, bool migrate=true, bool seedDevData=true, bool seedProdData=false)
        {
            

            // get context to seed the data
            using var scope = builder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MccContext>();
                
            if (migrate)
                context.Database.Migrate();

            if (seedDevData)
                SeedDevData(context);

            if (seedProdData)
            {
                var graphService = scope.ServiceProvider.GetRequiredService<IGraphGroupService>();
                SeedProdData(context, graphService);
            }
        }

        private static void SeedProdData(MccContext context, IGraphGroupService graphService)
        {
            var defaultRemoteHubId = Guid.Parse("00a7df8e-a810-4aa1-9791-6e0a79a911d3");

            // get all campus
            var allGroups = graphService.GetAllGroups().Result.ToList();

            // divide between hubs and campus
            var campusList = allGroups.Where(g => g.Name.StartsWith("Campus "));
            var hubs = allGroups.Where(g => g.Name.StartsWith("Hub "));

            var dbHubs = hubs.Select(hub => new Hub()
                {
                    AadGroupId = hub.Id,
                    Campus = new List<Campus>(),
                    CreatedAt = DateTime.UtcNow,
                    Lead = Guid.Empty,
                    ModifiedAt = DateTime.UtcNow,
                    Name = hub.Name
                })
                .ToList();

            foreach (var campus in campusList)
            {
                // check if campus exists, otherwise add it
                if (context.Campus.Any(c => c.AadGroupId == campus.Id))
                    continue;

                // find hub for campus
                if (!Guid.TryParse(campus.Description, out var hubId))
                {
                    // description could not be parsed as a guid -> take the default hub Id
                    hubId = defaultRemoteHubId;
                }

                var hub = dbHubs.FirstOrDefault(h => h.AadGroupId == hubId);
                var newCampus = new Campus(campus.Name, Guid.Empty, campus.Id, campus.Name.Replace("Campus ", ""),
                    Guid.Empty)
                {
                    Hub = hub
                };

                context.Campus.Add(newCampus);

                // add campus to hub
                hub?.Campus.ToList().Add(newCampus);
            }

            context.Hubs.AddRange(dbHubs);
            context.SaveChanges();
        }

        private static void SeedDevData(MccContext context)
        {   
            var testCampus = new Campus[]
            {
                new Campus()
                {
                    Id = TestCampusId,
                    Hub = null,
                    Lead = TestCampusLeadId,
                    UniversityName = "Microsoft Global University",
                    Name = "Campus Microsoft University",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedBy = Guid.NewGuid(),
                    AadGroupId = Guid.Parse("bd8226ca-2e6c-4aa6-adcb-85ce8c871b1d")
                },
            };
            var testHubs = new Hub[]
            {
                new Hub()
                {
                    Id = TestHubId,
                    AadGroupId = Guid.Parse("5460cdd4-a626-4cda-aa0b-f013e6eefea2"),
                    CreatedAt = DateTime.UtcNow,
                    Name = "Hub Global University",
                    Campus = new List<Campus>()
                    {
                        testCampus[0]
                    },
                    Lead = TestHubLeadId,
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedBy = Guid.NewGuid()
                },
            };
            testCampus[0].Hub = testHubs[0];

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
