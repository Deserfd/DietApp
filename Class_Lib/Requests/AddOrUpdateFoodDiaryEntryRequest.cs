namespace DietApp.Core.Requests;

public record AddOrUpdateFoodDiaryEntryRequest(
	Id? Id,
	Guid? UserId,
	Guid? RecipeId,
	Guid? IngredientId,
	double Quantity,
	DateTime Date
) : IRequest;