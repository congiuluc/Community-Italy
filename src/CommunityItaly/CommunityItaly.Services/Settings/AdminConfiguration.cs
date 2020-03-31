namespace CommunityItaly.Services.Settings
{
	public class AdminConfiguration
	{
		public string Mails { get; set; }
		public string BaseUrl { get; set; }

		public string[] GetMails()
		{
			return Mails.Split(',', ';');
		}

		public string GetConfirmationLink(string instanceId, string eventId)
		{
			string urlConfirmation = BaseUrl
				.Replace("{instanceId}", instanceId)
				.Replace("{eventId}", eventId);
			return urlConfirmation;
		}
	}
}
