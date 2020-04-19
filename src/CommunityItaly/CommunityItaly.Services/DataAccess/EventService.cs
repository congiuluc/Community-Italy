using CommunityItaly.EF;
using CommunityItaly.EF.Entities;
using CommunityItaly.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
			Event currentEvent = new Event(eventVM.Id, eventVM.Name, eventVM.StartDate, eventVM.EndDate);
			currentEvent.SetLogo(eventVM.Logo);
			if(!string.IsNullOrEmpty(eventVM.BuyTicket))
				currentEvent.SetBuyTicket(new Uri(eventVM.BuyTicket));

			if(eventVM.CFP != null)
			{
				currentEvent.SetCallForSpeaker(new CallForSpeaker(new Uri(eventVM.CFP.Url), eventVM.CFP.StartDate, eventVM.CFP.EndDate));
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
					Id = currentEvent.Id,
					Name = currentEvent.Name,
					Logo = currentEvent.Logo,
					StartDate = currentEvent.StartDate,
					EndDate = currentEvent.EndDate,
					BuyTicket = currentEvent.BuyTicket?.ToString(),
					CFP = currentEvent.CFP == null ? null : new CallForSpeakerViewModel
					{
						Url = currentEvent.CFP.Url.ToString(),
						StartDate = currentEvent.CFP.StartDate,
						EndDate = currentEvent.CFP.EndDate
					},
					Community = currentEvent.Community == null ? null : new CommunityUpdateViewModel
					{
						ShortName = currentEvent.Community.ShortName,
						Confirmed = currentEvent.Community.Confirmed,
						Name = currentEvent.Community.Name,
						Logo = currentEvent.Community.Logo,
						WebSite = currentEvent.Community.WebSite.ToString(),
						Managers = !currentEvent.Community.Managers.Any() ? 
							null : 
							currentEvent.Community.Managers.Select(t => new PersonUpdateViewModel
							{
								Id = t.Id,
								Name = t.Name,
								Surname = t.Surname,
								Picture = t.Picture,
								MVP_Code = t.MVP_Code
							}).ToList()
					}
				});

			return new PagedViewModel<EventViewModelReadOnly>
			{
				CurrentPage = take.Value * skip.Value,
				Total = totalElement,
				Entities = result
			};
		}

		public async Task<ICollection<EventViewModelReadOnly>> GetConfirmedIntervalledAsync(DateTimeOffset start, DateTimeOffset end)
		{
			var resultList = await db.Events.Where(x => x.Confirmed == true)
				.ToListAsync()
				.ConfigureAwait(false);

			var result = resultList
				.Where(x => x.StartDate >= start && x.StartDate < end)
				.Select(currentEvent => new EventViewModelReadOnly
				{
					Name = currentEvent.Name,
					Logo = currentEvent.Logo,
					StartDate = currentEvent.StartDate,
					EndDate = currentEvent.EndDate,
					BuyTicket = currentEvent.BuyTicket.ToString(),
					CFP = currentEvent.CFP == null ? null : new CallForSpeakerViewModel
					{
						Url = currentEvent.CFP.Url.ToString(),
						StartDate = currentEvent.CFP.StartDate,
						EndDate = currentEvent.CFP.EndDate
					},
					Community = currentEvent.Community == null ? null : new CommunityUpdateViewModel
					{
						Name = currentEvent.Community.Name,
						Logo = currentEvent.Community.Logo,
						WebSite = currentEvent.Community.WebSite.ToString(),
						Managers = !currentEvent.Community.Managers.Any() ?
							null :
							currentEvent.Community.Managers.Select(t => new PersonUpdateViewModel
							{
								Id = t.Id,
								Name = t.Name,
								Surname = t.Surname,
								Picture = t.Picture,
								MVP_Code = t.MVP_Code
							}).ToList()
					}
				});
			return result.ToList();
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
				BuyTicket = currentEvent.BuyTicket.ToString(),
				CFP = new CallForSpeakerViewModel
				{
					Url = currentEvent.CFP.Url.ToString(),
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
					WebSite = community.WebSite.ToString()
				};
			}
			return eventVM;
		}

		public async Task UpdateAsync(EventViewModel eventVM)
		{
			var currentEvent = await db.Events.FindAsync(eventVM.Id).ConfigureAwait(false);
			if(!string.IsNullOrEmpty(eventVM.BuyTicket))
				currentEvent.SetBuyTicket(new Uri(eventVM.BuyTicket));
			currentEvent.SetConfirmation(eventVM.Confirmed);
			//TODO: Update date and name

			if (eventVM.CFP != null)
			{
				currentEvent.SetCallForSpeaker(new CallForSpeaker(new Uri(eventVM.CFP.Url), eventVM.CFP.StartDate, eventVM.CFP.EndDate));
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
