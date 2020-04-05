using CommunityItaly.Shared.ViewModels;
using FluentValidation;

namespace CommunityItaly.Services.Validations
{
	public class PersonValidator : AbstractValidator<PersonViewModel>
	{
		private readonly IPersonService personService;

		public PersonValidator(IPersonService personService)
		{
			RuleFor(x => x.Name).NotEmpty();
			RuleFor(x => x.Surname).NotEmpty();
			this.personService = personService;
		}
	}
}
