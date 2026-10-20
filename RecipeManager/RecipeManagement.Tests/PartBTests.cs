using System;
using System.Collections.Generic;
using System.Linq;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Part B tests: LINQ title/ingredient search, the highest-protein report
/// (including missing nutrition data), the saved-recipe collection and one
/// integrated Part B workflow. A fresh manager is built for every test.
/// </summary>
public sealed class PartBTests
{
    private readonly RecipeManager _manager;

    public PartBTests()
    {
        _manager = new RecipeManager(SampleRecipes());
    }

    private static List<Recipe> SampleRecipes() => new()
    {
        new Recipe
        {
            Id = 1,
            Title = "Chicken Salad",
            Ingredients = new() { "chicken breast", "lettuce" },
            Nutrition = new NutritionInfo { ProteinG = 30 }
        },

        new Recipe
        {
            Id = 2,
            Title = "Beef Stew",
            Ingredients = new() { "beef chuck", "beef stock", "potato" },
            Nutrition = new NutritionInfo { ProteinG = 25 }
        },

        new Recipe
        {
            Id = 3,
            Title = "Fruit Bowl",
            Ingredients = new() { "apple", "banana" }
            // no nutrition on purpose, to test the protein report skips it
        },

        new Recipe
        {
            Id = 4,
            Title = "Egg Toast",
            Ingredients = new() { "egg", "bread" },
            Nutrition = new NutritionInfo { ProteinG = 15 }
        }
    };
    // ---------- Title search (LINQ) ----------

    [Fact]
    public void SearchByTitle_PartialCaseInsensitive_Matches()
    {
        var results = _manager.SearchByTitle("chi");
        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }

    [Fact]
    public void SearchByTitle_IsCaseInsensitive()
    {
        var results = _manager.SearchByTitle("BEEF");
        Assert.Single(results);
        Assert.Equal("Beef Stew", results[0].Title);
    }

    [Fact]
    public void SearchByTitle_Blank_ReturnsAll()
    {
        Assert.Equal(4, _manager.SearchByTitle("").Count);
        Assert.Equal(4, _manager.SearchByTitle("   ").Count);
    }

    
}