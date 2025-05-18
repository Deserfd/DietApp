namespace DietApp.Application.FoodDiaryEntrys;

public class DeleteIngredientCommand(
	IRepository<Ingredient> ingredientRepository
) : ICommand<ByIdRequest, BaseResponse>
{
	public async Task<BaseResponse> ExecuteAsync(ByIdRequest request, CancellationToken cancellationToken = default)
	{
		var ingredient = await ingredientRepository.GetOneAsync(
			i => i.Id.Value == request.Id,
			cancellationToken);

		if (ingredient is null)
			return new BaseResponse(404, "Ingredient not found.");

		await ingredientRepository.RemoveAsync(ingredient, cancellationToken);
		await ingredientRepository.SaveChangesAsync(cancellationToken);

		return new BaseResponse(200, "OK");
	}
}
