namespace DietApp.Core.Requests;

public record AddOrUpdateFoodDiaryEntryRequest(
	Guid? Id,
	DateTime Data,
	double Quantity
	) : IRequest;