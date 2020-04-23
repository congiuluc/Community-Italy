using CommunityItaly.Shared.ViewModels;
using FluentValidation;
using FluentValidation.Results;

namespace CommunityItaly.Services.Validations
{
	public class EventValidator : AbstractValidator<EventViewModel>
	{
		public EventValidator(ICommunityService communityService)
		{
			RuleFor(x => x.Name).NotEmpty();
			RuleFor(x => x.StartDate).NotNull().LessThanOrEqualTo(x => x.EndDate);
			RuleFor(x => x.EndDate).NotNull().GreaterThanOrEqualTo(x => x.StartDate);
			When(x => x.CFP != null, () =>
			{
				RuleFor(x => x.CFP.Url).NotEmpty();
				RuleFor(x => x.CFP.StartDate).NotNull().LessThanOrEqualTo(x => x.CFP.EndDate);
				RuleFor(x => x.CFP.EndDate).NotNull().GreaterThanOrEqualTo(x => x.CFP.StartDate);
			});
			When(x => string.IsNullOrEmpty(x.CommunityName), () =>
			{
				RuleFor(x => x.CommunityName).CustomAsync(async (name, ctx, cancellationToken) =>
				{
					bool exist = await communityService.ExistsAsync(name);
					if (!exist)
						ctx.AddFailure(new ValidationFailure("CommunityName", $"Community: '{name}' not exist. Please create first"));
				});
			});
		}
	}

	public class EventUpdateValidator : AbstractValidator<EventViewModel>
	{
		public EventUpdateValidator(ICommunityService communityService, IEventService eventService)
		{
			RuleFor(x => x.Id).NotEmpty().CustomAsync(async (id, ctx, cancellationToken) => 
			{
				var exist = await eventService.ExistsAsync(id);
				if(!exist)
					ctx.AddFailure(new ValidationFailure("Event", $"Event: '{id}' not exist. Please create first"));
			});
			RuleFor(x => x.Name).NotEmpty();
			RuleFor(x => x.StartDate).NotNull().LessThanOrEqualTo(x => x.EndDate);
			RuleFor(x => x.EndDate).NotNull().GreaterThanOrEqualTo(x => x.StartDate);
			When(x => x.CFP != null, () =>
			{
				RuleFor(x => x.CFP.Url).NotEmpty();
				RuleFor(x => x.CFP.StartDate).NotNull().LessThanOrEqualTo(x => x.CFP.EndDate);
				RuleFor(x => x.CFP.EndDate).NotNull().GreaterThanOrEqualTo(x => x.CFP.StartDate);
			});
			When(x => string.IsNullOrEmpty(x.CommunityName), () =>
			{
				RuleFor(x => x.CommunityName).CustomAsync(async (name, ctx, cancellationToken) =>
				{
					bool exist = await communityService.ExistsAsync(name);
					if (!exist)
						ctx.AddFailure(new ValidationFailure("CommunityName", $"Community: '{name}' not exist. Please create first"));
				});
			});
		}
	}
}
