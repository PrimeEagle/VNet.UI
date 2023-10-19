// ReSharper disable MemberCanBePrivate.Global
namespace VNet.UI.Avalonia.CategoryDefinitions
{
    public class CustomCategory : ICategoryDefinition
    {
        public string Name { get; private set; }
        public int DisplayOrder { get; private set; }

        public CustomCategory(string name)
        {
            Name = name;
            DisplayOrder = CategoryDisplayOrder.GetOrder(name);
        }
    }

    public static class CategoryDisplayOrder
    {
        public static readonly Dictionary<string, int> Orders = new()
        {
            { "General", 1 },
            { "Advanced", 2 },
        };

        public static int GetOrder(string categoryName)
        {
            return Orders.TryGetValue(categoryName, out var order) ? order : int.MaxValue;
        }
    }
}