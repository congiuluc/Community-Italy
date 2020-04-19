using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages.Communities
{
	public partial class CommunitiyList : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }
		[Inject]
		private NavigationManager NavigationManager { get; set; }
		public IEnumerable<CommunityUpdateViewModel> CommunityViewModels { get; set; }
		public int PageSize { get; set; } = 10;
		public int PageIndex { get; set; } = 1;
		public int Total { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await LoadDataAsync();
			await base.OnInitializedAsync();
		}

		async Task OnPage(MatPaginatorPageEvent e)
		{
			PageSize = e.PageSize;
			PageIndex = e.PageIndex;
			await LoadDataAsync();
		}

		void Edit(CommunityUpdateViewModel args)
		{
			AppStore.CommunityEdit = args;
			NavigationManager.NavigateTo(Routes.CommunityEdit(AppStore.CommunityEdit.ShortName));
		}

		async Task Delete(string id)
		{
			await Http.DeleteEvents(id);
		}

		private async Task LoadDataAsync()
		{
			var pagedViewModel = await Http.GetCommunitiesConfirmed(PageSize, PageSize * (PageIndex - 1));
			CommunityViewModels = pagedViewModel.Entities;
			Total = pagedViewModel.Total;
		}

		void Create()
		{
			NavigationManager.NavigateTo(Routes.CommunityCreate());
		}
	}
}
