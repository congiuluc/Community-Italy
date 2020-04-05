using CommunityItaly.Shared.ViewModels;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public interface IArticleService
	{
		Task<PagedViewModel<ArticleUpdateViewModel>> GetAsync(int? take = 10, int? skip = 0);
		Task<PagedViewModel<ArticleUpdateViewModel>> GetConfirmedAsync(int? take = 10, int? skip = 0);
		Task<ArticleUpdateViewModel> GetById(string id);
		Task<string> CreateAsync(ArticleViewModel eventVM);
		Task UpdateAsync(ArticleUpdateViewModel eventVM);
		Task DeleteAsync(string id);
		Task<bool> ExistsAsync(string id);
	}
}
