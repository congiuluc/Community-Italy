using CommunityItaly.Shared.ViewModels;

namespace CommunityItaly.Web.Stores
{
	public interface ICommunityItalyStore
	{
		EventViewModelReadOnly EventEdit { get; set; }
	}
}
