
namespace DietApp.Application.Ingredients;

public class AddOrUpdateIngredientCommand(
	IRepository<Ingredient> ingredientsRepository,
	IValidator<Ingredient>? ingredientValidator = null,
	ILogger<AddOrUpdateIngredientCommand>? logger = null
) : ICommand<AddOrUpdateIngredientReqest, BaseResponse>
{
	public async Task<BaseResponse> ExecuteAsync(AddOrUpdateIngredientReqest request, CancellationToken cancellationToken = default)
	{
		try
		{
			bool isAdding = request.Id == null;
			logger?.LogInformation("RQST: {Operation} ingredient {IngredientName}",
				isAdding ? "creation" : "update",
				request.Name);

			// Подготовка ингредиента
			Ingredient ingredient;
			if (isAdding)
			{
				ingredient = new Ingredient { Id = new Id(Guid.NewGuid()) };
			}
			else
			{
				ingredient = await ingredientsRepository.GetOneAsync(
					x => x.Id.Value == request.Id!.Value,
					cancellationToken);

				if (ingredient == null)
				{
					logger?.LogWarning("RQST: failed: Ingredient {IngredientId} not found", request.Id);
					return new BaseResponse(404, "Ingredient not found");
				}
			}

			// Сохранение старых данных для отката при валидации
			var oldIngredient = (ingredient.Name, ingredient.NutritionInfo);

			// Обновление полей
			ingredient.Name = request.Name;
			ingredient.NutritionInfo = request.NutritionInfo;

			// Валидация (если валидатор предоставлен)
			if (ingredientValidator != null && !await ingredientValidator.ValidateAsync(ingredient, cancellationToken))
			{
				logger?.LogWarning("RQST: failed: Validation failed for ingredient {IngredientName}", request.Name);
				ingredient.Name = oldIngredient.Name;
				ingredient.NutritionInfo = oldIngredient.NutritionInfo;
				return new BaseResponse(400, "Ingredient validation failed");
			}

			// Сохранение ингредиента
			await ingredientsRepository.AddOrUpdateAsync(ingredient, cancellationToken);
			await ingredientsRepository.SaveChangesAsync(cancellationToken);

			logger?.LogInformation("RQST: Successfully {Operation} ingredient {IngredientId} ({IngredientName})",
				isAdding ? "created" : "updated",
				ingredient.Id.Value,
				ingredient.Name);

			return new BaseResponse(200, "OK");
		}
		catch (Exception ex)
		{
			logger?.LogError(ex, "Error occurred while {Operation} ingredient {IngredientName}",
				request.Id == null ? "creating" : "updating",
				request.Name);
			return new BaseResponse(500, "An error occurred while processing your request");
		}
	}

}
