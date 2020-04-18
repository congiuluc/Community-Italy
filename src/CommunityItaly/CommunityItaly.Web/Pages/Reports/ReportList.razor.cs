using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityItaly.Web.Pages.Reports
{
	public partial class ReportList : ComponentBase
	{
		[Inject]
		IHttpServices Http { get; set; }

		public SearchReport Search { get; set; }

		public List<CommunityUpdateViewModel> ReportLists { get; set; } = new List<CommunityUpdateViewModel>();

		bool ManagerIsOpen { get; set; } = false;
		PersonUpdateViewModel ManagerSelected { get; set; } = new PersonUpdateViewModel();

		protected override void OnInitialized()
		{
			Search = new SearchReport
			{
				StartDate = DateTime.Now.AddMonths(-3),
				EndDate = DateTime.Now
			};
			base.OnInitialized();

		}

		async Task SearchEvents()
		{

		}

		void OpenManagers(PersonUpdateViewModel person)
		{
			ManagerSelected = person;
			ManagerIsOpen = true;
		}
	}

	public class SearchReport
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
