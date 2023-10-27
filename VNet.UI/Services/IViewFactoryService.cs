namespace VNet.UI.Services;

public interface IViewFactoryService
{
    T Create<T>() where T : class;
}