# Design

Complete this document for **Part B**.

## 1. Saved recipe collection

Name the C# collection you selected for the saved/favourite feature and explain why it suits add, remove, duplicate prevention and membership checks.

For the saved/favourite feature I chose a `HashSet<int>` that stores the IDs of saved recipes. A HashSet suits all four operations this feature needs. Adding and removing are both average O(1) because the set is hash-based rather than something that has to be searched through. Duplicate prevention comes for free since a HashSet only ever holds distinct values, and `Add` returns `false` if the ID is already saved, so a recipe cannot be saved twice without me writing any extra checking code. Membership checks are the same story — `IsRecipeSaved` simply calls `HashSet.Contains`, which is an average O(1) hash lookup instead of scanning every saved ID like a `List` would. I store only the recipe ID rather than the whole `Recipe` object, since the Part A `Dictionary` catalogue is already the single source of truth for recipe data, and because the order of saved IDs is not assessed, the fact that a HashSet does not guarantee order does not matter here.

## 2. Integration

All of the Part B features work off the Part A `RecipeManager` and the recipe catalogue it already builds, so I didn't need to add a second place to store recipes. Both LINQ searches and the highest-protein report run over `_recipes.Values`, which is the same `Dictionary<int, Recipe>` that Part A fills when the JSON is loaded, so whatever is in the catalogue at the time is what gets searched. The saved-recipe feature only keeps IDs, and `AddSavedRecipe` checks `_recipes.ContainsKey(recipeId)` before saving anything, so you can't save a recipe that isn't actually in the catalogue. That keeps the dictionary as the one place that decides which recipes exist, and the Part B features just sit on top of the Part A structures instead of replacing them.


## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | Your chosen collection | | Explain how your collection performs membership checks. |
