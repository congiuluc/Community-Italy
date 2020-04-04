using CommunityItaly.EF;
using CommunityItaly.EF.Entities;
using CommunityItaly.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public class CommunityService : ICommunityService
	{
		private readonly EventContext db;

		public CommunityService(EventContext db)
		{
			this.db = db;
		}

		public async Task<string> CreateAsync(CommunityViewModel communityVM)
		{
			Community domain = new Community(communityVM.Name);
			domain.SetConfirmation(false);
			domain.SetWebSite(communityVM.WebSite);
			foreach (var m in communityVM.Managers)
			{
				var foundManager = await db.People.FindAsync(m.Id).ConfigureAwait(false);
				if (foundManager == null)
					throw new ArgumentOutOfRangeException($"No person find with id {m.Id}");
				else
					domain.AddManager(foundManager.ToOwned());
			}
			db.Communities.Add(domain);
			await db.SaveChangesAsync().ConfigureAwait(false);
			return domain.ShortName;
		}

		public async Task DeleteAsync(string id)
		{
			var c = await db.Communities.FindAsync(id).ConfigureAwait(false);
			if (c == null)
				throw new ArgumentOutOfRangeException($"Community {id} not found");
			db.Communities.Remove(c);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<bool> ExistsAsync(string communityName)
		{
			var c = await db.Communities.FindAsync(communityName);
			return c != null;
		}

		public async Task<PagedViewModel<CommunityUpdateViewModel>> GetAsync(int? take = 10, int? skip = 0)
		{
			return await GetAsync(false, take, skip);
		}

		public async Task<CommunityUpdateViewModel> GetById(string id)
		{
			var currentCommunity = await db.Communities.FindAsync(id).ConfigureAwait(false);
			if (currentCommunity == null)
				throw new ArgumentOutOfRangeException($"Community {id} not exists");
			CommunityUpdateViewModel communityVM = new CommunityUpdateViewModel
			{
				Name = currentCommunity.Name,
				Logo = currentCommunity.Logo,
				Confirmed = currentCommunity.Confirmed,
				Id = currentCommunity.ShortName,
				WebSite = currentCommunity.WebSite,
				Managers = currentCommunity.Managers.Select(t => new PersonViewModelReadOnly
				{
					Id = t.Id,
					Name = t.Name,
					Surname = t.Surname,
					Picture = t.Picture,
					MVP_Code = t.MVP_Code
				})
			};
			return communityVM;
		}

		public async Task<PagedViewModel<CommunityUpdateViewModel>> GetConfirmedAsync(int? take = 10, int? skip = 0)
		{
			return await GetAsync(confirmed: true, take, skip);
		}

		private async Task<PagedViewModel<CommunityUpdateViewModel>> GetAsync(bool confirmed = false, int? take = 10, int? skip = 0)
		{
			take = !take.HasValue || take.Value == 0 ? 10 : take.Value;
			int totalElement = await db.Communities.CountAsync().ConfigureAwait(false);

			IQueryable<Community> resultListBase = null;
			if (confirmed == true)
				resultListBase = db.Communities.Where(x => x.Confirmed == confirmed);
			else
				resultListBase = db.Communities;

			var resultList = await resultListBase
				.Skip(skip.Value)
				.Take(take.Value)
				.ToListAsync()
				.ConfigureAwait(false);

			var result = resultList
				.Select(currentCommunity => new CommunityUpdateViewModel
				{
					Name = currentCommunity.Name,
					Logo = currentCommunity.Logo,
					Confirmed = currentCommunity.Confirmed,
					Id = currentCommunity.ShortName,
					WebSite = currentCommunity.WebSite,
					Managers = !currentCommunity.Managers.Any() ?
						null :
						currentCommunity.Managers.Select(t => new PersonViewModelReadOnly
						{
							Id = t.Id,
							Name = t.Name,
							Surname = t.Surname,
							Picture = t.Picture,
							MVP_Code = t.MVP_Code
						})
				});

			return new PagedViewModel<CommunityUpdateViewModel>
			{
				CurrentPage = take.Value * skip.Value,
				Total = totalElement,
				Entities = result
			};
		}

		public async Task UpdateAsync(CommunityUpdateViewModel communityVM)
		{
			var currentCommunity = await db.Communities.FindAsync(communityVM.Id).ConfigureAwait(false);
			currentCommunity.SetConfirmation(communityVM.Confirmed);
			currentCommunity.SetWebSite(communityVM.WebSite);
			var updateManagers = communityVM.Managers.ToList();
			// Manager da rimuovere o già presenti
			foreach (var m in currentCommunity.Managers)
			{
				var foundManager = updateManagers.Find(x => x.Id == m.Id);
				if (foundManager == null)
					currentCommunity.RemoveManager(m.Id);
				else
					updateManagers.Remove(foundManager);
			}
			// Manager da aggiungere
			foreach (var m in updateManagers)
			{
				var person = await db.People.FindAsync(m.Id).ConfigureAwait(false);
				currentCommunity.AddManager(person.ToOwned());
			}
			db.Communities.Update(currentCommunity);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task UpdateImageAsync(string id, Uri img)
		{
			var c = await db.Communities.FindAsync(id).ConfigureAwait(false);
			c.SetLogo(img);
			db.Communities.Update(c);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}
