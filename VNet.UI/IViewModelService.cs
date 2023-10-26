namespace VNet.UI
{
    public interface IViewModelService
    {
        TViewModel GetViewModel<TViewModel>() where TViewModel : class;
    }
}