using Avalonia.Controls;
using Microsoft.Extensions.Options;
#pragma warning disable CA2208

// ReSharper disable ConvertToAutoProperty


namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly Dictionary<Type, Type> _dialogMappings = new();
        private readonly DialogServiceOptions _options;
        private readonly IViewFactory _viewFactory;

        public DialogServiceOptions Options => _options;

        public DialogService(IOptions<DialogServiceOptions> options, IViewFactory viewFactory)
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

            var method = typeof(IViewFactory).GetMethod(nameof(IViewFactory.Create));
            var generic = method?.MakeGenericMethod(viewType);
            var viewInstance = generic?.Invoke(_viewFactory, null);

            if(viewInstance is not Window dialog) throw new ArgumentNullException();

            dialog.DataContext = viewModel;

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (dialog is IDialogWindow dialogWindow)
            {
                dialogWindow.InitializeComponent();
            }
            
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
                tcs.SetResult(new DialogResult<TViewModel>(result.ButtonResult, (TViewModel)result.ViewModel));
            }
        }
    }
}