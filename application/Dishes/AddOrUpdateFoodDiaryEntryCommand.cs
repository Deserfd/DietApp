namespace DietApp.Application.Dishes;
public class AddOrUpdateFoodDiaryEntryCommand(
	IRepository<FoodDiaryEntry> diaryRepository,
	IRepository<Recipe> recipeRepository,
	IRepository<Ingredient> ingredientRepository,
	IRepository<RecipeIngredient> recipeIngredientRepository,
	IRepository<User> userRepository,
	IValidator<FoodDiaryEntry>? diaryValidator = null,
	ILogger<AddOrUpdateFoodDiaryEntryCommand>? logger = null
) : ICommand<AddOrUpdateFoodDiaryEntryRequest, BaseResponse>
{
	public async Task<BaseResponse> ExecuteAsync(AddOrUpdateFoodDiaryEntryRequest request, CancellationToken cancellationToken = default)
	{
		try
		{
			bool isAdding = request.Id is null;
			logger?.LogInformation("RQST: {Operation} food diary entry for user {UserId} with {FoodType} (Quantity: {Quantity})",
				isAdding ? "creation" : "update",
				request.UserId,
				request.IngredientId.HasValue ? "ingredient" : "recipe",
				request.Quantity);

			// Подготовка записи
			FoodDiaryEntry diaryEntry;
			if (isAdding)
			{
				diaryEntry = new FoodDiaryEntry { Id = new Id(Guid.NewGuid()) };
			}
			else
			{
				diaryEntry = await diaryRepository.GetOneAsync(e => e.Id.Value == request.Id!.Value, cancellationToken);
				if (diaryEntry == null)
				{
					logger?.LogWarning("RQST: failed: Food diary entry {EntryId} not found", request.Id);
					return new BaseResponse(404, "Food diary entry not found");
				}
			}

			// Сохранение старых данных для отката при валидации
			var oldEntry = (diaryEntry.Quantity, diaryEntry.Date, diaryEntry.Recipe, diaryEntry.User);

			// Обновление полей
			diaryEntry.Quantity = request.Quantity;
			diaryEntry.Date = request.Date;
			diaryEntry.User = user;

			// Обработка рецепта или ингредиента
			Recipe recipe;
			double quantity = request.Quantity;
			if (request.IngredientId.HasValue)
			{
				var ingredient = await ingredientRepository.GetOneAsync(i => i.Id.Value == request.IngredientId.Value, cancellationToken);
				if (ingredient == null)
				{
					logger?.LogWarning("RQST: failed: Ingredient {IngredientId} not found", request.IngredientId);
					return new BaseResponse(404, "Ingredient not found");
				}

				// Проверяем, существует ли "рецепт-ингредиент"
				recipe = await recipeRepository.GetOneAsync(
					r => r.Name == $"Ingredient: {ingredient.Name}" && r.Ingredients.Any(ri => ri.Ingredient.Id.Value == ingredient.Id.Value),
					cancellationToken);

				if (recipe == null)
				{
					// Создаем новый рецепт для ингредиента
					recipe = new Recipe
					{
						Id = new Id(Guid.NewGuid()),
						Name = $"Ingredient: {ingredient.Name}",
						Description = $"Single ingredient: {ingredient.Name}",
						NutritionFacts = ingredient.NutritionInfo,
						Ingredients = new List<RecipeIngredient>()
					};

					var recipeIngredient = new RecipeIngredient
					{
						Id = new Id(Guid.NewGuid()),
						Recipe = recipe,
						Ingredient = ingredient,
						Amount = 100 // Базовая порция 100 г
					};
					recipe.Ingredients.Add(recipeIngredient);

					await recipeRepository.AddOrUpdateAsync(recipe, cancellationToken);
					await recipeIngredientRepository.AddOrUpdateAsync(recipeIngredient, cancellationToken);
				}

				quantity = request.Quantity / 100; // Переводим граммы в порции (100 г = 1 порция)
			}
			else if (request.RecipeId.HasValue)
			{
				recipe = await recipeRepository.GetOneAsync(r => r.Id.Value == request.RecipeId.Value, cancellationToken);
				if (recipe == null)
				{
					logger?.LogWarning("RQST: failed: Recipe {RecipeId} not found", request.RecipeId);
					return new BaseResponse(404, "Recipe not found");
				}
			}
			else
			{
				logger?.LogWarning("RQST: failed: Either RecipeId or IngredientId must be provided");
				return new BaseResponse(400, "Either RecipeId or IngredientId must be provided");
			}

			diaryEntry.Quantity = quantity;
			diaryEntry.Recipe = recipe;

			// Валидация (если валидатор предоставлен)
			if (diaryValidator != null && !await diaryValidator.ValidateAsync(diaryEntry, cancellationToken))
			{
				logger?.LogWarning("RQST: failed: Validation failed for food diary entry");
				diaryEntry.Quantity = oldEntry.Quantity;
				diaryEntry.Date = oldEntry.Date;
				diaryEntry.Recipe = oldEntry.Recipe;
				diaryEntry.User = oldEntry.User;
				return new BaseResponse(400, "Food diary entry validation failed");
			}

			// Сохранение записи
			await diaryRepository.AddOrUpdateAsync(diaryEntry, cancellationToken);

			logger?.LogInformation("RQST: Successfully {Operation} food diary entry {EntryId} for user {UserId} with recipe {RecipeId} ({RecipeName})",
				isAdding ? "created" : "updated",
				diaryEntry.Id.Value,
				request.UserId,
				recipe.Id.Value,
				recipe.Name);

			return new BaseResponse(200, "OK");
		}
		catch (Exception ex)
		{
			logger?.LogError(ex, "Error occurred while {Operation} food diary entry for user {UserId}",
				request.Id == null ? "creating" : "updating",
				request.UserId);
			return new BaseResponse(500, "An error occurred while processing your request");
		}
	}
}