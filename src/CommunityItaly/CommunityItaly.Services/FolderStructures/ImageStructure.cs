namespace CommunityItaly.Services.FolderStructures
{
	public static class ImageStructure
	{
		public static string PersonPictureOriginal(string Id) => $"people/original/{Id}";
		public static string PersonPictureIcon(string Id) => $"people/icon/{Id}";
		public static string PersonPictureMedium(string Id) => $"people/medium/{Id}";

		public static string EventPictureOriginal(string Id) => $"events/original/{Id}";
		public static string EventPictureIcon(string Id) => $"events/icon/{Id}";
		public static string EventPictureMedium(string Id) => $"events/medium/{Id}";

		public static string CommunityPictureOriginal(string Id) => $"communities/original/{Id}";
		public static string CommunityPictureIcon(string Id) => $"communities/icon/{Id}";
		public static string CommunityPictureMedium(string Id) => $"communities/medium/{Id}";
	}
}
