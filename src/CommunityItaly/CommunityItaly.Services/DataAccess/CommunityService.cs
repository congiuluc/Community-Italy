using CommunityItaly.EF;
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

		public async Task<bool> CommunityExistsAsync(string communityName)
		{
			var c = await db.Communities.FindAsync(communityName);
			return c != null;
		}
	}
}
