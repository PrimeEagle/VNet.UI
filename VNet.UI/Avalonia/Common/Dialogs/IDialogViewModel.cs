namespace VNet.UI.Avalonia.Common.Dialogs;

public interface IDialogViewModel
{
    event EventHandler<DialogResult<IDialogViewModel>> Completed;
}