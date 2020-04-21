using CommunityItaly.Shared.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityItaly.Web.Validator
{
	public class EventReadOnlyValidator : AbstractValidator<EventViewModelReadOnly>
	{
		public EventReadOnlyValidator()
		{
			RuleFor(x => x.Name).NotEmpty();
			RuleFor(x => x.StartDate).NotNull().LessThanOrEqualTo(x => x.EndDate);
			RuleFor(x => x.EndDate).NotNull().GreaterThanOrEqualTo(x => x.StartDate);
			When(x => !string.IsNullOrEmpty(x.CFP?.Url), () =>
			{
				RuleFor(x => x.CFP.Url).NotEmpty();
				RuleFor(x => x.CFP.StartDate).NotNull().LessThanOrEqualTo(x => x.CFP.EndDate);
				RuleFor(x => x.CFP.EndDate).NotNull().GreaterThanOrEqualTo(x => x.CFP.StartDate);
			});
		}
	}

	public class EventValidator : AbstractValidator<EventViewModel>
	{
		public EventValidator()
		{
			RuleFor(x => x.Name).NotEmpty();
			RuleFor(x => x.StartDate).NotNull().LessThanOrEqualTo(x => x.EndDate);
			RuleFor(x => x.EndDate).NotNull().GreaterThanOrEqualTo(x => x.StartDate);
			When(x => !string.IsNullOrEmpty(x.CFP?.Url), () =>
			{
				RuleFor(x => x.CFP.Url).NotEmpty();
				RuleFor(x => x.CFP.StartDate).NotNull().LessThanOrEqualTo(x => x.CFP.EndDate);
				RuleFor(x => x.CFP.EndDate).NotNull().GreaterThanOrEqualTo(x => x.CFP.StartDate);
			});
		}
	}
}
