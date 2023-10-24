namespace VNet.UI.Avalonia.Common.Dialogs
{
    public class DialogServiceOptions
    {
        public bool EnableEffects { get; set; } = true;
        public bool EnableParentBlurring { get; set; } = true;
        public bool EnableParentDarkening { get; set; } = true;
        public bool AnimateEffects { get; set; } = true;
        public double Opacity { get; set; } = 0.2;
        public double BlurRadius { get; set; } = 5;
        public double AnimationDuration { get; set; } = 150;
    }
}