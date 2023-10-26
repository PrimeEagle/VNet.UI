using Avalonia.Controls;

namespace VNet.UI.Avalonia.Common
{
    public interface IViewService
    {
        TView GetView<TView>() where TView : Window;
    }
}