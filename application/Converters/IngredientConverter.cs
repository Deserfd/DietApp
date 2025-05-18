namespace DietApp.Application.Converters;

public class IngredientConverter : IConverter<Ingredient, IngredientResponse>
{
	public IngredientResponse ToResponse(Ingredient entity) =>
		new(
			entity.Id.Value,
			entity.Name,
			entity.NutritionInfo
		);

	public Ingredient ToEntity(IngredientResponse response) =>
		throw new NotImplementedException("Conversion from DTO to Entity not supported for books");
}