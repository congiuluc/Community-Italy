namespace CommunityItaly.Services.Settings
{
	public class SendGridConnections
	{
		public string ApiKey { get; set; }
		public string TemplateEventId { get; set; }
		public string TemplateCommunityId { get; set; }
		public string TemplatePersonId { get; set; }
		public string TemplateArticleId { get; set; }

		public string From { get; set; }
	}
}
