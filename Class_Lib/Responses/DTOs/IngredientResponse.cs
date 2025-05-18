public record IngredientResponse(
	Guid Id,
	string Name,
	NutritionInfo NutritionInfo
	): IResponse;
