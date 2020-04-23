using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Components.Communities
{
	public partial class CommunitySelect : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }

		[Parameter]
		public string CommunitySelected { get; set; }

		[Parameter]
		public EventCallback<string> CommunitySelectedChanged { get; set; }

		public List<CommunityUpdateViewModel> CommunitiesToSelect { get; set; } = new List<CommunityUpdateViewModel>();


		protected override async Task OnInitializedAsync()
		{
			CommunitiesToSelect = (List<CommunityUpdateViewModel>)await Http.GetCommunitySelect().ConfigureAwait(false);
			await CommunitySelectedChanged.InvokeAsync(CommunitySelected);
		}
	}
}
