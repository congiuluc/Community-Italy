using CommunityItaly.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface ICommunityService
	{
		Task<bool> ExistsAsync(string communityName);
		Task<PagedViewModel<CommunityUpdateViewModel>> GetAsync(int? take = 10, int? skip = 0);
		Task<PagedViewModel<CommunityUpdateViewModel>> GetConfirmedAsync(int? take = 10, int? skip = 0);
		Task<CommunityUpdateViewModel> GetById(string id);
		Task<string> CreateAsync(CommunityViewModel communityVM);
		Task UpdateAsync(CommunityUpdateViewModel communityVM);
		Task UpdateImageAsync(string id, Uri img);
		Task DeleteAsync(string id);
	}
}
