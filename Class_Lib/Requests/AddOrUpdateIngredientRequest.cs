namespace DietApp.Core.Requests;

public record AddOrUpdateIngredientReqest(
	Guid? Id,
	string Name,
	NutritionInfo NutritionInfo
) : IRequest;