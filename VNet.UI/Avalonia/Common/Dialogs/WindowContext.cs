using Avalonia.Controls;
using Microsoft.Extensions.Options;

namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class WindowContext : IWindowContext
    {
        private readonly Window _parentWindow;
        private readonly UiEffectManager _uiEffectManager;

        public WindowContext(Window parentWindow, DialogServiceOptions options)
        {
            _parentWindow = parentWindow;
            _uiEffectManager = new UiEffectManager(parentWindow, options);
        }

        public void ApplyEffects()
        {
            _uiEffectManager.ApplyEffects();
        }

        public void RevertEffects()
        {
            _uiEffectManager.RevertEffects();
        }

        public Window GetParentWindow()
        {
            return _parentWindow;
        }
    }
}