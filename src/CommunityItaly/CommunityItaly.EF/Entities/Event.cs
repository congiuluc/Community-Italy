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
        public byte[] Logo { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public Uri BuyTicket { get; }
        public CallForSpeaker CFP { get; }
        public bool CFPEnable => CFP != null;
        public Community Community { get; }
        public bool IsCommunity => Community != null;
    }

    public class CallForSpeaker
    {
        public Uri Url { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    } 
}
