namespace VNet.UI.Services
{
    public interface IViewModelFactoryService
    {
        T Create<T>() where T : IViewModel;
        Task<T> CreateAsync<T>() where T : IViewModel;
    }
}