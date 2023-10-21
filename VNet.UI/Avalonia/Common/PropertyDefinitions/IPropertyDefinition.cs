using VNet.UI.Avalonia.Common.PropertyEditors;
using VNet.UI.Avalonia.Common.ReflectionStrategies;

namespace VNet.UI.Avalonia.Common.PropertyDefinitions;

public interface IPropertyDefinition
{
    IPropertyEditor Editor { get; }
    IReflectionStrategy ReflectionStrategy { get; }


    IPropertyEditor CreateEditor();
}