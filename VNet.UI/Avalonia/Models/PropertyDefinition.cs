namespace VNet.UI.Avalonia.Models;

public class PropertyDefinition : IPropertyDefinition
{
    public IPropertyGridEditor Editor { get; private set; }
    public bool UpdateModelFromControl { get; private set; }
    public bool UpdateControlFromModel { get; private set; }
    public ICategoryDefinition Category { get; }
    public int DisplayOrder { get; }
    public IReflectionStrategy ReflectionStrategy => throw new NotImplementedException();

    public PropertyDefinition(IPropertyGridEditor editor, bool updateModelFromControl, bool updateControlFromModel)
    {
        Editor = editor;
        UpdateModelFromControl = updateModelFromControl;
        UpdateControlFromModel = updateControlFromModel;
    }

    public IPropertyGridEditor CreateEditor()
    {
        throw new NotImplementedException();
    }
}