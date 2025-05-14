namespace DietApp.Application.Dishes;
public class AddOrUpdateFoodDiaryEntryCommand(
	IRepository<FoodDiaryEntry> foodDiaryEntryRepository,
	IValidator<FoodDiaryEntry>? diaryValidator = null,
	ILogger<AddOrUpdateFoodDiaryEntryCommand>? logger = null
) : ICommand<AddOrUpdateFoodDiaryEntryRequest, BaseResponse>
{
	public async Task<BaseResponse> ExecuteAsync(AddOrUpdateFoodDiaryEntryRequest request, CancellationToken cancellationToken = default)
	{
		try
		{
			bool isAdding = request.Id == null;
			logger?.LogInformation("RQST: {Operation} food diary entry (Quantity: {Quantity}, Date: {Date})",
				isAdding ? "creation" : "update",
				request.Quantity,
				request.Data);

			FoodDiaryEntry? foodDiaryEntry;
			if (isAdding)
			{
				foodDiaryEntry = new FoodDiaryEntry();
			}
			else
			{
				foodDiaryEntry = await foodDiaryEntryRepository.GetOneAsync(
					x => x.Id.Value == request.Id!.Value,
					cancellationToken);

				if (foodDiaryEntry == null)
				{
					logger?.LogWarning("RQST: failed: Food diary entry {EntryId} not found", request.Id);
					return new BaseResponse(404, "Food diary entry not found");
				}
			}

			var oldEntry = (foodDiaryEntry.Quantity, foodDiaryEntry.Date);

			foodDiaryEntry.Quantity = request.Quantity;
			foodDiaryEntry.Date = request.Data;

			if (diaryValidator != null && !await diaryValidator.ValidateAsync(foodDiaryEntry, cancellationToken))
			{
				logger?.LogWarning("RQST: failed: Validation failed for food diary entry (Quantity: {Quantity}, Date: {Date})",
					request.Quantity,
					request.Data);
				foodDiaryEntry.Quantity = oldEntry.Quantity;
				foodDiaryEntry.Date = oldEntry.Date;
				return new BaseResponse(400, "Food diary entry validation failed");
			}

			await foodDiaryEntryRepository.AddOrUpdateAsync(foodDiaryEntry, cancellationToken);
			await foodDiaryEntryRepository.SaveChangesAsync(cancellationToken);

			logger?.LogInformation("RQST: Successfully {Operation} food diary entry {EntryId}",
				isAdding ? "created" : "updated",
				foodDiaryEntry.Id.Value);

			return new BaseResponse(200, "OK");
		}
		catch (Exception ex)
		{
			logger?.LogError(ex, "Error occurred while {Operation} food diary entry",
				request.Id == null ? "creating" : "updating");
			return new BaseResponse(500, "An error occurred while processing your request");
		}
	}
}