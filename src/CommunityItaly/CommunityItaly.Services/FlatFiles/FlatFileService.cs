using CommunityItaly.Shared.ViewModels;
using FlatFile.Delimited.Implementation;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommunityItaly.Services.FlatFiles
{
	public class FlatFileService : IFlatFileService
	{
		public byte[] GenerateEventFlatFile(ICollection<EventViewModelReadOnly> eventVM)
		{
            byte[] result;
            var layout = new LayoutFactory();
            var factory = new DelimitedFileEngineFactory();
            using (var stream = new MemoryStream())
            {
                var flatFile = factory.GetEngine(layout);
                flatFile.Write<ReportEventModel>(stream, ReportEventModel.FromVM(eventVM));
                result = stream.ToArray();
            }
            return result;
        }
	}

    public class LayoutFactory : DelimitedLayout<ReportEventModel>
    {
        public LayoutFactory()
        {
            this.WithDelimiter(",")
               .WithQuote("\"")
               .WithHeader()
               .WithMember(o => o.Name, s => s.WithName("EventName"))
               .WithMember(o => o.StartDate, s => s.WithName("Event-StardDate"))
               .WithMember(o => o.EndDate, s => s.WithName("Event-EndDate"))
               .WithMember(o => o.BuyTicket, s => s.WithName("TicketURL"))
               .WithMember(o => o.CFP_StartDate, s => s.WithName("CFP-StartDate"))
               .WithMember(o => o.CFP_EndDate, s => s.WithName("CFP-EndDate"))
               .WithMember(o => o.CFP_Url, s => s.WithName("CFP-Url"))
               .WithMember(o => o.Community_Name, s => s.WithName("CommunityName"))
               ;
        }
    }

    public class ReportEventModel
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BuyTicket { get; set; }
        public string CFP_Url { get; set; }
        public DateTime CFP_StartDate { get; set; }
        public DateTime CFP_EndDate { get; set; }
        public string Community_Name { get; set; }

        public static ICollection<ReportEventModel> FromVM(ICollection<EventViewModelReadOnly> vm)
        {
            ICollection<ReportEventModel> l = new List<ReportEventModel>();
            foreach (var item in vm)
            {
                l.Add(new ReportEventModel
                {
                    BuyTicket = item.BuyTicket.ToString(),
                    Name = item.Name,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Community_Name = item.Community.Name,
                    CFP_Url = item.CFP.Url.ToString(),
                    CFP_StartDate = item.CFP.StartDate,
                    CFP_EndDate = item.CFP.EndDate
                });
            }
            return l;
        }
    }
}
