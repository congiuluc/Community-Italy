using CommunityItaly.EF;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Services
{
	public static class ServiceExtensions
	{
		public static IServiceCollection UseService(this IServiceCollection services)
		{
			services.AddScoped<IArticleService, EventService>();
			services.AddScoped<ICommunityService, CommunityService>();
			return services;
		}
	}
}
