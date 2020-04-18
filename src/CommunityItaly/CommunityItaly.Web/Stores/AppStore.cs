using CommunityItaly.Shared.ViewModels;
using MatBlazor;

namespace CommunityItaly.Web.Stores
{
	public static class AppStore
	{
		private static IMatToaster _toaster;
		
		public static void SetToaster(IMatToaster toaster)
		{
			if(_toaster == null)
			{
				_toaster = toaster;
			}
		}

		public static void AddNotification(NotificationMessage message)
		{
			_toaster.Add(message.Message, (MatToastType)message.NotificationType);
		}

		public static EventViewModelReadOnly EventEdit { get; set; }
		public static FileUploadEntry EventImage { get; set; }

		public static MatTheme Tema { get; set; } = new MatTheme()
		{
			Primary = MatThemeColors.Orange._500.Value,
			Secondary = MatThemeColors.BlueGrey._500.Value
		};
	}
}
