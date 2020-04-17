using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using Microsoft.AspNetCore.Components;
using System;
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
		public string ShortName { get; set; }

		public IEnumerable<CommunityUpdateViewModel> CommunitiesToSelect { get; set; }
		private CommunityUpdateViewModel CommunitySelected { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			CommunitiesToSelect = await Http.GetCommunitySelect().ConfigureAwait(false);
			if (!string.IsNullOrEmpty(ShortName))
			{
				CommunitySelected = CommunitiesToSelect.FirstOrDefault(x => x.ShortName == ShortName);
			}
		}
	}
}
