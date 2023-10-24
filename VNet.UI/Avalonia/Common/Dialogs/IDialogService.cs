using Avalonia.Controls;

namespace VNet.UI.Avalonia.Common.Dialogs;

public interface IDialogService
{
    public Task<DialogResult<TViewModel>> ShowDialogAsync<TViewModel>(TViewModel viewModel, IWindowContext parentContext)
        where TViewModel : class, IDialogViewModel, new();

    public void RegisterDialog<TViewModel, TView>()
        where TViewModel : class, IDialogViewModel
        where TView : Window;
}