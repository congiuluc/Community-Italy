using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages.Events
{
	public partial class EventEdit : ComponentBase
	{
		[Inject]
		public ICommunityItalyStore Store { get; set; }

		[Inject]
		private IHttpServices Http { get; set; }

		[Parameter]
		public string Id { get; set; }

		public EventViewModelReadOnly EventViewModel { get; set; }
		
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			EventViewModel = Store.EventEdit;
			Store.EventImage = null;
		}

		async Task Success()
		{
			var e = new EventUpdateViewModel
			{
				Id = EventViewModel.Id,
				StartDate = EventViewModel.StartDate,
				EndDate = EventViewModel.EndDate,
				CommunityName = EventViewModel.Community.ShortName,
				Name = EventViewModel.Name
			};
			await Http.UpdateEvent(e).ConfigureAwait(false);

			// TODO: Upload Image
			if(Store.EventImage != null)
			{
				await Http.UploadEventImage(e.Id, Store.EventImage.StreamData).ConfigureAwait(false);
			}
		}

		async Task FilesReady(IMatFileUploadEntry[] files)
		{
			var image = files.FirstOrDefault();
			Store.EventImage = await new FileUploadEntry().FromMat(image).ConfigureAwait(false);
		}
	}
}
