using System;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface IImageService
    {
        Task<Uri> UploadImageAsync(string blobContainerName, string filename, byte[] fileContent);
        Task DeleteImageAsync(string blobContainerName, string filename);
    }
}
