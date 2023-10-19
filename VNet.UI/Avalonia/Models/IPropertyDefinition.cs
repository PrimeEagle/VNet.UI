namespace VNet.UI.Avalonia.Models;

public interface IPropertyDefinition
{
    IPropertyGridEditor Editor { get; }
    IReflectionStrategy ReflectionStrategy { get; }
    bool UpdateModelFromControl { get; }
    bool UpdateControlFromModel { get; }
    ICategoryDefinition Category { get; }
    IPropertyGridEditor CreateEditor();
    int DisplayOrder { get; }
}