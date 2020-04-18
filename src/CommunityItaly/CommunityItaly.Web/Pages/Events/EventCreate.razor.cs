using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages.Events
{
	public partial class EventCreate : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }
		public EventViewModel EventViewModel { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			EventViewModel = new EventViewModel()
			{
				Id = Guid.NewGuid().ToString("N"),
				CFP = new CallForSpeakerViewModel()
			};
			AppStore.EventImage = null;
		}

		async Task Success()
		{
			var result = await Http.CreateEvent(EventViewModel).ConfigureAwait(false);
			if (result.IsSuccessStatusCode)
			{
				string resultId = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
				if (AppStore.EventImage != null)
				{
					await Http.UploadEventImage(EventViewModel.Id, AppStore.EventImage).ConfigureAwait(false);
				}
				AppStore.AddNotification(new NotificationMessage()
				{
					Message = "Evento sottomesso",
					NotificationType = NotificationMessage.MessageType.Success
				});
			}
			else
			{
				string error = await result.Content.ReadAsStringAsync();

				AppStore.AddNotification(new NotificationMessage()
				{
					Message = "Errore nella creazione",
					NotificationType = NotificationMessage.MessageType.Danger
				});
			}
		}

		async Task FilesReady(IMatFileUploadEntry[] files)
		{
			var image = files.FirstOrDefault();
			AppStore.EventImage = await new FileUploadEntry().FromMat(image).ConfigureAwait(false);
		}
	}
}
