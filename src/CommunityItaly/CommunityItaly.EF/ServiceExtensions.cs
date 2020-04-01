using CommunityItaly.EF.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.EF
{
	public static class ServiceExtensions
	{
		public static IServiceCollection UseDatabase(this IServiceCollection services)
		{
			services.AddDbContext<EventContext>((sp, optionsBuilder) =>
			{
				var settings = sp.GetRequiredService<IOptions<CosmosDbConnections>>();
				optionsBuilder.UseCosmos(
					accountEndpoint: settings.Value.AccountEndpoint, // "https://localhost:8081",
					accountKey: settings.Value.AccountKey, //"C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
					databaseName: settings.Value.DatabaseName); // "event-db");
				optionsBuilder.EnableSensitiveDataLogging();
			}, ServiceLifetime.Scoped);
			return services;
		}
	}
}
