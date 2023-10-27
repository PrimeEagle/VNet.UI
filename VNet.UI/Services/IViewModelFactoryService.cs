namespace VNet.UI.Services
{
    public interface IViewModelFactoryService
    {
        T Create<T>() where T : class;
    }
}