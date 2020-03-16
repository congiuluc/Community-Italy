using CommunityItaly.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunityItaly.Services.DataAccess
{
	public class CommunityService : ICommunityService
	{
		private readonly EventContext db;

		public CommunityService(EventContext db)
		{
			this.db = db;
		}

		public async Task<bool> CommunityExistsAsync(string communityName)
		{
			var c = await db.Communities.FindAsync(communityName);
			return c != null;
		}
	}
}
