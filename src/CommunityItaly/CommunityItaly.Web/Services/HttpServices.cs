using CommunityItaly.Shared.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Services
{
	public class HttpServices : IHttpServices
	{
		private readonly JsonSerializerOptions JsonOption = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};
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

		#region [Event]
		public async Task<PagedViewModel<EventViewModelReadOnly>> GetEvents(int take, int skip)
		{
			return await Http.GetFromJsonAsync<PagedViewModel<EventViewModelReadOnly>>($"Event?take={take}&skip={skip}").ConfigureAwait(false);
		}

		public async Task<HttpResponseMessage> UpdateEvent(EventUpdateViewModel vm)
		{
			return await Http.PutAsJsonAsync($"Event", vm, JsonOption).ConfigureAwait(false);
		}

		public async Task DeleteEvents(string id)
		{
			await Http.DeleteAsync(new Uri($"/Event?Id={id}")).ConfigureAwait(false);
		}
		#endregion

		#region [Community]
		public async Task<IEnumerable<CommunityUpdateViewModel>> GetCommunitySelect()
		{
			return await Http.GetFromJsonAsync<IEnumerable<CommunityUpdateViewModel>>($"CommunitySelect").ConfigureAwait(false);
		}
		#endregion
	}
}
