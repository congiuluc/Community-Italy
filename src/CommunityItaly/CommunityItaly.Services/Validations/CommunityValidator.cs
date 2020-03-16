using CommunityItaly.Shared.ViewModels;
using FluentValidation;

namespace CommunityItaly.Services.Validations
{
	public class CommunityValidator : AbstractValidator<CommunityViewModel>
	{
		public CommunityValidator()
		{
			RuleFor(x => x.Name).NotEmpty();
		}
	}
}
