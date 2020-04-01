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

		public string GetConfirmationLink(string instanceId, bool approveValue)
		{
			string urlConfirmation = BaseUrl
				.Replace("{instanceId}", instanceId)
				.Replace("{approvevalue}", approveValue.ToString());
			return urlConfirmation;
		}
	}
}
