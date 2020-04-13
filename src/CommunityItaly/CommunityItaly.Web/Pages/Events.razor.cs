using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Pages
{
	public partial class Events : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }

		public PagedViewModel<EventViewModelReadOnly> EventViewModels { get; set; }
		public int PageSize { get; set; }
		public int PageIndex { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			EventViewModels = await Http.GetEvents(PageSize, PageSize * PageIndex);			
		}

		async Task OnPage(MatPaginatorPageEvent e)
		{
			PageSize = e.PageSize;
			PageIndex = e.PageIndex;
			EventViewModels = await Http.GetEvents(PageSize, PageSize * PageIndex);
		}
	}
}
