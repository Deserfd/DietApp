namespace DietApp.Core.Entities;

public class Ingredient : IEntity
{
	public Id Id { get; set; }  // Используем Guid для уникальности
	public string Name { get; set; }
	public NutritionInfo NutritionInfo { get; set; }

}
