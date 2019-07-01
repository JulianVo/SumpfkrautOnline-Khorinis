using System.Linq;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.CharacterSelectionMenu;

namespace GUC.Scripts.GuiEventWiring
{
    internal sealed class CharacterLoadingEventWiring
    {
        public CharacterLoadingEventWiring(WaitScreen waitScreen, CharacterList characterList, MainMenu mainMenu, CharacterSelectionMenu selectionMenu)
        {
            characterList.RefreshCompleted += sender =>
            {
                mainMenu.Open();
                if (characterList.TryGetActiveCharacter(out Character.Character character))
                {
                    mainMenu.SetDisplayCharacter(character);
                }
                else
                {
                    mainMenu.HideDisplayCharacter();
                }
                selectionMenu.SetCharacterList(sender.Characters.ToList());

                waitScreen.Close();
            };
        }
    }
}
