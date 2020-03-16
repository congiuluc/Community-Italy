using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface ICommunityService
	{
		Task<bool> CommunityExistsAsync(string communityName);
	}
}
