using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VNet.UI.Avalonia.Common.Dialogs;

public partial class DialogBase : Window
{
    public DialogBase()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void InitializeView()
    {
        InitializeComponent();
    }
}