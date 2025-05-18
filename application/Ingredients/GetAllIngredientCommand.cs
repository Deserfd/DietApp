namespace DietApp.Application.Ingredients;

public class GetAllIngredientsCommand(
	IRepository<Ingredient> ingredientsRepository,
	IConverter<Ingredient, IngredientResponse> converter
) : ICommand<EmptyRequest, IngredientsArrayResponse>
{
	public async Task<IngredientsArrayResponse> ExecuteAsync(EmptyRequest request, CancellationToken cancellationToken = default)
	{
		try
		{
			var ingredients = await ingredientsRepository.GetAllAsync(cancellationToken);
			var convertedIngredients = ingredients.Select(converter.ToResponse);
			return new IngredientsArrayResponse(200, "OK", convertedIngredients);
		}
		catch (Exception)
		{
			return new IngredientsArrayResponse(500, "Internal server error", []);
		}
	}
}