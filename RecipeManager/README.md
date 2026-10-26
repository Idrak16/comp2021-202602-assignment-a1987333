# Recipe Management System

## Supplied code

- `RecipeManagement.Core/Models/` — recipe model classes
- `RecipeManagement.Core/RecipeLoader.cs` — reads `data/recipes.json`
- `RecipeManagement.Core/IRecipeManager.cs` — public API
- `RecipeManagement.Application/` — console menu (options labelled PartA / PartB)
- `data/recipes.json` — recipe dataset

## Implemented

**Part A** — `RecipeManager.cs` using the five nominated collections:

- `Dictionary<int, Recipe>` — recipe catalogue
- `List<string>` — shopping list
- `LinkedList<int>` — cooking plan
- `Stack<int>` — removed-recipe history
- `Queue<string>` — cooking instructions

**Part B** — LINQ title and ingredient search, highest-protein report, and a saved-recipe
feature backed by a `HashSet<int>`. See `Design.md` for the collection choice, integration
and complexity notes.

## Build and test

From this folder:

```bash
dotnet build
dotnet test
dotnet run --project RecipeManagement.Application -- data/recipes.json
```

## Tests

`RecipeManagement.Tests` contains the supplied example tests, `PartATests.cs` for the five
Part A collections and their edge cases, and `PartBTests.cs` for the Part B search, report
and saved-recipe features.

## AI acknowledgement

[Your acknowledgement statement here — see the assignment specification for the required wording.]