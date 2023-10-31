using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

// ReSharper disable SuggestBaseTypeForParameterInConstructor
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable MemberCanBeMadeStatic.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8601 // Possible null reference assignment.


namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class UiEffectManager
    {
        private readonly Control _parentContent;
        private Border _overlay;
        private readonly DialogServiceOptions _options;




        public UiEffectManager(Window parentWindow, DialogServiceOptions options)
        {
            _parentContent = (Control)parentWindow.Content;
            _options = options;
        }

        public async void ApplyEffects()
        {
            if (!_options.EnableDialogEffects) return;
            
            _overlay = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                IsHitTestVisible = true,
                Opacity = 0
            };

            if (_parentContent is Panel parentPanel)
            {
                parentPanel.Children.Add(_overlay);
            }

            await FadeIn(_overlay, _parentContent);
        }

        private async Task FadeIn(Border overlay, Control originalContent)
        {
            const int animationSteps = 5;
            var targetOpacity = _options.DialogEffectOpacity;
            var targetBlurRadius = _options.DialogEffectBlurRadius;
            var animationDuration = _options.DialogEffectAnimationDuration;

            if (!_options.EnableDialogEffectAnimation)
            {
                if (_options.EnableDarkenEffect) overlay.Opacity = targetOpacity;
                if (_options.EnableBlurEffect) ((BlurEffect)originalContent.Effect).Radius = targetBlurRadius;
                return;
            }

            overlay.Background = new SolidColorBrush(Colors.Black);
            var blur = new BlurEffect { Radius = 0 };
            originalContent.Effect = blur;

            for (var step = 1; step <= animationSteps; step++)
            {
                var opacity = step * targetOpacity / animationSteps;
                var blurRadius = step * targetBlurRadius / animationSteps;

                overlay.Opacity = opacity;
                blur.Radius = blurRadius;

                originalContent.InvalidateVisual();

                await Task.Delay((int)animationDuration / animationSteps);
            }
        }

        public async Task RevertEffects()
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await FadeOut(_overlay, _parentContent);
                _parentContent.Effect = null;
                if (_parentContent is Panel parentPanel)
                {
                    parentPanel.Children.Remove(_overlay);
                }
            });
        }


        private async Task FadeOut(Border border, Control originalContent)
        {
            const int animationSteps = 5;
            const int targetOpacity = 0;
            const int targetBlurRadius = 0;
            var animationDuration = _options.DialogEffectAnimationDuration;

            if (!_options.EnableDialogEffectAnimation)
            {
                if(_options.EnableDarkenEffect) border.Opacity = targetOpacity;
                if(_options.EnableBlurEffect) ((BlurEffect)originalContent.Effect).Radius = targetBlurRadius;
                return;
            }

            for (var step = 1; step <= animationSteps; step++)
            {
                var opacity = targetOpacity - step * targetOpacity / animationSteps;
                var blurRadius = targetBlurRadius - step * targetBlurRadius / animationSteps;

                border.Opacity = opacity;

                if (originalContent?.Effect is BlurEffect blur)
                {
                    blur.Radius = blurRadius;
                }

                originalContent?.InvalidateVisual();
                await Task.Delay((int)animationDuration / animationSteps);
            }

            originalContent.Effect = null;
        }
    }
}