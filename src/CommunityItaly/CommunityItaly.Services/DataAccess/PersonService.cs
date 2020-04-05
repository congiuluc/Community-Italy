using CommunityItaly.EF;
using CommunityItaly.EF.Entities;
using CommunityItaly.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Services
{
	public class PersonService : IPersonService
	{
		private readonly EventContext db;

		public PersonService(EventContext db)
		{
			this.db = db;
		}

		public async Task<string> CreateAsync(PersonViewModel personVM)
		{
			Person domain = new Person(personVM.Name, personVM.Surname);
			domain.SetConfirmation(false);
			domain.SetMVPCode(personVM.MVP_Code);
			db.People.Add(domain);
			await db.SaveChangesAsync().ConfigureAwait(false);
			return domain.Id;
		}

		public async Task DeleteAsync(string id)
		{
			var currentPerson = await db.People.FindAsync(id).ConfigureAwait(false);
			db.People.Remove(currentPerson);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<bool> ExistsAsync(string personId)
		{
			var personDomain = await db.People.FindAsync(personId).ConfigureAwait(false);
			return personDomain != null;
		}

		public async Task<PagedViewModel<PersonUpdateViewModel>> GetAsync(int? take = 10, int? skip = 0)
		{
			return await GetAsync(false, take, skip).ConfigureAwait(false);
		}

		public async Task<PagedViewModel<PersonUpdateViewModel>> GetConfirmedAsync(int? take = 10, int? skip = 0)
		{
			return await GetAsync(true, take, skip).ConfigureAwait(false);
		}

		private async Task<PagedViewModel<PersonUpdateViewModel>> GetAsync(bool confirmed, int? take = 10, int? skip = 0)
		{
			take = !take.HasValue || take.Value == 0 ? 10 : take.Value;
			int totalElement = await db.People.CountAsync().ConfigureAwait(false);

			IQueryable<Person> resultListBase = null;
			if (confirmed == true)
				resultListBase = db.People.Where(x => x.Confirmed == confirmed);
			else
				resultListBase = db.People;

			var resultList = await resultListBase
				.Skip(skip.Value)
				.Take(take.Value)
				.ToListAsync()
				.ConfigureAwait(false);

			var result = resultList
				.Select(currentPerson => new PersonUpdateViewModel
				{
					Id = currentPerson.Id,
					Name = currentPerson.Name,
					Surname = currentPerson.Surname,
					MVP_Code = currentPerson.MVP_Code,
					Picture = currentPerson.Picture
				});

			return new PagedViewModel<PersonUpdateViewModel>
			{
				CurrentPage = take.Value * skip.Value,
				Total = totalElement,
				Entities = result
			};
		}

		public async Task<PersonUpdateViewModel> GetById(string id)
		{
			var currentPerson = await db.People.FindAsync(id).ConfigureAwait(false);
			if (currentPerson == null)
				throw new ArgumentOutOfRangeException($"Person {id} not exists");
			PersonUpdateViewModel personVM = new PersonUpdateViewModel
			{
				Id = currentPerson.Id,
				Name = currentPerson.Name,
				Surname = currentPerson.Surname,
				Picture = currentPerson.Picture,
				MVP_Code = currentPerson.MVP_Code,
				Confirmed = currentPerson.Confirmed
			};
			return personVM;
		}

		public async Task UpdateAsync(PersonUpdateViewModel personVM)
		{
			var currentPerson = await db.People.FindAsync(personVM.Id).ConfigureAwait(false);
			currentPerson.SetConfirmation(personVM.Confirmed);
			currentPerson.SetMVPCode(personVM.MVP_Code);
			db.People.Update(currentPerson);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task UpdateImageAsync(string id, Uri img)
		{
			var currentPerson = await db.People.FindAsync(id).ConfigureAwait(false);
			currentPerson.SetPicture(img);
			db.People.Update(currentPerson);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}
