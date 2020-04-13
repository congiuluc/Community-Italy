using CommunityItaly.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Services
{
	public interface IHttpServices
	{
		Task<PagedViewModel<EventViewModelReadOnly>> GetEvents(int take, int skip);
	}
}
