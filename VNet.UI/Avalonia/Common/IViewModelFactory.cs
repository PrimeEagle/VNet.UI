namespace VNet.UI.Avalonia.Common
{
    public interface IViewModelFactory
    {
        public T Create<T>() where T : IViewModelBase;
    }
}