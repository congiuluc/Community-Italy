using CommunityItaly.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public class PersonService : IPersonService
	{
		public Task<string> CreateAsync(PersonViewModel personVM)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(string id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> ExistsAsync(string personId)
		{
			throw new NotImplementedException();
		}

		public Task<PagedViewModel<PersonViewModelReadOnly>> GetAsync(int? take = 10, int? skip = 0)
		{
			throw new NotImplementedException();
		}

		public Task<PersonViewModelReadOnly> GetById(string id)
		{
			throw new NotImplementedException();
		}

		public Task<PagedViewModel<PersonViewModelReadOnly>> GetConfirmedAsync(int? take = 10, int? skip = 0)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(PersonViewModel personVM)
		{
			throw new NotImplementedException();
		}

		public Task UpdateImageAsync(string id, Uri img)
		{
			throw new NotImplementedException();
		}
	}
}
