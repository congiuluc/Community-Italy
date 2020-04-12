using System;

namespace CommunityItaly.Services.FolderStructures
{
	public static class ImageStructure
	{
		public static BlobInformation PersonPictureOriginal(string id, string extension) => BlobInformation.Create($"people/{id}/original{extension}");
		public static BlobInformation PersonPictureIcon(string id, string extension) => BlobInformation.Create($"people/{id}/icon{extension}");
		public static BlobInformation PersonPictureMedium(string id, string extension) => BlobInformation.Create($"people/{id}/medium{extension}");

		public static BlobInformation EventPictureOriginal(string id, string extension) => BlobInformation.Create($"events/{id}/original{extension}");
		public static BlobInformation EventPictureIcon(string id, string extension) => BlobInformation.Create($"events/{id}/icon{extension}");
		public static BlobInformation EventPictureMedium(string id, string extension) => BlobInformation.Create($"events/{id}/medium{extension}");

		public static BlobInformation CommunityPictureOriginal(string id, string extension) => BlobInformation.Create($"communities/{id}/original{extension}");
		public static BlobInformation CommunityPictureIcon(string id, string extension) => BlobInformation.Create($"communities/{id}/icon{extension}");
		public static BlobInformation CommunityPictureMedium(string id, string extension) => BlobInformation.Create($"communities/{id}/medium{extension}");
	}

	public class BlobInformation
	{
		public static BlobInformation Create(string fullPath)
		{
			string[] segments = fullPath.Split('/');
			if(segments.Length == 3)
			{
				return new BlobInformation(segments[0], segments[1], segments[2]);
			}
			throw new System.ArgumentException("Full Path is not in correct format");
		}
		public BlobInformation(string blobContainerName, string path, string imageName)
		{
			BlobContainerName = blobContainerName;
			Path = path;
			ImageName = imageName;
		}

		public string BlobContainerName { get; set; }
		public string ImageName { get; set; }
		public string Path { get; set; }
		public string FullPath => $"{BlobContainerName}/{FileName}";
		public string FileName => $"{Path}/{ImageName}";
	}


	public static class ReportStructure
	{
		public static ReportInformation Report(string fileName) => ReportInformation.Create($"reports/{DateTime.Now.Year}/{fileName}");
	}

	public class ReportInformation
	{
		public static ReportInformation Create(string fullPath)
		{
			string[] segments = fullPath.Split('/');
			if (segments.Length == 3)
			{
				return new ReportInformation(segments[0], segments[1], segments[2]);
			}
			throw new System.ArgumentException("Full Path is not in correct format");
		}
		public ReportInformation(string blobContainerName, string year, string filename)
		{
			BlobContainerName = blobContainerName;
			Year = year;
			FileName = filename;
		}

		public string BlobContainerName { get; set; }
		public string FileName { get; set; }
		public string Year { get; set; }
		public string FullPath => $"{BlobContainerName}/{PathName}";
		public string PathName => $"{Year}/{FileName}";
	}
}
