using Avalonia.Controls;

namespace VNet.UI.Avalonia.PropertyEditors;

public interface IPropertyEditor
{
    object Value { get; set; }
    event EventHandler ValueChanged;
    string Tag { get; set; }
    Control GetEditorControl();
}