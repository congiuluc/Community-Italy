using CommunityItaly.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface IEventService
	{
		Task<PagedViewModel<EventViewModelReadOnly>> GetAsync(int? take = 10, int? skip = 0);
		Task<PagedViewModel<EventViewModelReadOnly>> GetConfirmedAsync(int? take = 10, int? skip = 0);
		Task<ICollection<EventViewModelReadOnly>> GetConfirmedIntervalledAsync(DateTimeOffset start, DateTimeOffset end);
		Task<EventViewModelReadOnly> GetById(string id);
		Task<string> CreateAsync(EventViewModel eventVM);
		Task UpdateAsync(EventViewModel eventVM);
		Task UpdateLogoAsync(string id, Uri logo);
		Task DeleteAsync(string id);
		Task<bool> ExistsAsync(string id);
	}
}
