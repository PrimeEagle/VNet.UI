using VNet.UI.Avalonia.PropertyEditors;
using VNet.UI.Avalonia.ReflectionStrategies;

namespace VNet.UI.Avalonia.PropertyDefinitions;

public interface IPropertyDefinition
{
    IPropertyEditor Editor { get; }
    IReflectionStrategy ReflectionStrategy { get; }


    IPropertyEditor CreateEditor();
}