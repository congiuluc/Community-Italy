using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages.Communities
{
	public partial class CommunityCreate : ComponentBase	
	{
		[Inject]
		private IHttpServices Http { get; set; }
		public CommunityViewModel CommunityViewModel { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			CommunityViewModel = new CommunityViewModel();
			AppStore.CommunityImage = null;
		}

		async Task Success()
		{
			var result = await Http.CreateCommunity(CommunityViewModel).ConfigureAwait(false);
			if (result.IsSuccessStatusCode)
			{
				string resultId = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
				if (AppStore.EventImage != null)
				{
					string shortname = CommunityViewModel.Name.Replace(" ", "-").ToLowerInvariant();
					await Http.UploadCommunityImage(shortname, AppStore.CommunityImage).ConfigureAwait(false);
				}
				AppStore.AddNotification(new NotificationMessage("Community sottomessa", NotificationMessage.MessageType.Success));
			}
			else
			{
				string error = await result.Content.ReadAsStringAsync();

				AppStore.AddNotification(new NotificationMessage("Errore nella creazione", NotificationMessage.MessageType.Danger));
			}
		}

		async Task FilesReady(IMatFileUploadEntry[] files)
		{
			var image = files.FirstOrDefault();
			AppStore.CommunityImage = await new FileUploadEntry().FromMat(image).ConfigureAwait(false);
		}
	}
}
