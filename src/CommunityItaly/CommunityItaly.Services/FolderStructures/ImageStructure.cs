using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Services.FolderStructures
{
	public static class ImageStructure
	{
		public static string PersonPictureOriginal(string ImageName) => $"people/original/{ImageName}";
		public static string PersonPictureIcon(string ImageName) => $"people/icon/{ImageName}";
		public static string PersonPictureMedium(string ImageName) => $"people/medium/{ImageName}";
	}
}
