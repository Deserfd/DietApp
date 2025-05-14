namespace DietApp.Core.Entities;

public class Recipe : IEntity
{
	public Id Id { get; set; }  // Используем Guid для уникальности
	public string Name { get; set; }
	public string Description { get; set; }
	public NutritionInfo NutritionFacts { get; set; }

	// Список ингредиентов
	public virtual ICollection<RecipeIngredient> Ingredients { get; set; } = [];
	
}
