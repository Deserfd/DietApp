using DietApp.Core.Responses.DTOs;

namespace DietApp.Core.Responses;

public record RecipeArrayResponse(
	int Code,
	string Description,
	IEnumerable<RecipeResponse> Recipes
	) : BaseResponse(Code, Description);
