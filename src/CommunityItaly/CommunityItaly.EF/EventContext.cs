using CommunityItaly.EF.Entities;
using CommunityItaly.EF.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommunityItaly.EF
{
    // https://docs.microsoft.com/it-it/ef/core/providers/cosmos/?tabs=dotnet-core-cli
    public class EventContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<Article> Articles { get; set; }

        public EventContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
                accountEndpoint: "https://localhost:8081",
                accountKey: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                databaseName: "event-db");
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityBuilder());
            modelBuilder.ApplyConfiguration(new ArticleEntityBuilder());
            modelBuilder.ApplyConfiguration(new CommunityEntityBuilder());
            modelBuilder.ApplyConfiguration(new PersonEntityBuilder());
        }
    }

    //public class EventContextFactory : IDesignTimeDbContextFactory<EventContext>
    //{
    //    public EventContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<EventContext>();
    //        optionsBuilder.UseCosmos(
    //            accountEndpoint: "https://localhost:8081",
    //            accountKey: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    //            databaseName: "event-db");
    //        optionsBuilder.EnableSensitiveDataLogging();


    //        return new EventContext(optionsBuilder.Options);
    //    }
    //}

}
