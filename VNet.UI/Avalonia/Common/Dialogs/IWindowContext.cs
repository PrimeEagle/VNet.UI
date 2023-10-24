using Avalonia.Controls;

namespace VNet.UI.Avalonia.Common.Dialogs
{
    public interface IWindowContext
    {
        Window GetParentWindow();
        void ApplyEffects();
        void RevertEffects();
    }
}