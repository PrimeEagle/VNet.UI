using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace VNet.UI.Avalonia.Common
{
    public class ViewService : IViewService
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TView GetView<TView>() where TView : Window
        {
            return _serviceProvider.GetRequiredService<TView>();
        }
    }
}