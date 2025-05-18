using DietApp.Core.Responses.DTOs;

namespace DietApp.Application.Recipes;

internal class GetAllRecipeCommand(
	IRepository<Recipe> recipesRepository,
	IConverter<Recipe, RecipeResponse> converter
) : ICommand<EmptyRequest, RecipeArrayResponse>
{
	public async Task<RecipeArrayResponse> ExecuteAsync(EmptyRequest request, CancellationToken cancellationToken = default)
	{
		try
		{
			var recipes = await recipesRepository.GetAllAsync(cancellationToken);
			var convertedIngredients = recipes.Select(converter.ToResponse);
			return new RecipeArrayResponse(200, "OK", convertedIngredients);
		}
		catch (Exception)
		{
			return new RecipeArrayResponse(500, "Internal server error", []);
		}
	}
}
