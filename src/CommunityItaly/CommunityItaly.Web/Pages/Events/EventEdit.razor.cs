using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
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
		List<IMatFileUploadEntry> FileUploadEntries { get; set; } = new List<IMatFileUploadEntry>();
		public CommunityUpdateViewModel CommunitySelected { get; set; }
		//public EventHandler<CommunityUpdateViewModel> OnCommunityChanged;

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			EventViewModel = Store.EventEdit;
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
			await Http.UpdateEvent(e);

			// TODO: Upload Image
		}

		void FilesReady(IMatFileUploadEntry[] files)
		{
			FileUploadEntries.AddRange(files);
		}
	}
}
