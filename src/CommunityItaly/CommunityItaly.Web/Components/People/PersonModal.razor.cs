using CommunityItaly.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Components.People
{
	public partial class PersonModal : ComponentBase
	{
		[Parameter]
		public bool IsOpen { get; set; }
		
		[Parameter]
		public PersonUpdateViewModel PersonData { get; set; }

		[Parameter]
		public EventCallback<bool> OnConfirm { get; set; }

		public string MVP_Url { get; set; }

		protected override void OnInitialized()
		{
			if (!string.IsNullOrEmpty(PersonData.MVP_Code))
				MVP_Url = $"https://mvp.microsoft.com/it-it/PublicProfile/{PersonData.MVP_Code}";
		}

		async Task Confirm()
		{
			IsOpen = !IsOpen;
			await OnConfirm.InvokeAsync(IsOpen);
		}
	}
}
