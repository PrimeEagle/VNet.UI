namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class DialogServiceOptions
    {
        public bool EnableDialogEffects { get; set; } = true;
        public bool EnableDarkenEffect { get; set; } = true;
        public bool EnableBlurEffect { get; set; } = true;
        public bool EnableDialogEffectAnimation { get; set; } = true;
        public double DialogEffectOpacity { get; set; } = 0.2;
        public double DialogEffectBlurRadius { get; set; } = 5;
        public double DialogEffectAnimationDuration { get; set; } = 150;
    }
}