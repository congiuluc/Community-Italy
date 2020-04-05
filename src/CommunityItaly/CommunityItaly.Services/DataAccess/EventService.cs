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
					currentEvent.AddCommunity(community.ToOwned());
				}
				else
					throw new ArgumentOutOfRangeException($"No community {eventVM.CommunityName} found");
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

		public async Task<bool> ExistsAsync(string id)
		{
			var eventDomain = await db.Events.FindAsync(id).ConfigureAwait(false);
			return eventDomain != null;
		}

		public async Task<PagedViewModel<EventViewModelReadOnly>> GetAsync(int? take = 10, int? skip = 0)
		{
			return await GetAsync(false, take, skip).ConfigureAwait(false);
		}

		public async Task<PagedViewModel<EventViewModelReadOnly>> GetConfirmedAsync(int? take = 10, int? skip = 0)
		{
			return await GetAsync(true, take, skip).ConfigureAwait(false);
		}

		private async Task<PagedViewModel<EventViewModelReadOnly>> GetAsync(bool confirmed, int? take = 10, int? skip = 0)
		{
			take = !take.HasValue || take.Value == 0 ? 10 : take.Value;
			int totalElement = await db.Events.CountAsync().ConfigureAwait(false);

			IQueryable<Event> resultListBase = null;
			if (confirmed == true)
				resultListBase = db.Events.Where(x => x.Confirmed == confirmed);
			else
				resultListBase = db.Events;

			var resultList = await resultListBase
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
					CFP = currentEvent.CFP == null ? null : new CallForSpeakerViewModel
					{
						Url = currentEvent.CFP.Url,
						StartDate = currentEvent.CFP.StartDate,
						EndDate = currentEvent.CFP.EndDate
					},
					Community = currentEvent.Community == null ? null : new CommunityUpdateViewModel
					{
						Name = currentEvent.Community.Name,
						Logo = currentEvent.Community.Logo,
						WebSite = currentEvent.Community.WebSite,
						Managers = !currentEvent.Community.Managers.Any() ? 
							null : 
							currentEvent.Community.Managers.Select(t => new PersonUpdateViewModel
							{
								Id = t.Id,
								Name = t.Name,
								Surname = t.Surname,
								Picture = t.Picture,
								MVP_Code = t.MVP_Code
							})
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
				eventVM.Community = new CommunityUpdateViewModel
				{
					Name = community.Name,
					Logo = community.Logo,
					WebSite = community.WebSite
				};
			}
			return eventVM;
		}

		public async Task UpdateAsync(EventUpdateViewModel eventVM)
		{
			var currentEvent = await db.Events.FindAsync(eventVM.Id).ConfigureAwait(false);
			currentEvent.SetBuyTicket(eventVM.BuyTicket);
			currentEvent.SetConfirmation(eventVM.Confirmed);
			//TODO: Update date and name

			if (eventVM.CFP != null)
			{
				currentEvent.AddCallForSpeaker(new CallForSpeaker(eventVM.CFP.Url, eventVM.CFP.StartDate, eventVM.CFP.EndDate));
			}
			if (!string.IsNullOrEmpty(eventVM.CommunityName))
			{
				Community community = await db.Communities.FindAsync(eventVM.CommunityName);
				if (community != null)
				{
					currentEvent.AddCommunity(community.ToOwned());
				}
				else
					throw new ArgumentOutOfRangeException($"No community {eventVM.CommunityName} found");
			}
			db.Events.Update(currentEvent);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task UpdateLogoAsync(string id, Uri logo)
		{
			var currentEvent = await db.Events.FindAsync(id).ConfigureAwait(false);
			currentEvent.SetLogo(logo);
			db.Events.Update(currentEvent);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}
