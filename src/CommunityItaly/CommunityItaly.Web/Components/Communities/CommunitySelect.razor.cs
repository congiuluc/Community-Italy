using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Components.Communities
{
	public partial class CommunitySelect : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }

		[Parameter]
		public CommunityUpdateViewModel CommunitySelected { get; set; }
		[Parameter]
		public EventCallback<CommunityUpdateViewModel> CommunitySelectedChanged { get; set; }

		public IReadOnlyList<CommunityUpdateViewModel> CommunitiesToSelect { get; set; } = new List<CommunityUpdateViewModel>();


		protected override async Task OnInitializedAsync()
		{
			CommunitiesToSelect = (IReadOnlyList<CommunityUpdateViewModel>)await Http.GetCommunitySelect().ConfigureAwait(false);
		}
	}
}
