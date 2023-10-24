using Avalonia.Controls;
using Avalonia.Media;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable MemberCanBeMadeStatic.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CA1822

namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class UiEffectManager
    {
        private readonly Window _parentWindow;
        private readonly Control _parentContent;
        private readonly object _originalContent;
        private Border _overlay;

        public UiEffectManager(Window parentWindow)
        {
            _parentWindow = parentWindow;
            _parentContent = (Control)parentWindow.Content;
            _originalContent = parentWindow.Content;
        }

        public async void ApplyEffects()
        {
            // Create an overlay with zero opacity initially
            _overlay = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                IsHitTestVisible = true,
                Opacity = 0
            };

            // Assuming _parentContent is a container control (like Panel, Grid, etc.)
            // that can have multiple children, you add the overlay to it.
            if (_parentContent is Panel parentPanel)
            {
                parentPanel.Children.Add(_overlay);
            }

            // Now, fade in the overlay for the darkening effect.
            // You don't have to set the _parentWindow.Content again since you're modifying its current content.
            await FadeIn(_overlay, _parentContent);
        }

        private async Task FadeIn(Border overlay, Control originalContent)
        {
            const int animationSteps = 3;
            const double targetOpacity = 0.2; // Final opacity for the overlay
            const double targetBlurRadius = 5; // Final blur radius for the content
            const int animationDuration = 150; // Total duration for the animation

            // Set the initial state of the overlay and blur effect
            overlay.Background = new SolidColorBrush(Colors.Black);
            var blur = new BlurEffect { Radius = 0 };
            originalContent.Effect = blur; // Applying the blur to the original content

            for (var step = 1; step <= animationSteps; step++)
            {
                var opacity = step * targetOpacity / animationSteps;
                var blurRadius = step * targetBlurRadius / animationSteps;

                // Adjusting the opacity of the overlay, not the content itself
                overlay.Opacity = opacity;

                // Increasing the blur effect on the original content
                blur.Radius = blurRadius;

                // Request the interface to redraw the updated visuals
                originalContent.InvalidateVisual();

                // Pause the execution for the step duration
                await Task.Delay(animationDuration / animationSteps);
            }
        }

        public async Task RevertEffects()
        {
            // First, animate the fading out of the overlay and the removal of the blur effect.
            await FadeOut(_overlay, _parentContent);  // This assumes FadeOut handles the animation logic correctly.

            // After the animation, restore everything to its original state.

            // Clear any remaining effects from the parent content.
            _parentContent.Effect = null;

            // Instead of setting the content again, we remove the overlay from the parent container.
            // This ensures the original content remains intact and undisturbed.
            if (_parentContent is Panel parentPanel && _overlay != null)
            {
                parentPanel.Children.Remove(_overlay);
            }

            // No need to reset _parentWindow.Content as we haven't replaced it, we've only added/removed children.
        }

        private async Task FadeOut(Border border, Control originalContent)
        {
            const int animationSteps = 3;
            const double targetOpacity = 0.2;
            const double targetBlurRadius = 5;
            const int animationDuration = 150; // in milliseconds

            for (var step = 1; step <= animationSteps; step++)
            {
                var opacity = targetOpacity - step * targetOpacity / animationSteps;
                var blurRadius = targetBlurRadius - step * targetBlurRadius / animationSteps;

                border.Opacity = opacity;

                // Safely check if the Effect is a BlurEffect and if so, change its properties.
                if (originalContent?.Effect is BlurEffect blur)
                {
                    blur.Radius = blurRadius;
                }

                originalContent?.InvalidateVisual();

                await Task.Delay(animationDuration / animationSteps);
            }

            // Assuming the effect should be removed afterward
            originalContent.Effect = null;

            // Additional cleanup if needed, for example, removing the overlay from its parent container
        }
    }
}