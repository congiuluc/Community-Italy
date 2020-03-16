using CommunityItaly.Shared.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Services.Validations
{
	public class PersonValidator : AbstractValidator<PersonViewModel>
	{
		public PersonValidator()
		{
			RuleFor(x => x.Name).NotEmpty();
			RuleFor(x => x.Surname).NotEmpty();
		}
	}
}
