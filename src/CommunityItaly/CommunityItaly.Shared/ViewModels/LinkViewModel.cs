using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Shared.ViewModels
{
	public static class LinkViewModel
	{
		public static  Uri GetImageIcon(Uri original)
		{
			string originalImage = original.ToString();
			return new Uri(originalImage.Replace("original", "icon"));
		}

		public static Uri GetImageMedium(Uri original)
		{
			string originalImage = original.ToString();
			return new Uri(originalImage.Replace("original", "medium"));
		}
	}
}
