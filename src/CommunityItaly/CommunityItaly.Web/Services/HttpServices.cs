using CommunityItaly.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Services
{
	public class HttpServices : IHttpServices
	{
		private readonly HttpClient Http;
		public HttpServices(HttpClient Http)
		{
			this.Http = Http;
			this.Http.BaseAddress = new Uri("http://localhost:7071/api/");
		}

		public async Task<PagedViewModel<EventViewModelReadOnly>> GetEvents(int take, int skip)
		{
			return await Http.GetJsonAsync<PagedViewModel<EventViewModelReadOnly>>($"Event?take={take}&skip={skip}");
		}
	}
}
