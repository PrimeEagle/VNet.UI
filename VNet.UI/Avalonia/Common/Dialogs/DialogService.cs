using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, Type> _dialogMappings = new();

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void RegisterDialog<TViewModel, TView>()
            where TViewModel : class, IDialogViewModel
            where TView : Window
        {
            var viewModelType = typeof(TViewModel);
            var viewType = typeof(TView);

            _dialogMappings[viewModelType] = viewType;
        }

        public async Task<DialogResult<TViewModel>> ShowDialogAsync<TViewModel>(TViewModel viewModel, IWindowContext parentContext)
            where TViewModel : class, IDialogViewModel, new()
        {
            if (!_dialogMappings.TryGetValue(typeof(TViewModel), out var viewType))
            {
                throw new InvalidOperationException($"No dialog type was registered for the view model type {typeof(TViewModel).FullName}");
            }

            var dialog = (Window)ActivatorUtilities.CreateInstance(_serviceProvider, viewType);
            dialog.DataContext = viewModel;

            if (dialog is IDialogWindow dialogWindow)
            {
                dialogWindow.InitializeComponent();
            }

            

            var tcs = new TaskCompletionSource<DialogResult<TViewModel>>();

            viewModel.Completed += CompletedHandler;

            try
            {
                // Start showing the dialog (but don't await it yet)
                var showDialogTask = dialog.ShowDialog(parentContext.GetParentWindow());

                // Apply the darkening effect to the parent
                parentContext.ApplyEffects();

                // Allow the UI to update
                await Task.Delay(100); // or more, you need to test the appropriate amount of delay

                // Now, wait for the dialog
                await showDialogTask;
            }
            catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
            {
                throw;
            }
            finally
            {
                viewModel.Completed -= CompletedHandler;

                // After the dialog work is done, we revert the effects using the same context.
                parentContext.RevertEffects();
            }

            return await tcs.Task;

            void CompletedHandler(object? sender, DialogResult<IDialogViewModel> result)
            {
                tcs.SetResult(new DialogResult<TViewModel>(result.ButtonResult, (TViewModel)result.ViewModel));
            }
        }
    }
}