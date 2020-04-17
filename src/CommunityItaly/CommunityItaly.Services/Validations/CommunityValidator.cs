using CommunityItaly.Shared.ViewModels;
using FluentValidation;

namespace CommunityItaly.Services.Validations
{
	public class CommunityValidator : AbstractValidator<CommunityViewModel>
	{
		public CommunityValidator(ICommunityService communityService)
		{
			RuleFor(x => x.Name).NotEmpty().CustomAsync(async(name, ctx, cancellationToken) => 
			{ 
				if(await communityService.ExistsAsync(name))
				{
					ctx.AddFailure($"Community {name} already Exists");
				}
			});
		}
	}

	public class CommunityUpdateValidator : AbstractValidator<CommunityUpdateViewModel>
	{
		public CommunityUpdateValidator(ICommunityService communityService)
		{
			RuleFor(x => x.Name).NotEmpty().CustomAsync(async (name, ctx, cancellationToken) =>
			{
				if (!await communityService.ExistsAsync(name))
				{
					ctx.AddFailure($"Community {name} not exist. Please create first");
				}
			});
		}
	}
}
