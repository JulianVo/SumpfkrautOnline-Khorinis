using GUC.GUI;

namespace GUC.Scripts.Menus
{
    internal sealed class WaitScreen : MenuWithViewBlocker
    {
        private GUCVisualText _Text;

        public WaitScreen()
        {
            _Text = Back.CreateTextCenterX("...", 50);
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
