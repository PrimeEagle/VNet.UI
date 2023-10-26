using Microsoft.Extensions.DependencyInjection;

namespace VNet.UI
{
    public class ViewModelService : IViewModelService
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TViewModel GetViewModel<TViewModel>() where TViewModel : class
        {
            return _serviceProvider.GetRequiredService<TViewModel>();
        }
    }
}