using GUC.GUI;
using GUC.Scripts.Sumpfkraut.Menus.MainMenus;

namespace GUC.Scripts.Menus
{
    internal abstract class MenuWithViewBlocker : GUCMainMenu
    {
        private readonly GUCVisual _BackgroundTexture;

        protected MenuWithViewBlocker()
        {
            var screenSize = GUCView.GetScreenSize();
            _BackgroundTexture = new GUCVisual(0, 0, screenSize.X, screenSize.Y);
            _BackgroundTexture.SetBackTexture("StartScreen.tga");
            _BackgroundTexture.Font = GUCVisual.Fonts.Menu;
        }

        protected override void OnCreate()
        {
            //Do nothing. With this we do not need to implement this method in classes that inherit from this class.
        }

        public override void Open()
        {
            _BackgroundTexture?.Show();
            base.Open();
        }

        public override void Close()
        {
            _BackgroundTexture?.Hide();
            base.Close();
        }
    }
}
