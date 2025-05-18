namespace DietApp.Core.Responses;

public record IngredientsArrayResponse(
	int Code,
	string Description,
	IEnumerable<IngredientResponse> Ingredients
	):BaseResponse(Code, Description);
