using System;
using System.Collections.Generic;
using GUC.GUI;
using GUC.Scripts.Menus;
using GUC.Scripts.Sumpfkraut.GUI.MainMenu;
using WinApi.User.Enumeration;
using GUC.Types;
using GUC.Scripts.Sumpfkraut.Controls;
using RP_Shared_Script;


namespace GUC.Scripts.Sumpfkraut.Menus.MainMenus
{
    /// <summary>
    /// Recreation of the classic Gothic main menu.
    /// </summary>
    abstract class GUCMainMenu : GUCMenu, IClosableMenu
    {
        protected GUCVisual Back;
        protected GUCVisual helpVis;
        protected GUCVisualText helpText => helpVis.Texts[0];
        protected List<MainMenuItem> Items = new List<MainMenuItem>();
        protected int cursor = 0;
        protected int[] pos;
        protected int preferredCursorItem = 0;

        public event GenericEventHandler<GUCMainMenu> OnEscape;

        public MainMenuItem CurrentItem { get; private set; }

        KeyHoldHelper scrollHelper;

        public event GenericEventHandler<GUCMainMenu> CursorChanged;

        public GUCMainMenu()
        {
            var screenSize = GUCView.GetScreenSize();
            pos = new int[] { (screenSize.X - 640) / 2, (screenSize.Y - 480) / 2 };
            Back = new GUCVisual(pos[0], pos[1], 640, 480);
            Back.SetBackTexture("Menu_Ingame.tga");
            Back.Font = GUCVisual.Fonts.Menu;

            helpVis = GUCVisualText.Create("", 0, pos[1] + 455);
            helpText.CenteredX = true;

            scrollHelper = new KeyHoldHelper()
            {
                { () => MoveCursor(true), VirtualKeys.Up },
                { () => MoveCursor(false), VirtualKeys.Down },
                { () => MoveCursor(false), VirtualKeys.Tab },
            };
        }

        private bool init = false;
        protected abstract void OnCreate();

        protected bool isOpen = false;
        public bool IsOpen { get { return this.isOpen; } }


        public override void Open()
        {
            CloseActiveMenus(); //main menus never overlap


            //Ju: This whole delayed creation is not required anymore because we do not use static menus anymore...
            if (!init)
            {   //create items on first opening, otherwise pointers to 'Open()-Methods' of other menus, which are yet not constructed, will be used => crash
                //could also be solved with static Open()-Methods
                OnCreate();
                init = true;
            }

            base.Open();
            Back.Show();
            helpVis.Show();
            for (int i = 0; i < Items.Count; i++)
                Items[i].Show();

            cursor = preferredCursorItem;
            if (cursor < Items.Count && cursor >= 0)
            {
                CurrentItem = Items[cursor];
                if (!CurrentItem.Enabled)
                {
                    sndEnabled = false; //sound would be played on opening
                    MoveCursor();
                    sndEnabled = true;
                }
                CurrentItem.Select();
                UpdateHelpText();
            }
            isOpen = true;
        }

        public override void Close()
        {
            base.Close();
            Back.Hide();
            helpVis.Hide();
            for (int i = 0; i < Items.Count; i++)
                Items[i].Hide();

            CurrentItem?.Deselect();

            isOpen = false;
        }

        protected MainMenuButton AddButton(string text, string help, int y, Action OnActivate)
        {   // X-centered version
            var b = new MainMenuButton(text, help, pos[1] + y, OnActivate);
            Items.Add(b);
            return b;
        }

        protected MainMenuButton AddButton(string text, string help, int x, int y, Action OnActivate)
        {
            var b = new MainMenuButton(text, help, pos[0] + x, pos[1] + y, OnActivate);
            Items.Add(b);
            return b;
        }

        protected MainMenuTextBox AddTextBox(string title, string help, int y, int width, Action OnActivate, bool isPassword = false)
        { //centered version
            const int borderOffset = 70;
            var tb = new MainMenuTextBox(title, help, pos[0] + 640 - width - borderOffset, pos[1] + y, width, pos[0] + borderOffset, OnActivate, isPassword);
            Items.Add(tb);
            return tb;
        }


        protected MainMenuTextBox AddTextBox(string title, string help, int x, int y, int width, int titleX, Action OnActivate, bool isPassword = false)
        {
            var tb = new MainMenuTextBox(title, help, pos[0] + x, pos[1] + y, width, pos[0] + titleX, OnActivate, isPassword);
            Items.Add(tb);
            return tb;
        }

