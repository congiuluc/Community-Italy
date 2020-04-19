using System;

namespace CommunityItaly.Shared.ViewModels
{
	public class ReportFilterViewModel
    { 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public void CheckDate()
        {
            if (StartDate == default || StartDate < DateTime.UtcNow.AddYears(-2))
            {
                StartDate = DateTime.UtcNow.AddMonths(-3).StartOfMonth();
            }

            if (EndDate == default || EndDate < DateTime.UtcNow.AddYears(-2).AddMonths(3))
            {
                EndDate = DateTime.UtcNow.EndOfMonth();
            }

            if (EndDate.CompareTo(StartDate) < 0)
            {
                EndDate = StartDate.AddMonths(3).EndOfMonth();
            }
        }
    }

}
