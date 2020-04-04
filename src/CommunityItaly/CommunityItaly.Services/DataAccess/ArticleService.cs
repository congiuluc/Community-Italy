
using CommunityItaly.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public class ArticleService : IArticleService
	{
		public Task<string> CreateAsync(ArticleViewModel eventVM)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(string id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> ExistsAsync(string id)
		{
			throw new NotImplementedException();
		}

		public Task<PagedViewModel<ArticleUpdateViewModel>> GetAsync(int? take = 10, int? skip = 0)
		{
			throw new NotImplementedException();
		}

		public Task<ArticleUpdateViewModel> GetById(string id)
		{
			throw new NotImplementedException();
		}

		public Task<PagedViewModel<ArticleUpdateViewModel>> GetConfirmedAsync(int? take = 10, int? skip = 0)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(ArticleUpdateViewModel eventVM)
		{
			throw new NotImplementedException();
		}

		public Task UpdateLogoAsync(string id, Uri logo)
		{
			throw new NotImplementedException();
		}
	}
}
