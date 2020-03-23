using System;

namespace CommunityItaly.EF.Entities
{
    public class Event
    {
        public Event(string name, DateTime startDate, DateTime endDate)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public string Id { get; }
        public string Name { get; }
        public Uri Logo { get; private set; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public Uri BuyTicket { get; private set; }
        public CallForSpeaker CFP { get; private set; }
        public bool CFPEnable => CFP != null;
        public CommunityOwned Community { get; private set; }
        public bool HasCommunity => Community != null;
        public bool Confirmed { get; private set; }

        public void SetConfirmation(bool confirmation) => Confirmed = confirmation;

        public void AddCallForSpeaker(CallForSpeaker cfp)
        {
            CFP = cfp;
        }

        public void AddCommunity(CommunityOwned community)
        {
            Community = community;
        }

        public void SetLogo(Uri logoUrl)
        {
            Logo = logoUrl;
        }

        public void SetBuyTicket(Uri ticketUrl)
        {
            BuyTicket = ticketUrl;
        }
    }

    public class CallForSpeaker
    {
        public CallForSpeaker(Uri url, DateTime startDate, DateTime endDate)
        {
            Url = url;
            StartDate = startDate;
            EndDate = endDate;
        }
        public Uri Url { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    } 
}
