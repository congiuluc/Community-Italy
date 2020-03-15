using CommunityItaly.EF.Entities;
using CommunityItaly.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface IEventServices
	{
		Task<IList<EventViewModel>> GetAsync(int? take = 10, int? skip = 0);
		Task<Event> GetById(int id);
		Task CreateAsync(EventViewModel eventVM);
		Task UpdateAsync(EventViewModel eventVM);
		Task DeleteAsync(int id);
	}
}
