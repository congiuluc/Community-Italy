using CommunityItaly.Shared.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Services
{
	public class HttpServices : IHttpServices
	{
		private readonly HttpClient Http;
		public HttpServices(HttpClient Http, IConfiguration configuration)
		{
			this.Http = Http;
			var baseUri = new Uri(configuration["BaseUrl"]);
			if (this.Http.BaseAddress != baseUri)
			{
				this.Http.BaseAddress = baseUri;
			}
		}

		public async Task<PagedViewModel<EventViewModelReadOnly>> GetEvents(int take, int skip)
		{
			return await Http.GetFromJsonAsync<PagedViewModel<EventViewModelReadOnly>>($"Event?take={take}&skip={skip}");
		}
	}
}
