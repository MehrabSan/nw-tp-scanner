using System.Drawing;

namespace TPS.Core;

public static class Categories
{
    public static List<Category> Root = [];
    public static List<Category> All = [];
    public static readonly Category Resources;
    public static readonly Category Tools;
    public static readonly Category Consumables;
    //public static readonly Category Ammo;
    public static readonly Category Furniture;

    static Categories()
    {
        Root.Add(new Category("WEAPONS", 0));
        Root.Add(new Category("TOOLS", 1)
        {
            //Children = 
            //[
            //    new Category("Gathering Tools", 0),
            //    new Category("Instruments", 0),
            //]
        });
        Root.Add(new Category("APPAREL", 2));
        Root.Add(new Category("RESOURCES", 3)
        {
            Children =
            [
                new Category("Raw Resources", 0),
                new Category("Arcana", 1),
                new Category("Potion Reagents", 2),
                new Category("Cooking Ingredients", 3),
                new Category("Dye Ingredients", 4),
                new Category("Refined Resources", 5),
                new Category("Components", 6),
                new Category("Craft Mods", 7),
                new Category("Azoth", 8),
                new Category("Runeglass Components", 9),
            ]
        });
        Root.Add(new Category("CONSUMABLES", 4){
            Children =
            [
                //new Category("Potions", 0),
                //new Category("Tinctures", 1),
                //new Category("Coatings", 2),
                //new Category("Enhancements", 3),
                //new Category("Attribute Bonus Foods", 4),
                //new Category("Trade Skill Bonus Foods", 5),
                //new Category("Recovery Foods", 6),
                //new Category("Event Consumables", 7),
                //new Category("Bait", 8),
                //new Category("Dyes", 9),
                //new Category("Cooking Recipes", 10),
                //new Category("Music Sheets", 11),
            ]
        });
        //Root.Add(new Category("AMMUNITION", 5));
        Root.Add(new Category("HOUSE FURNISHINGS", 5));

        foreach (var category in Root)
        {
            All.Add(category);
            foreach (var child in category.Children)
            {
                child.Parent = category;
                All.Add(child);
            }
        }

        Resources = All.First(c => c.Name == "RESOURCES");
        Tools = All.First(c => c.Name == "TOOLS");
        Consumables = All.First(c => c.Name == "CONSUMABLES");
        Furniture = All.First(c => c.Name == "HOUSE FURNISHINGS");
    }
}

public class Category(string name, int order)
{
    public string Name { get; set; } = name;
    public int Order { get; set; } = order;
    public List<Category> Children { get; set; } = [];
    public Category Parent { get; set; }
}