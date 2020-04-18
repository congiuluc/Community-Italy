using CommunityItaly.Shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace CommunityItaly.Web.Components.People
{
	public partial class PersonModal : ComponentBase
	{
		[Parameter]
		public bool IsOpen { get; set; }
		
		[Parameter]
		public PersonUpdateViewModel PersonData { get; set; }
	}
}
