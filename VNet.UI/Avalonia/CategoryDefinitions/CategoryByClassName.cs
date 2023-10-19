namespace VNet.UI.Avalonia.CategoryDefinitions
{
    public class CategoryByClassName : ICategoryDefinition
    {
        public string Name { get; }

        public int DisplayOrder => throw new NotImplementedException();

        public CategoryByClassName(string className)
        {
            Name = className ?? throw new ArgumentNullException(nameof(className));
        }

        public override bool Equals(object? obj)
        {
            return obj is CategoryByClassName category &&
                   Name == category.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}