namespace VNet.UI;

public interface IViewFactory
{
    T Create<T>() where T : class;
}