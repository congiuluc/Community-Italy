using CommunityItaly.EF;
using CommunityItaly.EF.Entities;
using CommunityItaly.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunityItaly.Services.DataAccess
{
	public class EventService : IEventServices
	{
		private readonly EventContext db;

		public EventService(EventContext db)
		{
			this.db = db;
		}

		public async Task<string> CreateAsync(EventViewModel eventVM)
		{
			
			Event currentEvent = new Event(eventVM.Name, eventVM.StartDate, eventVM.EndDate);
			if(eventVM.CFP != null)
			{
				currentEvent.AddCallForSpeaker(new CallForSpeaker(eventVM.CFP.Url, eventVM.CFP.StartDate, eventVM.CFP.EndDate));
			}
			if(!string.IsNullOrEmpty(eventVM.CommunityName))
			{
				Community community = await db.Communities.FindAsync(eventVM.CommunityName);
				if(community != null)
				{
					currentEvent.AddCommunity(community);
				}
			}			
			await db.Events.AddAsync(currentEvent);
			await db.SaveChangesAsync().ConfigureAwait(false);
			return currentEvent.Id;
		}

		public async Task DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<IList<EventViewModel>> GetAsync(int? take = 10, int? skip = 0)
		{
			throw new NotImplementedException();
		}

		public async Task<EventViewModel> GetById(string id)
		{
			throw new NotImplementedException();
		}

		public async Task UpdateAsync(EventViewModel eventVM)
		{
			throw new NotImplementedException();
		}
	}
}
