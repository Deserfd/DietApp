using Microsoft.Extensions.Logging;

namespace DietApp.Application.Users;

public class AddOrUpdateUserCommand(
	   IRepository<User> usersRepository,
	   IValidator<User>? userValidator = null,
	   ILogger<AddOrUpdateUserCommand>? logger = null
   ) : ICommand<AddOrUpdateUserReqest, BaseResponse>
{
	public async Task<BaseResponse> ExecuteAsync(AddOrUpdateUserReqest request, CancellationToken cancellationToken = default)
	{
		try
		{
			bool isAdding = request.Id == null;
			logger?.LogInformation("RQST: {Operation} user {UserName} (Email: {Email})",
				isAdding ? "creation" : "update",
				request.Name,
				request.Email);

			User potentialUser;
			if (isAdding)
			{				
				potentialUser = await usersRepository.GetOneAsync(
					user => user.Email == request.Email,
					cancellationToken);
				if (potentialUser != null)
				{
					logger?.LogWarning("RQST: failed: User with email {Email} already exists", request.Email);
					return new BaseResponse(409, $"User with email {request.Email} already exists");
				}
				potentialUser = new User { Id = new Id(Guid.NewGuid()) };
			}
			else
			{
				potentialUser = await usersRepository.GetOneAsync(
					x => x.Id.Value == request.Id!.Value,
					cancellationToken);
				if (potentialUser == null)
				{
					logger?.LogWarning("RQST: failed: User {UserId} not found", request.Id);
					return new BaseResponse(404, "User not found");
				}
			}
		
			var oldUser = (
				potentialUser.Name,
				potentialUser.Email,
				potentialUser.Height,
				potentialUser.Weight,
				potentialUser.BirthDate,
				potentialUser.ActivityLevel,
				potentialUser.Goal,
				potentialUser.Gender
			);

			potentialUser.Name = request.Name;
			potentialUser.Email = request.Email;
			potentialUser.Height = request.Height;
			potentialUser.Weight = request.Weight;
			potentialUser.BirthDate = request.Birthdate;
			potentialUser.ActivityLevel = request.ActivityLevel;
			potentialUser.Goal = request.Goal;
			potentialUser.Gender = request.Gender;

			if (userValidator != null && !await userValidator.ValidateAsync(potentialUser, cancellationToken))
			{
				logger?.LogWarning("RQST: failed: Validation failed for user {UserName} (Email: {Email})",
					request.Name,
					request.Email);
				potentialUser.Name = oldUser.Name;
				potentialUser.Email = oldUser.Email;
				potentialUser.Height = oldUser.Height;
				potentialUser.Weight = oldUser.Weight;
				potentialUser.BirthDate = oldUser.BirthDate;
				potentialUser.ActivityLevel = oldUser.ActivityLevel;
				potentialUser.Goal = oldUser.Goal;
				potentialUser.Gender = oldUser.Gender;
				return new BaseResponse(400, "User validation failed");
			}

			await usersRepository.AddOrUpdateAsync(potentialUser, cancellationToken);
			await usersRepository.SaveChangesAsync(cancellationToken);

			logger?.LogInformation("RQST: Successfully {Operation} user {UserId} ({UserName})",
				isAdding ? "created" : "updated",
				potentialUser.Id.Value,
				potentialUser.Name);

			return new BaseResponse(200, "OK");
		}
		catch (Exception ex)
		{
			logger?.LogError(ex, "Error occurred while {Operation} user {UserName} (Email: {Email})",
				request.Id == null ? "creating" : "updating",
				request.Name,
				request.Email);
			return new BaseResponse(500, "An error occurred while processing your request");
		}
	}
}

