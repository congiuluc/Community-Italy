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

		public async Task CreateAsync(EventViewModel eventVM)
		{
			Event currentEvent = new Event(eventVM.Name, eventVM.StartDate, eventVM.EndDate);
			//currentEvent.AddCallForSpeaker();
			//currentEvent.AddCommunity();
			await db.Events.AddAsync(currentEvent);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<IList<EventViewModel>> GetAsync(int? take = 10, int? skip = 0)
		{
			throw new NotImplementedException();
		}

		public async Task<Event> GetById(int id)
		{
			throw new NotImplementedException();
		}

		public async Task UpdateAsync(EventViewModel eventVM)
		{
			throw new NotImplementedException();
		}
	}
}
