using VNet.UI.Avalonia.CategoryDefinitions;
using VNet.UI.Avalonia.PropertyEditors;
using VNet.UI.Avalonia.ReflectionStrategies;

namespace VNet.UI.Avalonia.PropertyDefinitions;

public class PropertyDefinition : IPropertyDefinition
{
    public IPropertyEditor Editor { get; private set; }
    public bool UpdateModelFromControl { get; private set; }
    public bool UpdateControlFromModel { get; private set; }
    public ICategoryDefinition Category { get; }
    public int DisplayOrder { get; }
    public IReflectionStrategy ReflectionStrategy => throw new NotImplementedException();

    public PropertyDefinition(IPropertyEditor editor, bool updateModelFromControl, bool updateControlFromModel)
    {
        Editor = editor;
        UpdateModelFromControl = updateModelFromControl;
        UpdateControlFromModel = updateControlFromModel;
    }

    public IPropertyEditor CreateEditor()
    {
        throw new NotImplementedException();
    }
}