using CommunityItaly.Shared.ViewModels;

namespace CommunityItaly.Web.Stores
{
	public class CommunityItalyStore : ICommunityItalyStore
	{
		public EventViewModelReadOnly EventEdit { get; set; }
	}
}
