namespace DietApp.Application.Recipes;

public class DeleteRecipeCommand(
	IRepository<Recipe> recipeRepository
) : ICommand<ByIdRequest, BaseResponse>
{
	public async Task<BaseResponse> ExecuteAsync(ByIdRequest request, CancellationToken cancellationToken = default)
	{
		var recipe = await recipeRepository.GetOneAsync(
			i => i.Id.Value == request.Id,
			cancellationToken);

		if (recipe is null)
			return new BaseResponse(404, "Ingredient not found.");

		await recipeRepository.RemoveAsync(recipe, cancellationToken);
		await recipeRepository.SaveChangesAsync(cancellationToken);

		return new BaseResponse(200, "OK");
	}
}