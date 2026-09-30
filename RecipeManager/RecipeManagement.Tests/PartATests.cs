using System;
using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Part A tests: normal behaviour of each nominated collection plus the
/// required edge cases (duplicate/missing ID, empty stack/queue, duplicate in
/// plan) and several cross-component interactions.
///
/// Fixture: xUnit creates a fresh instance of this class for every test, so the
/// constructor gives each test its own clean manager with no shared state.
/// </summary>
public sealed class PartATests
{
    private readonly RecipeManager _manager;

    public PartATests()
    {
        _manager = new RecipeManager(SampleRecipes());
    }

    private static List<Recipe> SampleRecipes() => new()
    {
        new Recipe
        {
            Id = 1,
            Title = "Pancakes",
            Ingredients = new() { "flour", "milk", "egg" },
            Instructions = new() { "Mix", "Fry", "Serve" }
        },
        new Recipe
        {
            Id = 2,
            Title = "Toast",
            Ingredients = new() { "bread" },
            Instructions = new() { "Toast bread" }
        },
        new Recipe
        {
            Id = 3,
            Title = "Salad",
            Ingredients = new() { "lettuce", "tomato" }
            // no instructions on purpose
        }
    };
    // ---------- Constructor / Dictionary ----------

    [Fact]
    public void Constructor_NullRecipes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void Constructor_DuplicateId_Throws()
    {
        var recipes = new[]
        {
            new Recipe { Id = 1, Title = "A" },
            new Recipe { Id = 1, Title = "B" }
        };
        Assert.Throws<ArgumentException>(() => new RecipeManager(recipes));
    }

    [Fact]
    public void Constructor_NonPositiveId_Throws()
    {
        var recipes = new[] { new Recipe { Id = 0, Title = "A" } };
        Assert.Throws<ArgumentException>(() => new RecipeManager(recipes));
    }

    [Fact]
    public void Constructor_BlankTitle_Throws()
    {
        var recipes = new[] { new Recipe { Id = 1, Title = "   " } };
        Assert.Throws<ArgumentException>(() => new RecipeManager(recipes));
    }

    [Fact]
    public void Constructor_BuildsCatalogueFromInput()
    {
        Assert.Equal(3, _manager.RecipeCount);
        Assert.Equal("Pancakes", _manager.FindRecipe(1)?.Title);
    }

    [Fact]
    public void AddRecipe_NewId_ReturnsTrueAndStores()
    {
        Assert.True(_manager.AddRecipe(new Recipe { Id = 99, Title = "New" }));
        Assert.Equal(4, _manager.RecipeCount);
        Assert.Equal("New", _manager.FindRecipe(99)?.Title);
    }


}