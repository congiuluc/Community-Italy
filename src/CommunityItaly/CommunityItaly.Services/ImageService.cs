using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace CommunityItaly.Services
{
    public class ImageService : IImageService
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudBlobClient blobClient;

        public ImageService(string storageConnectionString)
        {
            if (!CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            { 
                throw new ArgumentException("Azure Blob Connectionstring is wrong");
            }
            blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task DeleteImageAsync(string blobContainerName, string filename)
        {
            var blobContainer = await CreateOrGetContainerAsync(blobContainerName);
            var blockBlob = blobContainer.GetBlockBlobReference(filename);
            await blockBlob.DeleteAsync();
        }

        public async Task UploadImageAsync(string blobContainerName, string filename, byte[] fileContent)
        {
            var blobContainer = await CreateOrGetContainerAsync(blobContainerName);
            var blockBlob = blobContainer.GetBlockBlobReference(filename);
            await blockBlob.UploadFromByteArrayAsync(fileContent, 0 , fileContent.Length);
        }

        private async Task<CloudBlobContainer> CreateOrGetContainerAsync(string containerName)
        {
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
            if(!await blobContainer.ExistsAsync())
            {
                await blobContainer.CreateAsync();
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Container
                };
                await blobContainer.SetPermissionsAsync(permissions);
            }
            return blobContainer;
        }
    }
}
