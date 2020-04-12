using System;
using System.IO;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface IFileService
    {
        Task<Uri> UploadImageAsync(string blobContainerName, string filename, byte[] fileContent);
        Task DeleteImageAsync(string blobContainerName, string filename);
        Task UploadReport(string blobContainerName, string filename, byte[] fileContent);
        Task<MemoryStream> DownloadReport(string blobContainerName, string filename);
    }
}
