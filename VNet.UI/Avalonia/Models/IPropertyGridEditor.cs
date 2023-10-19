using Avalonia.Controls;

namespace VNet.UI.Avalonia.Models;

public interface IPropertyGridEditor
{
    object Value { get; set; }
    event EventHandler ValueChanged;
    string Tag { get; set; }
    Control GetEditorControl();
}