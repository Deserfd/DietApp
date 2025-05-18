namespace DietApp.Core.Entities;

public class RecipeIngredient : IEntity
{
	public Id Id { get; set; }
	public virtual Recipe Recipe { get; set; }
	public virtual Ingredient Ingredient { get; set; }
	public double Weight { get; set; }
}
