namespace DietApp.Application.Recipe;

using DietApp.Core.Entities;
using DietApp.Core.Records;
using DietApp.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AddOrUpdateRecipeCommand(
	IRepository<Recipes> recipesRepository,
	IRepository<Ingredient> ingredientsRepository,
	IValidator<Recipes>? recipeValidator = null,
	ILogger<AddOrUpdateRecipeCommand>? logger = null
) : ICommand<AddOrUpdateRecipesRequest, BaseResponse>
{
	public async Task<BaseResponse> ExecuteAsync(AddOrUpdateRecipesRequest request, CancellationToken cancellationToken = default)
	{
		try
		{
			bool isAdding = request.Id == null;
			logger?.LogInformation("RQST: {Operation} recipe {RecipeName}",
				isAdding ? "creation" : "update",
				request.Name);

			// Подготовка рецепта
			Recipes potentialRecipe;
			if (isAdding)
			{
				potentialRecipe = new Recipes { Id = new Id(Guid.NewGuid()) };
			}
			else
			{
				potentialRecipe = await recipesRepository.GetOneAsync(
					meal => meal.Id.Value == request.Id!.Value,
					cancellationToken);

				if (potentialRecipe == null)
				{
					logger?.LogWarning("RQST: failed: Recipe {RecipeId} not found", request.Id);
					return new BaseResponse(404, "Recipe not found");
				}
			}

			var oldRecipe = (potentialRecipe.Name, Ingredients: potentialRecipe.Ingredients.ToList());

			potentialRecipe.Name = request.Name;

			var ingredients = new List<RecipeIngredient>();
			foreach (var ingredientDTO in request.Ingredients)
			{
				var ingredient = await ingredientsRepository.GetOneAsync(
					i => i.Id.Value == ingredientDTO.Id,
					cancellationToken);
				if (ingredient == null)
				{
					logger?.LogWarning("RQST: failed: Ingredient {IngredientId} not found", ingredientDTO.Id);
					return new BaseResponse(404, $"Ingredient with Id {ingredientDTO.Id} not found");
				}

				ingredients.Add(new RecipeIngredient
				{
					Id = new Id(Guid.NewGuid()), 
					Ingredient = ingredient,
					Weight = ingredientDTO.Weight,
					Recipe = potentialRecipe
				});
			}
			potentialRecipe.Ingredients = ingredients;

			if (recipeValidator != null && !await recipeValidator.ValidateAsync(potentialRecipe, cancellationToken))
			{
				logger?.LogWarning("RQST: failed: Validation failed for recipe {RecipeName}", request.Name);
				potentialRecipe.Name = oldRecipe.Name;
				potentialRecipe.Ingredients = oldRecipe.Ingredients;
				return new BaseResponse(400, "Recipe validation failed");
			}

			await recipesRepository.AddOrUpdateAsync(potentialRecipe, cancellationToken);
			await recipesRepository.SaveChangesAsync(cancellationToken);

			logger?.LogInformation("RQST: Successfully {Operation} recipe {RecipeId} ({RecipeName})",
				isAdding ? "created" : "updated",
				potentialRecipe.Id.Value,
				potentialRecipe.Name);

			return new BaseResponse(200, "OK");
		}
		catch (Exception ex)
		{
			logger?.LogError(ex, "Error occurred while {Operation} recipe {RecipeName}",
				request.Id == null ? "creating" : "updating",
				request.Name);
			return new BaseResponse(500, "An error occurred while processing your request");
		}
	}
}