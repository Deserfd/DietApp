namespace DietApp.Core.Entities;

public class User : IEntity
{
	public Id Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public DateTime BirthDate { get; set; }
	public double Height { get; set; }
	public double Weight { get; set; }
	public Gender Gender { get; set; }
	public ActivityLevel ActivityLevel { get; set; }
	public Goal Goal { get; set; }
	public NutritionInfo NutritionTargets { get; set; }

	public List<FoodDiaryEntry> FoodDiary { get; set; } = [];

}
