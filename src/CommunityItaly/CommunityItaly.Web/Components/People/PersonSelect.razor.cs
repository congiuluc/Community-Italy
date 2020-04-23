using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Components.People
{
	public partial class PersonSelect : ComponentBase
	{
		[Inject]
		private IHttpServices Http { get; set; }

		[Parameter]
		public PersonUpdateViewModel PersonSelected { get; set; }

		[Parameter]
		public EventCallback<PersonUpdateViewModel> PersonSelectedChanged { get; set; }

		public string ImageUrl(Uri uri)
		{
			return LinkViewModel.GetImageIcon(uri).ToString();		
		}

		public IReadOnlyList<PersonUpdateViewModel> PeopleToSelect { get; set; } = new List<PersonUpdateViewModel>();


		protected override async Task OnInitializedAsync()
		{
			PeopleToSelect = (IReadOnlyList<PersonUpdateViewModel>)await Http.GetPersonSelect().ConfigureAwait(false);
			await PersonSelectedChanged.InvokeAsync(PersonSelected);
		}
	}
}
