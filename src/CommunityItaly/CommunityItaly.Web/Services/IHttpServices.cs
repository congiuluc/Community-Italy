using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Stores;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Services
{
	public interface IHttpServices
	{
		#region [Events]
		Task<PagedViewModel<EventViewModelReadOnly>> GetEventsConfirmed(int take, int skip);
		Task<PagedViewModel<EventViewModelReadOnly>> GetEvents(int take, int skip);
		Task<HttpResponseMessage> CreateEvent(EventViewModel vm);
		Task<HttpResponseMessage> UpdateEvent(EventViewModel vm);
		Task DeleteEvents(string id);
		Task<HttpResponseMessage> UploadEventImage(string id, FileUploadEntry fileToUpload);
		Task<List<EventViewModelReadOnly>> GetReportConfirmedIntervalledAsync(DateTime startDate, DateTime endDate);
		#endregion
		#region [Community]
		Task<PagedViewModel<CommunityUpdateViewModel>> GetCommunitiesConfirmed(int take, int skip);
		Task<PagedViewModel<CommunityUpdateViewModel>> GetCommunities(int take, int skip);
		Task<IEnumerable<CommunityUpdateViewModel>> GetCommunitySelect();
		Task<HttpResponseMessage> CreateCommunity(CommunityViewModel vm);
		Task<HttpResponseMessage> UpdateCommunity(CommunityUpdateViewModel vm);
		Task<HttpResponseMessage> UploadCommunityImage(string id, FileUploadEntry fileToUpload);
		Task DeleteCommunities(string shortName);
		#endregion

		#region [Person]
		Task<IEnumerable<PersonUpdateViewModel>> GetPersonSelect();
		#endregion

		Task<HttpResponseMessage> GenerateReportEvents(DateTime startDate, DateTime endDate);		

		Task<HttpResponseMessage> UploadPersonImage(string id, FileUploadEntry fileToUpload);
	}
}
