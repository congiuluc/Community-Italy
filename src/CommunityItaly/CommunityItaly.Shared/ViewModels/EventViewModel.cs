using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Shared.ViewModels
{
	public class EventViewModel
	{
        public string Name { get; set; }
        public byte[] Logo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Uri BuyTicket { get; set; }
        public CallForSpeakerViewModel CFP { get; set; }
        public string CommunityName { get; set; }
    }
}
