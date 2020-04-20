using CommunityItaly.Shared;
using CommunityItaly.Shared.ViewModels;
using CommunityItaly.Web.Services;
using CommunityItaly.Web.Stores;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

		public List<EventViewModelReadOnly> ReportLists { get; set; } = new List<EventViewModelReadOnly>();
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
			if (Search.StartDate != null && Search.EndDate != null && Search.StartDate > Search.EndDate)
			{
				AppStore.AddNotification(new NotificationMessage("Le date non sono corrette", NotificationMessage.MessageType.Warning));
			}
			else
			{
				ReportLists = await Http.GetReportConfirmedIntervalledAsync(Search.StartDate.ToLocalTime(), Search.EndDate.ToLocalTime().EndOfDay());
			}
		}

		async Task Export()
		{
			var response = await Http.GenerateReportEvents(Search.StartDate, Search.EndDate);
			if(response.IsSuccessStatusCode)
			{
				AppStore.AddNotification(new NotificationMessage("Il report verrà inviato via mail agli amministratori", NotificationMessage.MessageType.Info));
			}
			else
			{
				AppStore.AddNotification(new NotificationMessage("Errore generazione del report", new Exception(response.ReasonPhrase)));
			}
		}

		void OpenManagers(PersonUpdateViewModel person)
		{
			ManagerSelected = person;
			ManagerIsOpen = true;
		}

		void ManagerOnConfirm(bool isOpen)
		{
			ManagerIsOpen = isOpen;
			ManagerSelected = new PersonUpdateViewModel();
		}
	}

	public class SearchReport
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
