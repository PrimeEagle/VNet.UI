using VNet.UI.Avalonia.CategoryDefinitions;
using VNet.UI.Avalonia.PropertyEditors;
using VNet.UI.Avalonia.ReflectionStrategies;

namespace VNet.UI.Avalonia.PropertyDefinitions;

public interface IPropertyDefinition
{
    IPropertyEditor Editor { get; }
    IReflectionStrategy ReflectionStrategy { get; }
    bool UpdateModelFromControl { get; }
    bool UpdateControlFromModel { get; }
    ICategoryDefinition Category { get; }
    int DisplayOrder { get; }


    IPropertyEditor CreateEditor();
}