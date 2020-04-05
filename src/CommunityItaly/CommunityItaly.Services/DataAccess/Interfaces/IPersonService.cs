using CommunityItaly.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface IPersonService
	{
		Task<bool> ExistsAsync(string personId);
		Task<PagedViewModel<PersonUpdateViewModel>> GetAsync(int? take = 10, int? skip = 0);
		Task<PagedViewModel<PersonUpdateViewModel>> GetConfirmedAsync(int? take = 10, int? skip = 0);
		Task<PersonUpdateViewModel> GetById(string id);
		Task<string> CreateAsync(PersonViewModel personVM);
		Task UpdateAsync(PersonUpdateViewModel personVM);
		Task UpdateImageAsync(string id, Uri img);
		Task DeleteAsync(string id);
	}
}
