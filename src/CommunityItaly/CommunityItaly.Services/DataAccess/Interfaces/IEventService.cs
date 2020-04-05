﻿using CommunityItaly.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface IEventService
	{
		Task<PagedViewModel<EventViewModelReadOnly>> GetAsync(int? take = 10, int? skip = 0);
		Task<PagedViewModel<EventViewModelReadOnly>> GetConfirmedAsync(int? take = 10, int? skip = 0);
		Task<EventViewModelReadOnly> GetById(string id);
		Task<string> CreateAsync(EventViewModel eventVM);
		Task UpdateAsync(EventUpdateViewModel eventVM);
		Task UpdateLogoAsync(string id, Uri logo);
		Task DeleteAsync(string id);
		Task<bool> ExistsAsync(string id);
	}
}