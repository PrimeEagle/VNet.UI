namespace VNet.UI.Avalonia.CategoryDefinitions
{
    public class CategoryByAttribute : ICategoryDefinition
    {
        public string Name { get; }
        public int DisplayOrder { get; }


        public CategoryByAttribute(string attributeName)
        {
            Name = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
        }

        public override bool Equals(object? obj)
        {
            return obj is CategoryByAttribute category &&
                   Name == category.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}