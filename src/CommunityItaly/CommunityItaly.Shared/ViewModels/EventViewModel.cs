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
        public bool Confirmed { get; set; }

        public static EventUpdateViewModel Create(EventViewModel vm)
        {
            return new EventUpdateViewModel
            {
                Name = vm.Name,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                BuyTicket = vm.BuyTicket,
                CFP = vm.CFP,
                CommunityName = vm.CommunityName
            };
        }
    }

    public class EventViewModelReadOnly
    {
        public string Name { get; set; }
        public Uri Logo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BuyTicket { get; set; }
        public CallForSpeakerViewModel CFP { get; set; }
        public CommunityUpdateViewModel Community { get; set; }
    }
}
