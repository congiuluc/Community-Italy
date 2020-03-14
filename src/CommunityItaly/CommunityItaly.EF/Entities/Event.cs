using System;
using System.Collections.Generic;
using System.Text;

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
        public Uri Logo { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public Uri BuyTicket { get; }
        public CallForSpeaker CFP { get; private set; }
        public bool CFPEnable => CFP != null;
        public Community Community { get; private set; }
        public bool HasCommunity => Community != null;

        public void AddCallForSpeaker(CallForSpeaker cfp)
        {
            CFP = cfp;
        }

        public void AddCommunity(Community community)
        {
            Community = community;
        }
    }

    public class CallForSpeaker
    {
        public Uri Url { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    } 
}
