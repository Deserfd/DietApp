namespace DietApp.Application.FoodDiaryEntrys;

public class DeleteFoodDiaryEntryCommand(
	IRepository<FoodDiaryEntry> foodDiaryEntryRepository
) : ICommand<ByIdRequest, BaseResponse>
{
	public async Task<BaseResponse> ExecuteAsync(ByIdRequest request, CancellationToken cancellationToken = default)
	{
		var foodDiaryEntry = await foodDiaryEntryRepository.GetOneAsync(
			i => i.Id.Value == request.Id,
			cancellationToken);

		if (foodDiaryEntry is null)
			return new BaseResponse(404, "Ingredient not found.");

		await foodDiaryEntryRepository.RemoveAsync(foodDiaryEntry, cancellationToken);
		await foodDiaryEntryRepository.SaveChangesAsync(cancellationToken);

		return new BaseResponse(200, "OK");
	}
}