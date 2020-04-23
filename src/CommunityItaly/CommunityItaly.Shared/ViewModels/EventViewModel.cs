using System;

namespace CommunityItaly.Shared.ViewModels
{
	public class EventViewModel
	{
        public string Id { get; set; }
        public bool Confirmed { get; set; }
        public string Name { get; set; }
        public Uri Logo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BuyTicket { get; set; }
        public CallForSpeakerViewModel CFP { get; set; }
        public string CommunityName { get; set; }
        public EventViewModel DeepCopy()
        {
            return new EventViewModel
            {
                Id = Id,
                CommunityName = CommunityName,
                Confirmed = Confirmed,
                BuyTicket = BuyTicket,
                StartDate = StartDate,
                EndDate = EndDate,
                Name = Name,
                Logo = Logo,
                CFP = string.IsNullOrEmpty(CFP.Url) ?
                null :
                new CallForSpeakerViewModel
                {
                    Url = CFP.Url,
                    StartDate = CFP.StartDate,
                    EndDate = CFP.EndDate
                }
            };
        }
    }

    public class EventViewModelReadOnly
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Uri Logo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BuyTicket { get; set; }
        public CallForSpeakerViewModel CFP { get; set; } = new CallForSpeakerViewModel();
        public CommunityUpdateViewModel Community { get; set; } = new CommunityUpdateViewModel();
    }
}
