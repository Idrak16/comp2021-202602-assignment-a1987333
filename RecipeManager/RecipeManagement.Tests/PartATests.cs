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
    [Fact]
    public void AddRecipe_DuplicateId_ReturnsFalse()
    {
        Assert.False(_manager.AddRecipe(new Recipe { Id = 1, Title = "Dup" }));
        Assert.Equal(3, _manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_InvalidIdOrTitle_ReturnsFalse()
    {
        Assert.False(_manager.AddRecipe(new Recipe { Id = -5, Title = "Bad" }));
        Assert.False(_manager.AddRecipe(new Recipe { Id = 50, Title = "" }));
    }

    [Fact]
    public void AddRecipe_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _manager.AddRecipe(null!));
    }

    [Fact]
    public void FindRecipe_MissingId_ReturnsNull()
    {
        Assert.Null(_manager.FindRecipe(1234));
    }

    [Fact]
    public void RemoveRecipe_Existing_ReturnsTrue()
    {
        Assert.True(_manager.RemoveRecipe(2));
        Assert.Null(_manager.FindRecipe(2));
        Assert.Equal(2, _manager.RecipeCount);
    }

    [Fact]
    public void RemoveRecipe_Missing_ReturnsFalse()
    {
        Assert.False(_manager.RemoveRecipe(999));
    }

    [Fact]
    public void RemoveRecipe_WhileInCookingPlan_ReturnsFalse()
    {
        _manager.AddRecipeToCookingPlan(1);
        Assert.False(_manager.RemoveRecipe(1));
        Assert.NotNull(_manager.FindRecipe(1));
    }

    // ---------- Shopping list (List) ----------

    [Fact]
    public void AddIngredients_CopiesInOrder_AndReturnsCount()
    {
        int added = _manager.AddIngredientsToShoppingList(1);
        Assert.Equal(3, added);
        Assert.Equal(new[] { "flour", "milk", "egg" }, _manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredients_MissingRecipe_ReturnsZero()
    {
        Assert.Equal(0, _manager.AddIngredientsToShoppingList(999));
        Assert.Empty(_manager.GetShoppingList());
    }

    [Fact]
    public void ClearShoppingList_EmptiesList()
    {
        _manager.AddIngredientsToShoppingList(1);
        _manager.ClearShoppingList();
        Assert.Empty(_manager.GetShoppingList());
        Assert.Equal(0, _manager.ShoppingItemCount);
    }

    [Fact]
    public void GetShoppingList_DoesNotExposeInternalList()
    {
        _manager.AddIngredientsToShoppingList(2);
        var returned = _manager.GetShoppingList();
        if (returned is List<string> mutable)
        {
            mutable.Add("hacked");
        }
        // Mutating the returned copy must not change the manager's own list.
        Assert.Single(_manager.GetShoppingList());
    }

    // ---------- Cooking plan (LinkedList) ----------

    [Fact]
    public void AddToCookingPlan_AppendsInOrder()
    {
        Assert.True(_manager.AddRecipeToCookingPlan(1));
        Assert.True(_manager.AddRecipeToCookingPlan(2));
        Assert.Equal(new[] { 1, 2 }, _manager.GetCookingPlan());
    }

    [Fact]
    public void AddToCookingPlan_MissingRecipe_ReturnsFalse()
    {
        Assert.False(_manager.AddRecipeToCookingPlan(999));
    }

    [Fact]
    public void AddToCookingPlan_Duplicate_ReturnsFalse()
    {
        _manager.AddRecipeToCookingPlan(1);
        Assert.False(_manager.AddRecipeToCookingPlan(1));
        Assert.Equal(new[] { 1 }, _manager.GetCookingPlan());
    }
    // ---------- Removed history (Stack) ----------

    [Fact]
    public void RemoveFromPlan_PushesToStack_LifoRestore()
    {
        _manager.AddRecipeToCookingPlan(1);
        _manager.AddRecipeToCookingPlan(2);
        _manager.RemoveRecipeFromCookingPlan(1);
        _manager.RemoveRecipeFromCookingPlan(2);

        Assert.Equal(2, _manager.PeekLastRemovedRecipe());
        Assert.True(_manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 2 }, _manager.GetCookingPlan());

        Assert.True(_manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 2, 1 }, _manager.GetCookingPlan());
    }

    [Fact]
    public void RemoveFromPlan_NotPlanned_ReturnsFalse_StackUnchanged()
    {
        Assert.False(_manager.RemoveRecipeFromCookingPlan(1));
        Assert.Equal(0, _manager.RemovedRecipeCount);
    }

    [Fact]
    public void PeekLastRemoved_EmptyStack_ReturnsNull()
    {
        Assert.Null(_manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RestoreLastRemoved_EmptyStack_ReturnsFalse()
    {
        Assert.False(_manager.RestoreLastRemovedRecipe());
    }

     // ---------- Cooking instructions (Queue) ----------

    [Fact]
    public void StartCooking_LoadsInstructionsFifo()
    {
        Assert.True(_manager.StartCooking(1));
        Assert.Equal(3, _manager.PendingInstructionCount);
        Assert.Equal("Mix", _manager.PeekNextInstruction());
        Assert.Equal("Mix", _manager.CompleteNextInstruction());
        Assert.Equal("Fry", _manager.CompleteNextInstruction());
        Assert.Equal("Serve", _manager.CompleteNextInstruction());
    }

    [Fact]
    public void StartCooking_ReplacesPreviousQueue()
    {
        _manager.StartCooking(1);
        _manager.CompleteNextInstruction();
        Assert.True(_manager.StartCooking(2));
        Assert.Equal(1, _manager.PendingInstructionCount);
        Assert.Equal("Toast bread", _manager.PeekNextInstruction());
    }

    [Fact]
    public void StartCooking_NoInstructions_ReturnsFalse()
    {
        Assert.False(_manager.StartCooking(3));
    }

    [Fact]
    public void StartCooking_MissingRecipe_ReturnsFalse()
    {
        Assert.False(_manager.StartCooking(999));
    }

    [Fact]
    public void EmptyQueue_PeekAndComplete_ReturnNull()
    {
        Assert.Null(_manager.PeekNextInstruction());
        Assert.Null(_manager.CompleteNextInstruction());
    }

      // ---------- Interactions between components ----------

    [Fact]
    public void Interaction_PlanRemoveRestoreThenCook()
    {
        _manager.AddRecipeToCookingPlan(1);
        _manager.AddRecipeToCookingPlan(2);

        Assert.True(_manager.RemoveRecipeFromCookingPlan(1));
        Assert.Equal(new[] { 2 }, _manager.GetCookingPlan());

        Assert.True(_manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 2, 1 }, _manager.GetCookingPlan());

        Assert.True(_manager.StartCooking(1));
        Assert.Equal("Mix", _manager.CompleteNextInstruction());
        Assert.Equal(2, _manager.PendingInstructionCount);
    }

      [Fact]
    public void Interaction_ShoppingListBuiltFromCatalogue()
    {
        _manager.AddIngredientsToShoppingList(1);
        _manager.AddIngredientsToShoppingList(2);
        Assert.Equal(4, _manager.ShoppingItemCount);
        Assert.Equal(new[] { "flour", "milk", "egg", "bread" }, _manager.GetShoppingList());
    }

    [Fact]
    public void Interaction_FullLifecycle_AddPlanCookComplete()
    {
        // A brand-new recipe should flow through every collection cleanly.
        Assert.True(_manager.AddRecipe(new Recipe
        {
            Id = 42,
            Title = "Omelette",
            Ingredients = new() { "egg", "butter" },
            Instructions = new() { "Beat eggs", "Cook" }
        }));

        Assert.Equal(2, _manager.AddIngredientsToShoppingList(42));
        Assert.True(_manager.AddRecipeToCookingPlan(42));
        Assert.Equal(new[] { 42 }, _manager.GetCookingPlan());

        Assert.True(_manager.StartCooking(42));
        Assert.Equal("Beat eggs", _manager.CompleteNextInstruction());
        Assert.Equal("Cook", _manager.CompleteNextInstruction());
        Assert.Null(_manager.CompleteNextInstruction());
    }
    [Fact]
    public void Interaction_RemoveFromCatalogueAfterPlanRemoval_RestoreFails()
    {
        // Plan a recipe, take it back off the plan (goes to the stack),
        // then delete it from the catalogue. Restoring the now-deleted recipe
        // must fail because the dictionary no longer knows it.
        _manager.AddRecipeToCookingPlan(1);
        _manager.RemoveRecipeFromCookingPlan(1);

        Assert.True(_manager.RemoveRecipe(1));      // allowed: not in the plan anymore
        Assert.False(_manager.RestoreLastRemovedRecipe());
        Assert.Empty(_manager.GetCookingPlan());
    }
    

    


}