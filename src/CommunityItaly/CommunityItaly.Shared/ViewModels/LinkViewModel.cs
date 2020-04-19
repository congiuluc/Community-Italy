using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Shared.ViewModels
{
	public static class LinkViewModel
	{
		public static  Uri GetImageIcon(Uri original)
		{
			if (original != null)
			{
				string originalImage = original.ToString();
				return new Uri(originalImage.Replace("original", "icon"));
			}
			return new Uri("https://communityitaly.blob.core.windows.net/default/icon.png");
		}

		public static Uri GetImageMedium(Uri original)
		{
			if(original != null)
			{
				string originalImage = original.ToString();
				return new Uri(originalImage.Replace("original", "medium"));
			}
			return new Uri("https://communityitaly.blob.core.windows.net/default/medium.png");
		}
	}
}
