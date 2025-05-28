using DietApp.Application;
using DietApp.Infrastructure.Repos;

namespace DietApp.Infrastructure;

public static class Extensions
{
	public static IServiceCollection AddDietAppRepositories(this IServiceCollection services, DbContextOptions dbContextOptions)
	{
		var entityTypes = typeof(IEntity).Assembly
										 .GetTypes()
										 .Where(t => typeof(IEntity).IsAssignableFrom(t) &&
													 t.IsClass && !t.IsAbstract);

		foreach (var entityType in entityTypes)
			services.AddSingleton(typeof(IRepository<>).MakeGenericType(entityType),
								  typeof(BaseRepository<>).MakeGenericType(entityType)!
														  .GetMethod(nameof(BaseRepository<IEntity>.CreateRepository))
														  .Invoke(null, [dbContextOptions]));

		return services;
	}

	public static IServiceCollection AddFullDietApp(this IServiceCollection services, DbContextOptions dbContextOptions) =>
		services.AddDietAppRepositories(dbContextOptions)
				.AddDietApp();
}
