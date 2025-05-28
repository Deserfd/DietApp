using DietApp.Infrastructure.Db.EF.Converters;
using Microsoft.EntityFrameworkCore;

namespace DietApp.Infrastructure.Db.EF;

internal class DietAppContext(DbContextOptions options) : DbContext(options)
{
	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Properties<Id>()
							.HaveConversion<IdConverter>();

		base.ConfigureConventions(configurationBuilder);
	}

	protected override void OnModelCreating(ModelBuilder mb)
	{
		var entities = typeof(IEntity).Assembly
									  .GetTypes()
									  .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
									  .ToArray();

		foreach (var entityType in entities)
		{
			mb.Entity(entityType).HasKey(nameof(IEntity.Id));
			mb.Entity(entityType).Property(nameof(IEntity.Id)).HasDefaultValueSql("NEWSEQUENTIALID()");
		}

		base.OnModelCreating(mb);
	}
}
