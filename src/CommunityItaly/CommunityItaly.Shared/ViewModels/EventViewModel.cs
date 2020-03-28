using System;

namespace CommunityItaly.Shared.ViewModels
{
	public class EventViewModel
	{
        public string Name { get; set; }
        public Uri Logo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Uri BuyTicket { get; set; }
        public CallForSpeakerViewModel CFP { get; set; }
        public string CommunityName { get; set; }
    }

    public class EventUpdateViewModel : EventViewModel
    {
        public string Id { get; set; }
        public bool Confirmation { get; set; }
    }

    public class EventViewModelReadOnly
    {
        public string Name { get; set; }
        public Uri Logo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Uri BuyTicket { get; set; }
        public CallForSpeakerViewModel CFP { get; set; }
        public CommunityViewModel Community { get; set; }
    }
}
