using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Shared.ViewModels
{
	public class EventViewModel
	{
        public string Name { get; set; }
        public byte[] Logo { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public Uri BuyTicket { get; }
        public CallForSpeakerViewModel CFP { get; set; }
        public CommunityViewModel Community { get; private set; }
    }
}