        protected MainMenuTextBox AddTextBox(string title, string help, int x, int y, int width, int titleX, int titleY, Action OnActivate, bool isPassword = false)
        {
            var tb = new MainMenuTextBox(title, help, pos[0] + x, pos[1] + y, width, pos[0] + titleX, pos[1] + titleY, OnActivate, isPassword);
            Items.Add(tb);
            return tb;
        }

        protected MainMenuCharacter AddCharacter(int x, int y, int w, int h)
        {
            var c = new MainMenuCharacter("", pos[0] + x, pos[1] + y, w, h);
            c.Enabled = false;
            Items.Add(c);
            return c;
        }

        protected MainMenuCharacter AddCharacter(string help, int x, int y, int w, int h)
        {
            var c = new MainMenuCharacter(help, pos[0] + x, pos[1] + y, w, h);
            Items.Add(c);
            return c;
        }
    
        protected MainMenuChoice AddChoice(string title, string help, int x, int y, Dictionary<int, string> choices, bool sorted, Action OnActivate, Action OnChange)
        {
            var c = new MainMenuChoice(title, help, pos[0] + x, pos[1] + y, choices, sorted, OnActivate, OnChange);
            Items.Add(c);
            return c;
        }

        protected void UpdateHelpText()
        {
            if (DateTime.UtcNow.Ticks > helpTextNextUpdateTime)
            {
                helpText.SetColor(ColorRGBA.White);
                helpText.Text = CurrentItem.HelpText;
            }
        }

        protected long helpTextNextUpdateTime = 0;
        public void SetHelpText(String Text)
        {
            helpText.SetColor(ColorRGBA.Red);
            helpText.Text = Text;
            helpTextNextUpdateTime = DateTime.UtcNow.Ticks + 2 * TimeSpan.TicksPerSecond;
        }

        protected void SetCursor(MainMenuItem item)
        {
            SetCursor(Items.IndexOf(item));
        }

        protected void SetCursor(int i)
        {
            if (i >= 0 && i < Items.Count)
            {
                MainMenuItem newItem = Items[i];
                if (newItem.Enabled)
                {
                    CurrentItem?.Deselect();
                    cursor = i;
                    CurrentItem = newItem;
                    CurrentItem.Select();
                    UpdateHelpText();
                }

                CursorChanged?.Invoke(this);
            }
        }

        protected void MoveCursor()
        {
            MoveCursor(false);
        }

        protected void MoveCursor(bool up)
        {
            //We do not have any items so we can not change the selected item.
            if ((Items?.Count ?? 0) <= 0)
            {
                return;
            }

            CurrentItem?.Deselect();


            for (int i = 0; i < Items.Count; i++)
            {
                if (up)
                {
                    cursor--;
                    if (cursor < 0)
                        cursor = Items.Count - 1;
                }
                else
                {
                    cursor++;
                    if (cursor >= Items.Count)
                        cursor = 0;
                }

                CurrentItem = Items[cursor];
                if (CurrentItem.Enabled)
                {
                    CurrentItem.Select();
                    UpdateHelpText();
                    break;
                }
            }
            PlaySound(sndBrowse);
            CursorChanged?.Invoke(this);
        }



        protected override void KeyPress(VirtualKeys key, bool hold)
        {
            //Do not react if the current menu is not open.
            if (!isOpen)
            {
                return;
            }

            long now = GameTime.Ticks;
            switch (key)
            {
                case VirtualKeys.Return:
                    if (!hold)
                    {
                        //Check the index or we will get into trouble if no item does exist
                        if (cursor >= 0 && Items.Count > cursor)
                        {
                            Items[cursor].OnActivate?.Invoke();
                            PlaySound(sndSelect);
                        }
                    }
                    break;
                case VirtualKeys.Escape:
                    if (!hold)
                    {
                        //I dont think closing is a good default behavior for pressing escape. For now lets just invoke the event.
                        //this.Close();
                        OnEscape?.Invoke(this);
                        PlaySound(sndEscape);
                    }
                    break;
                default:
                    if (CurrentItem is InputReceiver rec)
                        rec.KeyPressed(key);
                    break;
            }
        }

        protected override void Update(long now)
        {
            scrollHelper.Update(now);
            if (cursor < Items.Count && Items[cursor] is MainMenuTextBox tb)
                tb.Update(now); // for cursor
        }

        bool sndEnabled = true;
        void PlaySound(SoundDefinition snd)
        {
            if (sndEnabled)
            {
                SoundHandler.PlaySound(snd, 1.5f);
            }
        }

        static readonly SoundDefinition sndBrowse = new SoundDefinition("MENU_BROWSE");
        static readonly SoundDefinition sndSelect = new SoundDefinition("MENU_SELECT");
        static readonly SoundDefinition sndEscape = new SoundDefinition("MENU_ESC");
    }
}
