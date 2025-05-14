namespace DietApp.Core.Requests;
public record AddOrUpdateRecipesRequest(
	Guid? Id,
	string Name,
	IngredientWeightDTO[] Ingredients
	) : IRequest;
public record IngredientWeightDTO(Guid Id, double Weight);