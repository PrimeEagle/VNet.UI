using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VNet.UI.Services;

// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToAutoProperty
#pragma warning disable IDE0052
#pragma warning disable CA2208


namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly Dictionary<Type, Type> _dialogMappings = new();
        private readonly DialogServiceOptions _options;
        private readonly IViewFactoryService _viewFactory;
        private readonly ILogger<DialogService> _loggerService;


        public DialogServiceOptions Options => _options;

        public DialogService(IOptions<DialogServiceOptions> options, IViewFactoryService viewFactory, ILogger<DialogService> logger)
        {
            _options = options.Value;
            _viewFactory = viewFactory;
            _loggerService = logger;
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

            var constructor = viewType.GetConstructor(new[] { typeof(TViewModel) }) ?? throw new InvalidOperationException($"The view {viewType} does not have a constructor that accepts a parameter of type {typeof(TViewModel)}.");
            var dialog = (Window) constructor.Invoke(new object[] {viewModel}) ?? throw new ArgumentNullException();
            var tcs = new TaskCompletionSource<DialogResult<TViewModel>>();
            viewModel.Completed += CompletedHandler;

            try
            {
                var showDialogTask = dialog.ShowDialog(parentContext.GetParentWindow());
                parentContext.ApplyEffects();
                await Task.Delay(100).ConfigureAwait(false);
                await showDialogTask.ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
            {
                _loggerService.LogError(ex, "There was an error showing the dialog.");
                throw;
            }
            finally
            {
                viewModel.Completed -= CompletedHandler;
                parentContext.RevertEffects();
            }

            return await tcs.Task.ConfigureAwait(false);

            void CompletedHandler(object? sender, DialogResult<IDialogViewModel> result)
            {
                tcs.SetResult(new DialogResult<TViewModel>(result.ButtonResult, (TViewModel) result.ViewModel));
            }
        }
    }
}