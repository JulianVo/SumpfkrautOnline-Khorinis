using GUC.Scripts.Arena.Menus;
using GUC.Scripts.Menus;

namespace GUC.Scripts
{
    internal sealed class ExitMenuEventWiring
    {
        public ExitMenuEventWiring(ExitMenu exitMenu, MainMenu mainMenu)
        {
            exitMenu.ExitGameSelected += sender => Program.Exit();

            exitMenu.BackToMainMenu += sender =>
            {
                sender.Close();
                mainMenu.Open();
            };

            exitMenu.OnEscape += sender => sender.Close();
        }
    }
}
