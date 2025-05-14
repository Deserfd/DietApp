namespace DietApp.Application.Interfaces;
public interface IValidator<TEntity> where TEntity : class, IEntity
{
	Task<bool> ValidateAsync(TEntity entity, CancellationToken cancellationToken = default);
}