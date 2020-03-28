using CommunityItaly.EF;
using CommunityItaly.Shared.ViewModels;
using System;
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

		public Task<string> CreateAsync(CommunityViewModel communityVM)
		{
			throw new NotImplementedException();
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

		public Task<PagedViewModel<CommunityUpdateViewModel>> GetAsync(int? take = 10, int? skip = 0)
		{
			throw new NotImplementedException();
		}

		public Task<CommunityUpdateViewModel> GetById(string id)
		{
			throw new NotImplementedException();
		}

		public Task<PagedViewModel<CommunityUpdateViewModel>> GetConfirmedAsync(int? take = 10, int? skip = 0)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(CommunityUpdateViewModel communityVM)
		{
			throw new NotImplementedException();
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
