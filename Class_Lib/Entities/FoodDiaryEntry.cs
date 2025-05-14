namespace DietApp.Core.Entities;

public class FoodDiaryEntry : IEntity
{
	public Id Id { get; set; }  // Используем Guid для уникальности
	public double Quantity { get; set; }  // Количество съеденной пищи
	public DateTime Date { get; set; }     // Дата приема пищи
	
	public virtual Recipe Recipe { get; set; }     // Связь с рецептом
	public virtual User User { get; set; }       // Связь с пользователем
}
