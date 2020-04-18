using MatBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Stores
{
	public class FileUploadEntry
	{
		public DateTime LastModified { get; set; }
		public string Name { get; set; }
		public long Size { get; set; }
		public string Type { get; set; }
		public MemoryStream StreamData { get; set; }

		public async Task<FileUploadEntry> FromMat(IMatFileUploadEntry entry)
		{
			MemoryStream memory = new MemoryStream();
			await entry.WriteToStreamAsync(memory);
			memory.Position = 0;
			return new FileUploadEntry
			{
				LastModified = entry.LastModified,
				Name = entry.Name,
				Size = entry.Size,
				Type = entry.Type,
				StreamData = memory
			};
		}
	}
}
