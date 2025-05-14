namespace DietApp.Core.Interfaces;
public interface IFood : IEntity
{
	public string Name { get; set; }
	public NutritionInfo NutritionInfo { get; set; }
}

