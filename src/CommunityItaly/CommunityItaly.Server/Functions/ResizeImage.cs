using System.Collections.Generic;
using System.IO;
using CommunityItaly.Server.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CommunityItaly.Server.Functions
{
    public class ResizeImages
    {
        [FunctionName(UploadTasks.ResizeImagePeople)]
        public static void RunPeople([BlobTrigger("people/{name}/original.{blobextension}", Connection = "BlobStorageConnections:ConnectionString")] Stream image, string name, string blobextension,
            [Blob("people/{name}/icon.{blobextension}", FileAccess.Write, Connection = "BlobStorageConnections:ConnectionString")] Stream imageSmall,
            [Blob("people/{name}/medium.{blobextension}", FileAccess.Write, Connection = "BlobStorageConnections:ConnectionString")] Stream imageMedium)
        {
            CreateImages(image, ref imageSmall, ref imageMedium);
        }

        [FunctionName(UploadTasks.ResizeImageEvent)]
        public static void RunEvent([BlobTrigger("events/{name}/original.{blobextension}", Connection = "BlobStorageConnections:ConnectionString")] Stream image, string name, string blobextension,
            [Blob("events/{name}/icon.{blobextension}", FileAccess.Write, Connection = "BlobStorageConnections:ConnectionString")] Stream imageSmall,
            [Blob("events/{name}/medium.{blobextension}", FileAccess.Write, Connection = "BlobStorageConnections:ConnectionString")] Stream imageMedium)
        {
            CreateImages(image, ref imageSmall, ref imageMedium);
        }

        [FunctionName(UploadTasks.ResizeImageCommunity)]
        public static void RunCommunity([BlobTrigger("communities/{name}/original.{blobextension}", Connection = "BlobStorageConnections:ConnectionString")] Stream image, string name, string blobextension,
            [Blob("communities/{name}/icon.{blobextension}", FileAccess.Write)] Stream imageSmall,
            [Blob("communities/{name}/medium.{blobextension}", FileAccess.Write)] Stream imageMedium)
        {
            CreateImages(image, ref imageSmall, ref imageMedium);
        }

        public static (Stream, Stream) CreateImages(Stream image, ref Stream imageSmall, ref Stream imageMedium)
        {
            IImageFormat format;

            using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
            {
                ResizeImage(input, imageSmall, ImageSize.Small, format);
            }

            image.Position = 0;
            using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
            {
                ResizeImage(input, imageMedium, ImageSize.Medium, format);
            }
            return (imageSmall, imageMedium);
        }

        public static void ResizeImage(Image<Rgba32> input, Stream output, ImageSize size, IImageFormat format)
        {
            var dimensions = imageDimensionsTable[size];

            input.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.Primitives.Size(dimensions.Item1, dimensions.Item2),
                Mode = ResizeMode.Pad
            }));
            input.Save(output, format);
        }

        public enum ImageSize { ExtraSmall, Small, Medium }

        private static readonly Dictionary<ImageSize, (int, int)> imageDimensionsTable = new Dictionary<ImageSize, (int, int)>() {
            { ImageSize.Small,  (32, 32) },
            { ImageSize.Medium, (320, 320) }
        };
    }
}
