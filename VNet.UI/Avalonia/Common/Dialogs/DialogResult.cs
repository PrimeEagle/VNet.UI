namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class DialogResult<TViewModel> where TViewModel : IDialogViewModel
    {
        public string ButtonResult { get; }
        public TViewModel ViewModel { get; }

        public DialogResult(string buttonResult, TViewModel viewModel)
        {
            ButtonResult = buttonResult;
            ViewModel = viewModel;
        }
    }
}