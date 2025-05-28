using DietApp.Application.Converters;
using DietApp.Core.Responses.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietApp.Application;

public static class Extensions
{
	public static IServiceCollection AddDietApp(this IServiceCollection services)
	{
		
		var appCommands = typeof(ICommand<,>).Assembly
								 .GetTypes()
								 .Where(t => t.IsClass && typeof(ICommand<,>).IsAssignableFrom(t))
								 .ToArray();
		services.AddSingleton<IConverter<Ingredient, IngredientResponse>, IngredientConverter>()
				.AddSingleton<IConverter<Recipe, RecipeResponse>, RecipeConverter>();

		foreach (var command in appCommands)
			services.AddSingleton(command);

		return services;
	}
}
