using CommunityItaly.Shared.ViewModels;
using System.Collections.Generic;

namespace CommunityItaly.Services.FlatFiles
{
	public interface IFlatFileService
	{
		byte[] GenerateEventFlatFile(ICollection<EventViewModelReadOnly> eventVM);
	}
}
