using CommunityItaly.EF.Entities;
using CommunityItaly.Services;
using CommunityItaly.Services.FolderStructures;
using CommunityItaly.Services.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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


            var blobStorageConnection = Options.Create<BlobStorageConnections>(new BlobStorageConnections 
            { 
                ConnectionString = "DefaultEndpointsProtocol=https;AccountName=communityitaly;AccountKey=1pNldDHkUAIwrHYOeT2p5c98BKzwqVyeqFr6JqUYc/luLu/66pBfHSIxV4Zq8Ewg7N6YzzuB8oh1/47RJ739Vw==;EndpointSuffix=core.windows.net" 
            });
            FileService fileService = new FileService(blobStorageConnection);
            string currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string id = string.Empty;
            using (var db = new EventContext(optionsBuilder.Options))
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();

                string andreatosatoid = Guid.NewGuid().ToString("N");
                Person andreaTosato = new Person(andreatosatoid, "Andrea", "Tosato");
                andreaTosato.SetMVPCode("5003336");
                string pathAndreaTosato = Path.Combine(currentPath, @"Assets\andrea-tosato\original.jpg");
                var biAndreaTosato = ImageStructure.PersonPictureOriginal(andreatosatoid, ".jpg");
                var imageUriTosato = await fileService.UploadImageAsync(biAndreaTosato.BlobContainerName, biAndreaTosato.FileName, Utility.ImageToByte(pathAndreaTosato));
                andreaTosato.SetPicture(imageUriTosato);
                andreaTosato.SetConfirmation(true);


                string marcozamanaid = Guid.NewGuid().ToString("N");
                Person marcoZamana = new Person(marcozamanaid, "Marco", "Zamana");
                marcoZamana.SetMVPCode("5003347");
                string pathMarcoZamana = Path.Combine(currentPath, @"Assets\marco-zamana\original.jpg");
                var biZamana = ImageStructure.PersonPictureOriginal(marcozamanaid, ".jpg");
                var imageUriZamana = await fileService.UploadImageAsync(biZamana.BlobContainerName, biZamana.FileName, Utility.ImageToByte(pathMarcoZamana));
                marcoZamana.SetPicture(imageUriZamana);
                marcoZamana.SetConfirmation(true);


                var community = new Community("cloudgen-verona");
                community.SetWebSite(new Uri("https://cloudgen.it"));
                string pathLogoCloudgen = Path.Combine(currentPath, @"Assets\cloudgen-verona\original.png");
                var biCloudgen = ImageStructure.CommunityPictureOriginal("cloudgen-verona", ".png");
                var imageUriCloudgen = await fileService.UploadImageAsync(biCloudgen.BlobContainerName, biCloudgen.FileName, Utility.ImageToByte(pathLogoCloudgen));
                community.SetLogo(imageUriCloudgen);
                community.AddManager(andreaTosato);
                community.AddManager(marcoZamana);
                community.SetConfirmation(true);

                string eventid = Guid.NewGuid().ToString("N");
                var globalAzure = new Event(eventid, "Global Azure",
                    new DateTime(2020, 04, 24, 9, 0, 0),
                    new DateTime(2020, 04, 24, 18, 0, 0));
                globalAzure.AddCommunity(community.ToOwned());
                globalAzure.SetBuyTicket(new Uri("https://www.eventbrite.it/e/biglietti-global-azure-2020-88158844477"));
                string pathGlobalAzure = Path.Combine(currentPath, @"Assets\global-azure\original.png");
                var biGlobalAzure = ImageStructure.EventPictureOriginal(eventid, ".png");
                var imageUriAzure = await fileService.UploadImageAsync(biGlobalAzure.BlobContainerName, biGlobalAzure.FileName, Utility.ImageToByte(pathGlobalAzure));
                globalAzure.SetLogo(imageUriAzure);
                var cfp = new CallForSpeaker(new Uri("https://sessionize.com/global-azure-2020/"), new DateTime(2020, 01, 31), new DateTime(2020, 02, 28));
                globalAzure.SetCallForSpeaker(cfp);
                globalAzure.SetConfirmation(true);

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
