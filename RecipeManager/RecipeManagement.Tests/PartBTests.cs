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

    [Fact]
    public void SearchByTitle_NoMatch_ReturnsEmpty()
    {
        Assert.Empty(_manager.SearchByTitle("pizza"));
    }

    // ---------- Ingredient search (LINQ) ----------

    [Fact]
    public void SearchByIngredient_CaseInsensitivePartial_Matches()
    {
        var results = _manager.SearchByIngredient("CHICKEN");
        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
    }

    [Fact]
    public void SearchByIngredient_RecipeReturnedOnce_WhenSeveralIngredientsMatch()
    {
        // "beef" appears in two of recipe 2's ingredients, but it must come back once.
        var results = _manager.SearchByIngredient("beef");
        Assert.Single(results);
        Assert.Equal(2, results[0].Id);
    }

    [Fact]
    public void SearchByIngredient_Blank_ReturnsAll()
    {
        Assert.Equal(4, _manager.SearchByIngredient("").Count);
    }

    // ---------- Highest-protein report (LINQ) ----------

    [Fact]
    public void GetHighestProtein_OrdersByProteinDescending()
    {
        var results = _manager.GetHighestProteinRecipes(2);
        Assert.Equal(new[] { 1, 2 }, results.Select(r => r.Id).ToArray());
    }

    [Fact]
    public void GetHighestProtein_ExcludesRecipesWithNoProtein_AndDoesNotCrash()
    {
        // Recipe 3 has no nutrition data; it must be skipped, not throw.
        var results = _manager.GetHighestProteinRecipes(10);
        Assert.Equal(3, results.Count);
        Assert.DoesNotContain(results, r => r.Id == 3);
    }

    [Fact]
    public void GetHighestProtein_ZeroOrNegative_ReturnsEmpty()
    {
        Assert.Empty(_manager.GetHighestProteinRecipes(0));
        Assert.Empty(_manager.GetHighestProteinRecipes(-5));
    }

    // ---------- Saved-recipe collection (HashSet) ----------

    [Fact]
    public void AddSavedRecipe_ExistingId_ReturnsTrueAndIsSaved()
    {
        Assert.True(_manager.AddSavedRecipe(1));
        Assert.True(_manager.IsRecipeSaved(1));
    }

    [Fact]
    public void AddSavedRecipe_MissingId_ReturnsFalse()
    {
        Assert.False(_manager.AddSavedRecipe(999));
        Assert.False(_manager.IsRecipeSaved(999));
    }

    [Fact]
    public void AddSavedRecipe_Duplicate_ReturnsFalse()
    {
        Assert.True(_manager.AddSavedRecipe(1));
        Assert.False(_manager.AddSavedRecipe(1));
        Assert.Single(_manager.GetSavedRecipes());
    }

    [Fact]
    public void RemoveSavedRecipe_Saved_ReturnsTrue()
    {
        _manager.AddSavedRecipe(1);
        Assert.True(_manager.RemoveSavedRecipe(1));
        Assert.False(_manager.IsRecipeSaved(1));
    }

    [Fact]
    public void RemoveSavedRecipe_NotSaved_ReturnsFalse()
    {
        Assert.False(_manager.RemoveSavedRecipe(1));
    }

    [Fact]
    public void GetSavedRecipes_ReturnsAllSavedIds()
    {
        _manager.AddSavedRecipe(1);
        _manager.AddSavedRecipe(2);
        var saved = _manager.GetSavedRecipes();
        Assert.Equal(2, saved.Count);
        Assert.Contains(1, saved);
        Assert.Contains(2, saved);
    }


    
}

