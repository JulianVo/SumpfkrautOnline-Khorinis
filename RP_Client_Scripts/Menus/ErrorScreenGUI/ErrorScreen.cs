using GUC.GUI;
using GUC.Types;

namespace GUC.Scripts.Menus
{
    internal class ErrorScreen : MenuWithViewBlocker
    {
        private readonly GUCVisualText _Text;

        public ErrorScreen()
        {
            _Text = Back.CreateTextCenterX("...", 50);
            _Text.SetColor(ColorRGBA.Red);
            Back.CreateTextCenterX("mit Esc schließen.", 150);
        }

        public string Message
        {
            get => _Text?.Text ?? string.Empty;
            set
            {
                if (value != null && _Text != null)
                {
                    _Text.Text = value;
                }
            }
        }
    }
}
