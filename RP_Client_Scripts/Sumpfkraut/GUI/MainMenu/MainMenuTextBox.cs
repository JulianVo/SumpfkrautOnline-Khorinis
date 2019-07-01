using System;
using WinApi.User.Enumeration;
using GUC.GUI;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.GUI.MainMenu
{
    class MainMenuTextBox : MainMenuItem, InputReceiver
    {

        const string BackTexture = "Menu_Choice_Back.tga";
        const int height = 30;

        GUCVisual vis;
        GUCVisual titleVis;
        GUCTextBox tb;

        public Action<long> Update { get; protected set; }

        public bool AllowSymbols { set => tb.AllowSymbols = value; }
        public string Input
        {
            get => tb.Input;
            set
            {
                if (value != null)
                {
                    tb.Input = value;
                }
            }
        }

        public MainMenuTextBox(string help, int x, int y, int width, Action action, bool isPassword = false)
            : this("", help, x, y, width, 0, 0, action, isPassword) { }

        public MainMenuTextBox(string title, string help, int x, int y, int width, int titleX, Action action, bool isPassword = false)
            : this(title, help, x, y, width, titleX, y + (height - FontsizeMenu) / 2, action, isPassword) { }

        public MainMenuTextBox(string title, string help, int x, int y, int width, int titleX, int titleY, Action action,bool isPassword=false)
        {
            HelpText = help;
            OnActivate = action;

            //title text
            titleVis = GUCVisualText.Create(title, titleX, titleY);
            titleVis.Font = Fonts.Menu;

            //background visual
            vis = new GUCVisual(x, y, width, height); 
            vis.SetBackTexture(BackTexture);

            //text box
            if (isPassword)
            {
                tb = new GUCPasswordTextBox(x + 15, y + 5, width - 30, true);
            }
            else
            {
                tb = new GUCTextBox(x + 15, y + 5, width - 30, true);
            }

            tb.AllowSpaces = false;
            Update = tb.Update;
        }

        public override void Select()
        {
            titleVis.Font = Fonts.Menu_Hi;
            tb.Enabled = true;
        }

        public override void Deselect()
        {
            titleVis.Font = Fonts.Menu;
            tb.Enabled = false;
        }

        public override void Show()
        {
            titleVis.Show();
            vis.Show();
            tb.Show();
        }

        public override void Hide()
        {
            titleVis.Hide();
            vis.Hide();
            tb.Hide();
        }

        public void KeyPressed(VirtualKeys key)
        {
            tb.KeyPressed(key);
        }
    }
}
