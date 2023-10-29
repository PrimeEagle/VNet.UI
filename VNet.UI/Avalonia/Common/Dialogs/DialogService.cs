using Avalonia.Controls;
using Microsoft.Extensions.Options;
using VNet.UI.Services;

#pragma warning disable CA2208
// ReSharper disable ConvertToAutoProperty


namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly Dictionary<Type, Type> _dialogMappings = new();
        private readonly DialogServiceOptions _options;
        private readonly IViewFactoryService _viewFactory;

        public DialogServiceOptions Options => _options;

        public DialogService(IOptions<DialogServiceOptions> options, IViewFactoryService viewFactory)
        {
            _options = options.Value;
            _viewFactory = viewFactory;
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
            where TViewModel : class, IDialogViewModel
        {
            if (!_dialogMappings.TryGetValue(typeof(TViewModel), out var viewType))
            {
                throw new InvalidOperationException($"No dialog type was registered for the view model type {typeof(TViewModel).FullName}");
            }

            // Check if viewType is a Window and has a constructor that accepts a TViewModel.
            var constructor = viewType.GetConstructor(new[] { typeof(TViewModel) }) ?? throw new InvalidOperationException($"The view {viewType} does not have a constructor that accepts a parameter of type {typeof(TViewModel)}.");

            // Create an instance of the view, passing the view model to the constructor.
            var dialog = (Window) constructor.Invoke(new object[] {viewModel});

            if (dialog == null) throw new ArgumentNullException();

            var tcs = new TaskCompletionSource<DialogResult<TViewModel>>();
            viewModel.Completed += CompletedHandler;

            try
            {
                var showDialogTask = dialog.ShowDialog(parentContext.GetParentWindow());
                parentContext.ApplyEffects();
                await Task.Delay(100);
                await showDialogTask;
            }
            catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
            {
                throw;
            }
            finally
            {
                viewModel.Completed -= CompletedHandler;
                parentContext.RevertEffects();
            }

            return await tcs.Task;

            void CompletedHandler(object? sender, DialogResult<IDialogViewModel> result)
            {
                tcs.SetResult(new DialogResult<TViewModel>(result.ButtonResult, (TViewModel) result.ViewModel));
            }
        }
    }
}