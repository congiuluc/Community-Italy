using CommunityItaly.Shared.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

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
