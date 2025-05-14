using DietApp.Core.Enums;

public record AddOrUpdateUserReqest(
	Guid? Id,
	string Name,
	string Email,
	double Height,
	double Weight,
	DateTime Birthdate,
	ActivityLevel ActivityLevel,
	Goal Goal,
	Gender Gender
) : IRequest;