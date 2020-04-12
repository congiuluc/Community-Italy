using CommunityItaly.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunityItaly.Services.FlatFiles
{
	public interface IFlatFileService
	{
		byte[] GenerateEventFlatFile(ICollection<EventViewModelReadOnly> eventVM);
	}
}
