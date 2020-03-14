using CommunityItaly.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CommunityItaly.EF.Migration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EventContext>();
            optionsBuilder.UseCosmos(
                accountEndpoint: "https://localhost:8081",
                accountKey: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                databaseName: "event-db");
            optionsBuilder.EnableSensitiveDataLogging();

            using (var db = new EventContext(optionsBuilder.Options))
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();

                var globalAzure = new Event("Global Azure",
                    new DateTime(2020, 04, 24, 9, 0, 0),
                    new DateTime(2020, 04, 24, 18, 0, 0));
                db.Add(globalAzure);
                await db.SaveChangesAsync();
            }

        }
    }
}
