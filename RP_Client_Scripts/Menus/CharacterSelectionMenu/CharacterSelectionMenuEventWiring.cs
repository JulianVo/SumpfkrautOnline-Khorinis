namespace GUC.Scripts.Menus.CharacterSelectionMenu
{
    internal sealed class CharacterSelectionMenuEventWiring
    {
        public CharacterSelectionMenuEventWiring(CharacterSelectionMenu selectionMenu, MainMenu mainMenu, CharacterList characterList)
        {
            selectionMenu.OnEscape += sender =>
            {
                mainMenu.Open();
            };

            selectionMenu.CharacterSelected += (sender, character) =>
            {
                characterList.SetActiveCharacter(character);
                mainMenu.SetDisplayCharacter(character);
                mainMenu.Open();
                sender.Close();
            };
        }
    }
}
