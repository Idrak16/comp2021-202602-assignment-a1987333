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
}