namespace DietApp.Core.Responses.DTOs;

public record RecipeResponse(
	Guid Id,
	string Name,
	NutritionInfo NutritionInfo
	) : IResponse;

