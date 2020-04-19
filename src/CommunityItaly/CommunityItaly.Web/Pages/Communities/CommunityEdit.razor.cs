using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages.Communities
{
	public partial class CommunityEdit : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }

		[Parameter]
		public string Id { get; set; }

		public CommunityUpdateViewModel CommunityViewModel { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			CommunityViewModel = AppStore.CommunityEdit;
			AppStore.CommunityImage = null;
		}

		async Task Success()
		{
			await Http.UpdateCommunity(CommunityViewModel).ConfigureAwait(false);
			if (AppStore.CommunityImage != null)
			{
				await Http.UploadCommunityImage(CommunityViewModel.ShortName, AppStore.CommunityImage).ConfigureAwait(false);
			}
		}

		void SelectPerson(PersonUpdateViewModel vm)
		{

		}

		async Task FilesReady(IMatFileUploadEntry[] files)
		{
			var image = files.FirstOrDefault();
			AppStore.CommunityImage = await new FileUploadEntry().FromMat(image).ConfigureAwait(false);
		}
	}
}
