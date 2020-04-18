using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
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
			AppStore.EventImage = null;
		}

		async Task Success()
		{
			var e = new EventViewModel
			{
				Id = EventViewModel.Id,
				StartDate = EventViewModel.StartDate,
				EndDate = EventViewModel.EndDate,
				CFP = EventViewModel.CFP,
				CommunityName = EventViewModel.Community.ShortName,
				Name = EventViewModel.Name
			};
			if (!string.IsNullOrEmpty(EventViewModel.BuyTicket))
			{
				e.BuyTicket = EventViewModel.BuyTicket;
			}
			await Http.UpdateEvent(e).ConfigureAwait(false);
			if(AppStore.EventImage != null)
			{
				await Http.UploadEventImage(e.Id, AppStore.EventImage).ConfigureAwait(false);
			}
		}

		async Task FilesReady(IMatFileUploadEntry[] files)
		{
			var image = files.FirstOrDefault();
			AppStore.EventImage = await new FileUploadEntry().FromMat(image).ConfigureAwait(false);
		}
	}
}
