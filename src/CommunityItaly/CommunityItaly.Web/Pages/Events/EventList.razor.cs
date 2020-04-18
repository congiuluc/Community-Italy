using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages.Events
{
	public partial class EventList : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }
		[Inject]
		private NavigationManager NavigationManager { get; set; }
		public IEnumerable<EventViewModelReadOnly> EventViewModels { get; set; }
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

		void Edit(EventViewModelReadOnly args)
		{
			AppStore.EventEdit = args;
			NavigationManager.NavigateTo(Routes.EventEdit(AppStore.EventEdit.Id));
		}

		async Task Delete(string id)
		{
			await Http.DeleteEvents(id);
		}

		private async Task LoadDataAsync()
		{
			var pagedViewModel = await Http.GetEvents(PageSize, PageSize * (PageIndex - 1));
			EventViewModels = pagedViewModel.Entities;
			Total = pagedViewModel.Total;
		}

		void Create()
		{
			NavigationManager.NavigateTo(Routes.EventCreate());
		}
	}
}
