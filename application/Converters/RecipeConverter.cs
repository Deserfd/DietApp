using DietApp.Core.Responses.DTOs;

namespace DietApp.Application.Converters;

public class RecipeConverter : IConverter<Recipe, RecipeResponse>
{
	public RecipeResponse ToResponse(Recipe entity) =>
		new(
			entity.Id.Value,
			entity.Name,
			entity.NutritionInfo
		);

	public Recipe ToEntity(RecipeResponse response) =>
		throw new NotImplementedException("Conversion from DTO to Entity not supported for books");
}

