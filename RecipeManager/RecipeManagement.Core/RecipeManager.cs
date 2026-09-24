using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Part A implementation of the recipe-management system.
/// Uses the five nominated collections for their assigned roles:
///   Dictionary<int, Recipe> - recipe catalogue (primary lookup by ID)
///   List<string>            - shopping list of ingredient strings
///   LinkedList<int>         - cooking plan (recipe IDs in cook order)
///   Stack<int>              - history of recently removed plan recipes
///   Queue<string>           - active recipe's cooking instructions
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes = new();
    private readonly List<string> _shoppingList = new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipes = new();
    private readonly Queue<string> _instructionQueue = new();

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes is null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }

        foreach (Recipe? recipe in recipes)
        {
            if (recipe is null)
            {
                throw new ArgumentException("Recipe collection contains a null entry.", nameof(recipes));
            }

            if (recipe.Id <= 0)
            {
                throw new ArgumentException($"Recipe ID must be positive (got {recipe.Id}).", nameof(recipes));
            }

            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException($"Recipe {recipe.Id} has a blank title.", nameof(recipes));
            }

            if (!_recipes.TryAdd(recipe.Id, recipe))
            {
                throw new ArgumentException($"Duplicate recipe ID: {recipe.Id}.", nameof(recipes));
            }
        }
    }


    

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructionQueue.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe) 
        {
        if (recipe is null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (recipe.Id <= 0 || string.IsNullOrWhiteSpace(recipe.Title))
        {
            return false;
        }
        // TryAdd returns false when the key already exists, so duplicates are rejected.
        return _recipes.TryAdd(recipe.Id, recipe);
    }

    public Recipe? FindRecipe(int recipeId) =>
        _recipes.TryGetValue(recipeId, out Recipe? recipe) ? recipe : null;

    public bool RemoveRecipe(int recipeId)
    {
        // Cannot remove a recipe that is missing or still planned for cooking.
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        return _recipes.Remove(recipeId);
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        if (!_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return 0;
        }
        foreach (string j in recipe.Ingredients)
        {
            _shoppingList.Add(j);
        }
        return recipe.Ingredients.Count;
 
    }

    // Return a copy so callers cannot mutate the internal list.
    public IReadOnlyList<string> GetShoppingList() => _shoppingList.ToList();

    public void ClearShoppingList() => _shoppingList.Clear();


    public bool AddRecipeToCookingPlan(int recipeId)
    {
        //Only a nown recipe must appear, that too only once
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }
        _cookingPlan.AddLast(recipeId)
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId) =>
        throw new NotImplementedException("Part A: implement RemoveRecipeFromCookingPlan.");

    public bool RestoreLastRemovedRecipe() =>
        throw new NotImplementedException("Part A: implement RestoreLastRemovedRecipe.");

    public int? PeekLastRemovedRecipe() =>
        throw new NotImplementedException("Part A: implement PeekLastRemovedRecipe.");

    public IReadOnlyList<int> GetCookingPlan() =>
        throw new NotImplementedException("Part A: implement GetCookingPlan.");

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException("Part A: implement StartCooking.");

    public string? PeekNextInstruction() =>
        throw new NotImplementedException("Part A: implement PeekNextInstruction.");

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException("Part A: implement CompleteNextInstruction.");

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
