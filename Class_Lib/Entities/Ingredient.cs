namespace DietApp.Core.Entities;

public class Ingredient : IFood
{
	public Id Id { get; set; }  // Используем Guid для уникальности
	public string Name { get; set; }
	public NutritionInfo NutritionInfo { get; set; }

}
