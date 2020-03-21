using CommunityItaly.EF;
using CommunityItaly.EF.Entities;
using CommunityItaly.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public class EventService : IEventService
	{
		private readonly EventContext db;

		public EventService(EventContext db)
		{
			this.db = db;
		}

		public async Task<string> CreateAsync(EventViewModel eventVM)
		{
			Event currentEvent = new Event(eventVM.Name, eventVM.StartDate, eventVM.EndDate);
			currentEvent.SetLogo(eventVM.Logo);
			currentEvent.SetBuyTicket(eventVM.BuyTicket);

			if(eventVM.CFP != null)
			{
				currentEvent.AddCallForSpeaker(new CallForSpeaker(eventVM.CFP.Url, eventVM.CFP.StartDate, eventVM.CFP.EndDate));
			}
			if (!string.IsNullOrEmpty(eventVM.CommunityName))
			{
				Community community = await db.Communities.FindAsync(eventVM.CommunityName);
				if (community != null)
				{
					currentEvent.AddCommunity(community);
				}
			}
			await db.Events.AddAsync(currentEvent);
			await db.SaveChangesAsync().ConfigureAwait(false);
			return currentEvent.Id;
		}

		public async Task DeleteAsync(string id)
		{
			var currentEvent = await db.Events.FindAsync(id).ConfigureAwait(false);
			db.Events.Remove(currentEvent);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<PagedViewModel<EventViewModelReadOnly>> GetAsync(int? take = 10, int? skip = 0)
		{
			int totalElement = await db.Events.CountAsync().ConfigureAwait(false);

			var resultList = await db.Events
				.Skip(skip.Value)
				.Take(take.Value)
				.ToListAsync()
				.ConfigureAwait(false);

			var result = resultList
				.Select(currentEvent => new EventViewModelReadOnly
				{
					Name = currentEvent.Name,
					Logo = currentEvent.Logo,
					StartDate = currentEvent.StartDate,
					EndDate = currentEvent.EndDate,
					BuyTicket = currentEvent.BuyTicket,
					CFP = new CallForSpeakerViewModel
					{
						Url = currentEvent.CFP.Url,
						StartDate = currentEvent.CFP.StartDate,
						EndDate = currentEvent.CFP.EndDate
					},
					Community = new CommunityViewModel
					{
						Name = currentEvent.Community.Name,
						Logo = currentEvent.Community.Logo,
						WebSite = currentEvent.Community.WebSite
					}
				});

			return new PagedViewModel<EventViewModelReadOnly>
			{
				CurrentPage = take.Value * skip.Value,
				Total = totalElement,
				Entities = result
			};
		}

		public async Task<EventViewModelReadOnly> GetById(string id)
		{
			var currentEvent = await db.Events.FindAsync(id).ConfigureAwait(false);
			EventViewModelReadOnly eventVM = new EventViewModelReadOnly
			{
				Name = currentEvent.Name,
				Logo = currentEvent.Logo,
				StartDate = currentEvent.StartDate,
				EndDate = currentEvent.EndDate,
				BuyTicket = currentEvent.BuyTicket,
				CFP = new CallForSpeakerViewModel
				{
					Url = currentEvent.CFP.Url,
					StartDate = currentEvent.CFP.StartDate,
					EndDate = currentEvent.CFP.EndDate
				}
			};
			if (currentEvent?.Community != null)
			{
				var community = await db.Communities.FindAsync(currentEvent.Community.Name);
				eventVM.Community = new CommunityViewModel
				{
					Name = community.Name,
					Logo = community.Logo,
					WebSite = community.WebSite
				};
			}
			return eventVM;
		}

		public async Task UpdateAsync(EventViewModel eventVM)
		{
			throw new NotImplementedException();
		}
	}
}
