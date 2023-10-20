using System.Collections.ObjectModel;

namespace VNet.UI.Avalonia.Design
{
    public class PreferenceTabs
    {
        public ObservableCollection<string> TabNames { get; set; } = new ObservableCollection<string>()
            {
                "Basic",
                "Application",
                "Plugin",
                "Physical Constants"
            };
    }
}