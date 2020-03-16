using System.Threading.Tasks;

namespace CommunityItaly.Services.DataAccess
{
	public interface ICommunityService
	{
		Task<bool> CommunityExistsAsync(string communityName);
	}
}
