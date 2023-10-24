using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class UiEffectManager
    {
        private readonly Window _parentWindow;
        private readonly Control _parentContent;
        private readonly object _originalContent;
        private readonly IEffect _originalEffect;
        private Border _overlay;

        public UiEffectManager(Window parentWindow)
        {
            _parentWindow = parentWindow;
            _parentContent = (Control)parentWindow.Content;
            _originalContent = parentWindow.Content;
            _originalEffect = _parentContent.Effect;
        }

        public void ApplyEffects()
        {
            //ApplyBlur();
            ApplyDarken();
        }

        private void ApplyBlur()
        {
            _parentContent.Effect = new BlurEffect { Radius = 5 };
        }

        private async void ApplyDarken()
        {
            //DoubleTransition gridTransition = new DoubleTransition();
            //gridTransition.Duration = TimeSpan.FromSeconds(10);
            //gridTransition.Property = global::Avalonia.Visual.OpacityProperty;

            // For mainPanel fades
            //DoubleTransition borderTransition = new DoubleTransition();
            //borderTransition.Duration = TimeSpan.FromSeconds(10);
            //borderTransition.Property = global::Avalonia.Visual.OpacityProperty;

            // Create a new style
            var overlayStyle = new Style(x => x.OfType<Border>());

            // Define the transition
            var transitions = new Transitions();
            transitions.Add(new DoubleTransition
            {
                Property = Border.OpacityProperty,
                Duration = TimeSpan.FromSeconds(5), // or whatever duration you prefer
            });

            _overlay = new Border
            {
                Background = new SolidColorBrush(Colors.Black, 0),
                IsHitTestVisible = true,
                Opacity = 0
            };

            _overlay.Styles.Add(overlayStyle);

            // Set the "Transitions" property through the style
            overlayStyle.Setters.Add(new Setter(Border.TransitionsProperty, transitions));

            // Optionally, you might want to set an initial Opacity as well
            overlayStyle.Setters.Add(new Setter(Border.OpacityProperty, 0)); // Initial opacity

            //var transition = new DoubleTransition()
            //{
            //    Property = Border.OpacityProperty,
            //    Duration = TimeSpan.FromMilliseconds(2000),
            //    Easing = new LinearEasing()
            //};

            // Add to the visual tree
            var grid = new Grid();
            grid.Children.Add(_overlay);
            _parentWindow.Content = grid;

            // Trigger the transition after the element is rendered, possibly with a slight delay
            await Task.Delay(100); // This delay is arbitrary; you may need to adjust this.
            _overlay.Opacity = 0.7; // Or whatever your target opacity value is.


            //_overlay = new Border
            //{
            //    Background = new SolidColorBrush(Colors.Black, 0),
            //    IsHitTestVisible = true,
            //    Opacity = 0
            //};
            //_overlay.Transitions = new Transitions();
            //_overlay.Transitions.Add(borderTransition);

            //var grid = new Grid();
            //grid.Transitions = new Transitions();

            //grid.Children.Add(_overlay);
            //_parentWindow.Content = grid;

            //await Task.Delay(1000);
            //_parentWindow.UpdateLayout();
            //_overlay.Opacity = 0.2;
            //await FadeIn(_overlay);
        }

        //private async void ApplyDarken()
        //{
        //    // Create an animation instance
        //    var animation = new Animation
        //    {
        //        Duration = TimeSpan.FromSeconds(10), // Define the same duration
        //        FillMode = FillMode.Both, // Maintain the animation's state after it finishes
        //        IterationCount = new IterationCount(1), // Run the animation once
        //    };

        //    // Define what property the animation applies to and the keyframes
        //    animation.Children.Add(new KeyFrame
        //    {
        //        Setters =
        //        {
        //            new Setter
        //            {
        //                Property = Visual.OpacityProperty,
        //                Value = 0.2d, // The target opacity value
        //            },
        //        },
        //        Cue = new Cue(1f) // The time cue (1f represents the end of the animation timeline)
        //    });

        //    _overlay = new Border
        //    {
        //        Background = new SolidColorBrush(Colors.Black),
        //        IsHitTestVisible = true,
        //        Opacity = 0 // Set initial opacity to 0
        //    };

        //    var grid = new Grid();
        //    grid.Children.Add(_overlay);

        //    _parentWindow.Content = grid;

        //    // Now, apply and start the animation
        //    await animation.RunAsync(_overlay);
        //}

        private async Task FadeIn(Border border)
        {
            const int animationSteps = 3;
            const double targetOpacity = 0.2;
            const int animationDuration = 150; // in milliseconds

            for (int step = 1; step <= animationSteps; step++)
            {
                double opacity = step * targetOpacity / animationSteps;
                border.Background = new SolidColorBrush(Colors.Black, opacity);

                await Task.Delay(animationDuration / animationSteps);
            }
        }

        public async Task RevertEffects()
        {
            //await FadeOut(_overlay);
            _overlay.Opacity = 0;
            _parentContent.Effect = _originalEffect;
            _parentWindow.Content = _originalContent;
        }

        private async Task FadeOut(Border border)
        {
            const int animationSteps = 3;
            const double targetOpacity = 0;
            const int animationDuration = 150; // in milliseconds

            for (int step = 1; step <= animationSteps; step++)
            {
                double opacity = 1 - (step * (1 - targetOpacity) / animationSteps);
                border.Background = new SolidColorBrush(Colors.Black, opacity);

                await Task.Delay(animationDuration / animationSteps);
            }
        }
    }
}