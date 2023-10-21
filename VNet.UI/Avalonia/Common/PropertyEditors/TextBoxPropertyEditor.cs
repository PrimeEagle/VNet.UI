using Avalonia.Controls;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace VNet.UI.Avalonia.Common.PropertyEditors
{
    public class TextBoxPropertyEditor : IPropertyEditor
    {
        private readonly TextBox _textBox;

        public TextBoxPropertyEditor()
        {
            _textBox = new TextBox();
            _textBox.TextChanged += (sender, args) => ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public object Value
        {
            get => _textBox.Text ?? null;
            set => _textBox.Text = value.ToString();
        }

        public event EventHandler ValueChanged;

        public string Tag { get; set; }

        public Control GetEditorControl() => _textBox;
    }
}