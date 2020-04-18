using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Services
{
	public interface IHttpServices
	{
		#region [Events]
		Task<PagedViewModel<EventViewModelReadOnly>> GetEvents(int take, int skip);
		Task<HttpResponseMessage> CreateEvent(EventViewModel vm);
		Task<HttpResponseMessage> UpdateEvent(EventViewModel vm);
		Task DeleteEvents(string id);
		Task<HttpResponseMessage> UploadEventImage(string id, FileUploadEntry fileToUpload);
		#endregion
		#region [Community]
		Task<IEnumerable<CommunityUpdateViewModel>> GetCommunitySelect();
		Task<HttpResponseMessage> UploadCommunityImage(string id, FileUploadEntry fileToUpload);
		#endregion

		Task<HttpResponseMessage> UploadPersonImage(string id, FileUploadEntry fileToUpload);
	}
}
