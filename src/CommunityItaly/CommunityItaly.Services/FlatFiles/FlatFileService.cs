using CommunityItaly.Shared.ViewModels;
using FlatFile.Delimited;
using FlatFile.Delimited.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
                var flatFile = factory.GetEngine(layout.GetLayout());
                flatFile.Write<EventViewModelReadOnly>(stream, eventVM);
                result = stream.ToArray();
            }
            return result;
        }
	}

    public class LayoutFactory
    {
        public IDelimitedLayout<EventViewModelReadOnly> GetLayout()
        {
            IDelimitedLayout<EventViewModelReadOnly> layout = new DelimitedLayout<EventViewModelReadOnly>()
                .WithDelimiter(";")
                .WithQuote("\"")
                .WithMember(o => o.Name, s => s.WithName("EventName"))
                .WithMember(o => o.StartDate, s => s.WithName("Event-StardDate"))
                .WithMember(o => o.EndDate, s => s.WithName("Event-EndDate"))
                .WithMember(o => o.BuyTicket.ToString(), s => s.WithName("TicketURL"))
                .WithMember(o => o.CFP.StartDate, s => s.WithName("CFP-StartDate"))
                .WithMember(o => o.CFP.EndDate, s => s.WithName("CFP-EndDate"))
                .WithMember(o => o.CFP.Url, s => s.WithName("CFP-Url"))
                .WithMember(o => o.Community.Name, s => s.WithName("CommunityName"));

            return layout;
        }
    }
}
