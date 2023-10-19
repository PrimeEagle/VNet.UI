using VNet.UI.Avalonia.CategoryDefinitions;
using VNet.UI.Avalonia.PropertyEditors;
using VNet.UI.Avalonia.ReflectionStrategies;
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace VNet.UI.Avalonia.PropertyDefinitions;

public class PropertyDefinition : IPropertyDefinition
{
    public IPropertyEditor Editor { get; init; }
    public IReflectionStrategy ReflectionStrategy { get; init; }




    public PropertyDefinition()
    {
    }

    public IPropertyEditor CreateEditor()
    {
        throw new NotImplementedException();
    }
}