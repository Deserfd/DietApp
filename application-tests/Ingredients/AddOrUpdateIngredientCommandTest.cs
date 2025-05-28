using DietApp.Application.Ingredients;

namespace DietApp.Tests.Application.Ingredients;

public class AddOrUpdateIngredientCommandTests
{
	private const string ingredientId = "a3bb8f1e-1208-430a-9e48-c221557ec962";

	[TestCase("Apple", 0.9, 0.2, 10)]
	[TestCase("Potato", 2.0, 0.1, 15)]
	public async Task ExecuteAsync_WhenIngredientIsAdded_ReturnsOK(string Name, double Protein, double Fat, double Carbohydrates)
	{
		// Arrange
		FakeRepository<Ingredient> ingredientRepository = new();
		AddOrUpdateIngredientCommand command = new(ingredientRepository);
		var nutInfo = new NutritionInfo(Protein, Fat, Carbohydrates);
		AddOrUpdateIngredientReqest addIngredientRequest = new(null, Name, nutInfo);

		// Act
		var response = await command.ExecuteAsync(addIngredientRequest);
		var addedIngredient = ingredientRepository.Db.Last();

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(response.Code, Is.EqualTo(200));
			Assert.That(ingredientRepository.Db, Has.Count.EqualTo(1));
			Assert.That(addedIngredient.Name, Is.EqualTo(Name));
			Assert.That(addedIngredient.NutritionInfo.Protein, Is.EqualTo(Protein));
			Assert.That(addedIngredient.NutritionInfo.Fat, Is.EqualTo(Fat));
			Assert.That(addedIngredient.NutritionInfo.Carbohydrates, Is.EqualTo(Carbohydrates));
		});
	}

	[TestCase(ingredientId, "Carrot", 1.0, 0.3, 12)]
	[TestCase(ingredientId, "Potato", 2.1, 0.2, 18)]
	public async Task ExecuteAsync_WhenIngredientIsUpdated_ReturnsOK(Guid Id, string Name, double Protein, double Fat, double Carbohydrates)
	{
		// Arrange
		FakeRepository<Ingredient> ingredientRepository = new(
			[
				new Ingredient
					{
						Id = new(Id),
						Name = "Old Name",
						NutritionInfo = new NutritionInfo(0.5, 0.1, 8.0)
					}
			]);
		AddOrUpdateIngredientCommand command = new(ingredientRepository);
		var nutInfo = new NutritionInfo(Protein, Fat, Carbohydrates);
		AddOrUpdateIngredientReqest addIngredientRequest = new(Id, Name, nutInfo);
		
		// Act
		var response = await command.ExecuteAsync(addIngredientRequest);
		var updatedIngredient = ingredientRepository.Db.Last();

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(response.Code, Is.EqualTo(200));
			Assert.That(ingredientRepository.Db, Has.Count.EqualTo(1));
			Assert.That(updatedIngredient.Name, Is.EqualTo(Name));
			Assert.That(updatedIngredient.NutritionInfo.Protein, Is.EqualTo(Protein));
			Assert.That(updatedIngredient.NutritionInfo.Fat, Is.EqualTo(Fat));
			Assert.That(updatedIngredient.NutritionInfo.Carbohydrates, Is.EqualTo(Carbohydrates));
		});
	}
}