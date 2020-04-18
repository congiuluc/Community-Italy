using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Stores;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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

		public async Task<HttpResponseMessage> UpdateEvent(EventViewModel vm)
		{
			return await Http.PutAsJsonAsync($"Event", vm, JsonOption).ConfigureAwait(false);
		}

		public async Task<HttpResponseMessage> CreateEvent(EventViewModel vm)
		{
			return await Http.PostAsJsonAsync($"Event", vm, JsonOption).ConfigureAwait(false);
		}

		public async Task<HttpResponseMessage> UploadEventImage(string id, FileUploadEntry fileToUpload)
		{
			return await UploadImage(id, "EVENT", fileToUpload);
		}

		public async Task DeleteEvents(string id)
		{
			await Http.DeleteAsync(new Uri($"/Event?Id={id}")).ConfigureAwait(false);
		}
		#endregion

		#region [Community]
		public async Task<IEnumerable<CommunityUpdateViewModel>> GetCommunitySelect()
		{
			return await Http.GetFromJsonAsync<IEnumerable<CommunityUpdateViewModel>>("CommunitySelect").ConfigureAwait(false);
		}

		public async Task<HttpResponseMessage> UploadCommunityImage(string id, FileUploadEntry fileToUpload)
		{
			return await UploadImage(id, "COMMUNITY", fileToUpload);
		}
		#endregion

		public async Task<HttpResponseMessage> UploadPersonImage(string id, FileUploadEntry fileToUpload)
		{
			return await UploadImage(id, "PERSON", fileToUpload);
		}


		private async Task<HttpResponseMessage> UploadImage(string id, string type, FileUploadEntry fileToUpload)
		{
			MultipartFormDataContent content = new MultipartFormDataContent();
			
			var fileContent = new StreamContent(fileToUpload.StreamData);
			fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = "\"files\"",
				FileName = "\"" + fileToUpload.Name + "\""
			};
			fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileToUpload.Type);
			content.Add(fileContent);
			return await Http.PostAsync($"UploadImage?id={id}&type={type}", content).ConfigureAwait(false);
		}
	}
}
