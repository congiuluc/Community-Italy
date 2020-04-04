namespace CommunityItaly.Services.Settings
{
	public class AdminConfiguration
	{
		public string Mails { get; set; }
		public string BaseEventUrl { get; set; }
		public string BaseCommunityUrl { get; set; }
		public string BasePersonUrl { get; set; }
		public string BaseArticleUrl { get; set; }


		public string[] GetMails()
		{
			return Mails.Split(',', ';');
		}

		public string GetConfirmationEventLink(string instanceId, bool approveValue)
		{
			string urlConfirmation = BaseEventUrl
				.Replace("{instanceId}", instanceId)
				.Replace("{approvevalue}", approveValue.ToString());
			return urlConfirmation;
		}

		public string GetConfirmationCommunityLink(string instanceId, bool approveValue)
		{
			string urlConfirmation = BaseCommunityUrl
				.Replace("{instanceId}", instanceId)
				.Replace("{approvevalue}", approveValue.ToString());
			return urlConfirmation;
		}

		public string GetConfirmationPersonLink(string instanceId, bool approveValue)
		{
			string urlConfirmation = BasePersonUrl
				.Replace("{instanceId}", instanceId)
				.Replace("{approvevalue}", approveValue.ToString());
			return urlConfirmation;
		}

		public string GetConfirmationArticleLink(string instanceId, bool approveValue)
		{
			string urlConfirmation = BaseArticleUrl
				.Replace("{instanceId}", instanceId)
				.Replace("{approvevalue}", approveValue.ToString());
			return urlConfirmation;
		}
	}
}
