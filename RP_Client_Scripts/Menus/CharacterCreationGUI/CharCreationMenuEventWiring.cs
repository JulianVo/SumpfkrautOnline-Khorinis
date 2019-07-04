using GUC.Scripts.Character;
using GUC.Scripts.Menus.ErrorScreenGUI;

namespace GUC.Scripts.Menus.CharacterCreationGUI
{
    internal sealed class CharCreationMenuEventWiring
    {
        public CharCreationMenuEventWiring(
            CharCreationMenu charCreationMenu,
            MainMenu mainMenu,
            CharacterCreation characterCreation,
            WaitScreen waitScreen,
            CharacterList characterList,
            ErrorScreenManager errorScreenManager)
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
                waitScreen.Close();
                errorScreenManager.ShowError(args.ReasonText, charCreationMenu.Open);
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
