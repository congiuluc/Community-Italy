
using CommunityItaly.EF;
using CommunityItaly.EF.Entities;
using CommunityItaly.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public class ArticleService : IArticleService
	{
		private readonly EventContext db;

		public ArticleService(EventContext db)
		{
			this.db = db;
		}

		public async Task<string> CreateAsync(ArticleViewModel articleVM)
		{
			Article domain = new Article(articleVM.Url, articleVM.PublishDate, articleVM.Name);
			domain.SetConfirmation(false);
			foreach (var a in articleVM.Authors)
			{
				var foundAuthor = await db.People.FindAsync(a.Id).ConfigureAwait(false);
				if (foundAuthor == null)
					throw new ArgumentOutOfRangeException($"No person find with id {a.Id}");
				else
					domain.AddAuthor(foundAuthor.ToOwned());
			}
			db.Articles.Add(domain);
			await db.SaveChangesAsync().ConfigureAwait(false);
			return domain.Id;
		}

		public async Task DeleteAsync(string id)
		{
			var currentArticle = await db.Articles.FindAsync(id).ConfigureAwait(false);
			db.Articles.Remove(currentArticle);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<bool> ExistsAsync(string id)
		{
			var articleDomain = await db.Articles.FindAsync(id).ConfigureAwait(false);
			return articleDomain != null;
		}

		public async Task<PagedViewModel<ArticleUpdateViewModel>> GetAsync(int? take = 10, int? skip = 0)
		{
			return await GetAsync(false, take, skip).ConfigureAwait(false);
		}
		public async Task<PagedViewModel<ArticleUpdateViewModel>> GetConfirmedAsync(int? take = 10, int? skip = 0)
		{
			return await GetAsync(true, take, skip).ConfigureAwait(false);
		}

		private async Task<PagedViewModel<ArticleUpdateViewModel>> GetAsync(bool confirmed, int? take = 10, int? skip = 0)
		{
			take = !take.HasValue || take.Value == 0 ? 10 : take.Value;
			int totalElement = await db.Events.CountAsync().ConfigureAwait(false);

			IQueryable<Article> resultListBase = null;
			if (confirmed == true)
				resultListBase = db.Articles.Where(x => x.Confirmed == confirmed);
			else
				resultListBase = db.Articles;

			var resultList = await resultListBase
				.Skip(skip.Value)
				.Take(take.Value)
				.ToListAsync()
				.ConfigureAwait(false);

			var result = resultList
				.Select(currentArticle => new ArticleUpdateViewModel
				{
					Id = currentArticle.Id,
					Name = currentArticle.Name,
					Url = currentArticle.Url,
					PublishDate = currentArticle.PublishDate,
					Confirmed = currentArticle.Confirmed,
					Authors = currentArticle.Authors.Select(t => new PersonUpdateViewModel
					{
						Id = t.Id,
						Name = t.Name,
						Surname = t.Surname,
						Picture = t.Picture,
						MVP_Code = t.MVP_Code
					})
				});

			return new PagedViewModel<ArticleUpdateViewModel>
			{
				CurrentPage = take.Value * skip.Value,
				Total = totalElement,
				Entities = result
			};
		}

		public async Task<ArticleUpdateViewModel> GetById(string id)
		{
			var currentArticle = await db.Articles.FindAsync(id).ConfigureAwait(false);
			if (currentArticle == null)
				throw new ArgumentOutOfRangeException($"Community {id} not exists");
			ArticleUpdateViewModel authorVM = new ArticleUpdateViewModel
			{
				Name = currentArticle.Name,
				Confirmed = currentArticle.Confirmed,
				Id = currentArticle.Id,
				Url = currentArticle.Url,
				PublishDate = currentArticle.PublishDate,
				Authors = currentArticle.Authors.Select(t => new PersonUpdateViewModel
				{
					Id = t.Id,
					Name = t.Name,
					Surname = t.Surname,
					Picture = t.Picture,
					MVP_Code = t.MVP_Code
				})
			};
			return authorVM;
		}


		public async Task UpdateAsync(ArticleUpdateViewModel eventVM)
		{
			var currentArticle = await db.Articles.FindAsync(eventVM.Id).ConfigureAwait(false);
			var updateAuthors = currentArticle.Authors.ToList();
			// Manager da rimuovere o già presenti
			foreach (var a in currentArticle.Authors)
			{
				var findAuthor = updateAuthors.Find(x => x.Id == a.Id);
				if (findAuthor == null)
					currentArticle.RemoveAuthor(a.Id);
				else
					updateAuthors.Remove(findAuthor);
			}
			// Manager da aggiungere
			foreach (var a in updateAuthors)
			{
				var person = await db.People.FindAsync(a.Id).ConfigureAwait(false);
				currentArticle.AddAuthor(person.ToOwned());
			}

			db.Articles.Update(currentArticle);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}
