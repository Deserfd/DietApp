namespace DietApp.Core.Entities;

public class Recipe : IFood
{
	public Id Id { get; set; }  // Используем Guid для уникальности
	public string Name { get; set; }
	public NutritionInfo NutritionInfo { get; set; }
	// Список ингредиентов
	public virtual ICollection<RecipeIngredient> Ingredients { get; set; } = [];
	
}
