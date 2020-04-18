using CommunityItaly.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.Json;
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

            string currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string id = string.Empty;
            using (var db = new EventContext(optionsBuilder.Options))
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();

                Person andreaTosato = new Person("Andrea", "Tosato");
                andreaTosato.SetPicture(new Uri("https://communityitaly.blob.core.windows.net/people/andrea-tosato.jpg"));
                andreaTosato.SetMVPCode("5003336");

                Person marcoZamana = new Person("Marco", "Zamana");
                marcoZamana.SetPicture(new Uri("https://communityitaly.blob.core.windows.net/people/marco-zamana.jpg"));
                marcoZamana.SetMVPCode("5003347");

                var community = new Community("Cloudgen Verona");
                community.SetWebSite(new Uri("https://cloudgen.it"));
                string pathLogoCloudgen = Path.Combine(currentPath, @"Assets\logo-cloudgen.png");
                community.SetLogo(new Uri("https://communityitaly.blob.core.windows.net/communities/cloudgen-verona.png"));
                community.AddManager(andreaTosato);
                community.AddManager(marcoZamana);

                var globalAzure = new Event(Guid.NewGuid().ToString("N"), "Global Azure",
                    new DateTime(2020, 04, 24, 9, 0, 0),
                    new DateTime(2020, 04, 24, 18, 0, 0));
                globalAzure.AddCommunity(community.ToOwned());
                globalAzure.SetBuyTicket(new Uri("https://www.eventbrite.it/e/biglietti-global-azure-2020-88158844477"));
                globalAzure.SetLogo(new Uri("https://communityitaly.blob.core.windows.net/events/globalAzureVerona.jpg"));
                var cfp = new CallForSpeaker(new Uri("https://sessionize.com/global-azure-2020/"), new DateTime(2020, 01, 31), new DateTime(2020, 02, 28));
                globalAzure.SetCallForSpeaker(cfp);

                await db.Events.AddAsync(globalAzure).ConfigureAwait(false);
                await db.SaveChangesAsync();
                id = globalAzure.Id;

                await db.Communities.AddAsync(community).ConfigureAwait(false);
                await db.SaveChangesAsync();
            }


            using (var db = new EventContext(optionsBuilder.Options))
            {
                var e = await db.Events.FindAsync(id).ConfigureAwait(false);
                Console.WriteLine(JsonSerializer.Serialize(e).ToString());
            }
        }
    }

    public static class Utility
    {
        public static byte[] ImageToByte(string path)
        {
            Image img = Image.FromFile(path);
            byte[] arr;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                arr = ms.ToArray();
            }
            return arr;
        }
    }

}
