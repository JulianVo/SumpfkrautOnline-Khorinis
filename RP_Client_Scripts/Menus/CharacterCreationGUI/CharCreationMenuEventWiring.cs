using GUC.Scripts.Character;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.CharacterCreationGUI;
using GUC.Scripts.Sumpfkraut.Networking;

namespace GUC.Scripts.GuiEventWiring
{
    internal sealed class CharCreationMenuEventWiring
    {
        public CharCreationMenuEventWiring(CharCreationMenu charCreationMenu, MainMenu mainMenu, ScriptClient client, CharacterCreation characterCreation, WaitScreen waitScreen, CharacterList characterList)
        {
            charCreationMenu.CharacterCreationCompleted += (sender, args) =>
            {
                sender.Close();
                characterCreation.StartCharacterCreation(args.CharCreationInfo);
                waitScreen.Message = "Charaktererstellung läuft...";
                waitScreen.Open();
            };

            characterCreation.CharacterCreationFailed += (sender, args) =>
            {
                charCreationMenu.SetHelpText(args.ReasonText);
                charCreationMenu.Open();
                waitScreen.Close();
            };

            characterCreation.CharacterCreationSuccessful += sender =>
            {
                waitScreen.Message = "Lade Charakterliste...";
                waitScreen.Open();
                characterList.RefreshFromServer();
            };

            charCreationMenu.OnEscape += sender =>
            {
                sender.Close();
                mainMenu.Open();
            };
        }
    }
}
