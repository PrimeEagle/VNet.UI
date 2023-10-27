namespace VNet.UI
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : class;
    }
}