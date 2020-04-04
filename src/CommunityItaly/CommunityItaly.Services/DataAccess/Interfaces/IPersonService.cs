using CommunityItaly.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface IPersonService
	{
		Task<bool> ExistsAsync(string personId);
		Task<PagedViewModel<PersonViewModelReadOnly>> GetAsync(int? take = 10, int? skip = 0);
		Task<PagedViewModel<PersonViewModelReadOnly>> GetConfirmedAsync(int? take = 10, int? skip = 0);
		Task<PersonViewModelReadOnly> GetById(string id);
		Task<string> CreateAsync(PersonViewModel personVM);
		Task UpdateAsync(PersonViewModelReadOnly personVM);
		Task UpdateImageAsync(string id, Uri img);
		Task DeleteAsync(string id);
	}
}
