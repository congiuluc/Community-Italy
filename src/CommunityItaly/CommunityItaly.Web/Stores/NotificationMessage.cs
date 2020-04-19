using System;

namespace CommunityItaly.Web.Stores
{
	public class NotificationMessage
	{
		public NotificationMessage(string message, Exception ex) : 
			this(message, MessageType.Danger)
		{
			Exception = ex;
		}

		public NotificationMessage(string message, MessageType notificationType)
		{
			Message = message;
			NotificationType = notificationType;
		}

		public string Message { get; set; }
		public Exception Exception { get; set; }
		public MessageType NotificationType { get; set; }
		public enum MessageType
		{
			Danger = 0,
			Dark = 1,
			Info = 2,
			Light = 3,
			Link = 4,
			Primary = 5,
			Secondary = 6,
			Success = 7,
			Warning = 8
		}
	}
}
