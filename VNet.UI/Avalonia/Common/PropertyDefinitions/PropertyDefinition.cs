using VNet.UI.Avalonia.Common.PropertyEditors;
using VNet.UI.Avalonia.Common.ReflectionStrategies;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace VNet.UI.Avalonia.Common.PropertyDefinitions;

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