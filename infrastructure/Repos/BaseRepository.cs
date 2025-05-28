using DietApp.Infrastructure.Db.EF;
using Microsoft.EntityFrameworkCore;

namespace DietApp.Infrastructure.Repos;

public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
	private BaseRepository() { }
	private DietAppContext DbContext { get; init; }
	public static BaseRepository<TEntity> CreateRepository(DbContextOptions dbContextOptions) =>
		new() { DbContext = new(dbContextOptions) };

	public async Task AddOrUpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
	{
		var toAdd = entities.Where(e => e.Id is null).ToArray();
		var toUpdate = entities.Except(toAdd).ToArray();

		DbContext.UpdateRange(toUpdate);
		await DbContext.AddRangeAsync(toAdd, cancellationToken);
		await DbContext.SaveChangesAsync(cancellationToken);
	}

	public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return Task.FromResult(DbContext.Set<TEntity>().ToArray().AsEnumerable());
	}

	public Task<IEnumerable<TEntity>> GetAllAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(DbContext.Set<TEntity>().Where(predicate).ToArray().AsEnumerable());
	}

	public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
	{
		return Task.Run(() => DbContext.RemoveRange(entities), cancellationToken);
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return DbContext.SaveChangesAsync(cancellationToken);
	}
}
