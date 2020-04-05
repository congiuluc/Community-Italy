using Microsoft.Extensions.DependencyInjection;

namespace CommunityItaly.Services
{
	public static class ServiceExtensions
	{
		public static IServiceCollection UseService(this IServiceCollection services)
		{
			services.AddScoped<IEventService, EventService>();
			services.AddScoped<ICommunityService, CommunityService>();
			services.AddScoped<IArticleService, ArticleService>();
			services.AddScoped<IPersonService, PersonService>();
			return services;
		}
	}
}
