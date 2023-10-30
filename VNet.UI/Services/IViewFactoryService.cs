namespace VNet.UI.Services;

public interface IViewFactoryService
{
    T Create<T>() where T : IView;
    Task<T> CreateAsync<T>() where T : IView;
}