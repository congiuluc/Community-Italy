using CommunityItaly.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Services
{
	public interface IHttpServices
	{
		#region [Events]
		Task<PagedViewModel<EventViewModelReadOnly>> GetEvents(int take, int skip);
		Task<HttpResponseMessage> UpdateEvent(EventUpdateViewModel vm);
		Task DeleteEvents(string id);
		#endregion
		#region [Community]
		Task<IEnumerable<CommunityUpdateViewModel>> GetCommunitySelect();
		#endregion
	}
}
