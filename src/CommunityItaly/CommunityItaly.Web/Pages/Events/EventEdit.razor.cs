using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages.Events
{
	public partial class EventEdit : ComponentBase
	{

		[Inject]
		private IHttpServices Http { get; set; }

		[Parameter]
		public string Id { get; set; }

		public EventViewModelReadOnly EventViewModel { get; set; }
		
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			EventViewModel = AppStore.EventEdit;
			if (EventViewModel.CFP == null)
				EventViewModel.CFP = new CallForSpeakerViewModel();
			AppStore.EventImage = null;
		}

		async Task Success()
		{
			var e = new EventViewModel
			{
				Id = EventViewModel.Id,
				StartDate = EventViewModel.StartDate,
				EndDate = EventViewModel.EndDate,
				CFP = string.IsNullOrEmpty(EventViewModel.CFP.Url) ? null : EventViewModel.CFP,
				CommunityName = EventViewModel.Community.ShortName,
				Name = EventViewModel.Name
			};
			if (!string.IsNullOrEmpty(EventViewModel.BuyTicket))
			{
				e.BuyTicket = EventViewModel.BuyTicket;
			}
			var responseUpdate = await Http.UpdateEvent(e).ConfigureAwait(false);
			if (responseUpdate.IsSuccessStatusCode)
			{
				if (AppStore.EventImage != null)
				{
					var responseUploadMessage = await Http.UploadEventImage(e.Id, AppStore.EventImage).ConfigureAwait(false);
					if (!responseUploadMessage.IsSuccessStatusCode)
						AppStore.AddNotification(new NotificationMessage("Errore salvataggio", NotificationMessage.MessageType.Danger));
					else
						AppStore.AddNotification(new NotificationMessage("Evento salvato", NotificationMessage.MessageType.Success));
				}
				else
				{
					AppStore.AddNotification(new NotificationMessage("Evento salvato", NotificationMessage.MessageType.Success));
				}
			}
		}

		async Task FilesReady(IMatFileUploadEntry[] files)
		{
			var image = files.FirstOrDefault();
			AppStore.EventImage = await new FileUploadEntry().FromMat(image).ConfigureAwait(false);
		}
	}
}
