using CommunityItaly.Shared.ViewModels;
using FluentValidation;
using FluentValidation.Results;

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

	public class PersonUpdateValidator : AbstractValidator<PersonUpdateViewModel>
	{
		private readonly IPersonService personService;
		public PersonUpdateValidator(IPersonService personService)
		{
			RuleFor(x => x.Name).NotEmpty();
			RuleFor(x => x.Surname).NotEmpty();
			this.personService = personService;

			RuleFor(x => x.Id).NotEmpty().CustomAsync(async (id, ctx, cancellationToken) =>
			{
				var exist = await personService.ExistsAsync(id);
				if (!exist)
					ctx.AddFailure(new ValidationFailure("Person", $"Person: '{id}' not exist. Please create first"));
			});
		}
	}
}
