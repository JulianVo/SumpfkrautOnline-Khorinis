using System;
using System.Collections.Generic;
using System.Linq;
using GUC.GUI;
using GUC.Scripts.Character;
using GUC.Scripts.Sumpfkraut.GUI.MainMenu;
using GUC.Scripts.Sumpfkraut.Menus.MainMenus;
using RP_Shared_Script;

namespace GUC.Scripts.Menus.CharacterSelectionMenu
{
    internal sealed class CharacterSelectionMenu : MenuWithViewBlocker
    {
        private readonly int _DistanceBetweenButtons = 50;

        private readonly MainMenuButton[] _CharacterButtons = new MainMenuButton[0];
        private Character.Character[] _Characters = new Character.Character[0];

        public event GenericEventHandler<CharacterSelectionMenu, Character.Character> CharacterSelected;


        private readonly MainMenuCharacter _CharacterDisplay;
        private readonly GUCVisualText _CharacterNameText;

        public CharacterSelectionMenu()
        {
            Back.CreateTextCenterX("Charakterauswahl", 10);

            _CharacterButtons = new MainMenuButton[5];
            for (int i = 0; i < _CharacterButtons.Length; i++)
            {
                _CharacterButtons[i] = AddButton("...", "", _DistanceBetweenButtons * (i + 1), OnActivate);
                _CharacterButtons[i].Enabled = false;
            }

            //Increase the size so we can fit the character model into the menu
            var screenSize = GUCView.GetScreenSize();
            int width = 900;
            int height = 600;

            int posX = (screenSize.X - width) / 2;
            int posY = (screenSize.Y - height) / 2;


            Back.SetPos(posX, posY);
            Back.SetSize(width, height);


            _CharacterDisplay = AddCharacter("", 400, 0, 533, 400);
            _CharacterDisplay.SetVisual("HUMANS.MDS");
            _CharacterDisplay.SetAdditionalVisuals(HumBodyMeshs.HUM_BODY_BABE0.ToString(), (int)HumBodyTexs.G1Hero, HumHeadMeshs.HUM_HEAD_BABE.ToString(), (int)HumHeadTexs.FaceBabe_B_RedLocks);
            _CharacterDisplay.Enabled = false;
            _CharacterDisplay.Hide();

            _CharacterNameText = Back.CreateText("PlayerName", 650, 475);
            _CharacterNameText.Hide();

            CursorChanged += OnCursorChanged;
        }

        private void OnCursorChanged(GUCMainMenu sender)
        {
            if (cursor >= 0 && cursor < _Characters.Length)
            {
                Character.Character character = _Characters[cursor];

                _CharacterNameText.Text = character.Name;
                _CharacterNameText.Show();
                _CharacterDisplay.SetVisual(character.Template.Model.BaseDef.Visual);

                //We have custom visuals from the server because it is a human character.
                if (character is HumanCharacter humanCharacter)
                {
                    HumanCharacterVisuals visuals = humanCharacter.CharacterVisuals;
                    _CharacterDisplay.SetAdditionalVisuals(visuals.BodyMesh.ToString(), (int)visuals.BodyTex, visuals.HeadMesh.ToString(), (int)visuals.HeadTex);
                    _CharacterDisplay.SetFatness(visuals.Fatness);
                    _CharacterDisplay.SetScale(new Types.Vec3f(visuals.BodyWidth, 1.0f, visuals.BodyWidth));
                }
                else
                {
                    //Its a non human character, take the visuals from the template.
                    _CharacterDisplay.SetAdditionalVisuals(character.Template.BodyMesh, character.Template.BodyTex, character.Template.HeadMesh, character.Template.HeadTex);
                    _CharacterDisplay.SetFatness(1);
                    _CharacterDisplay.SetScale(new Types.Vec3f(1.0f, 1.0f, 1.0f));
                }
            }
        }

        public override void Open()
        {
            SetCursor(0);
            base.Open();
        }


        private void OnActivate()
        {
            if (cursor >= 0 && cursor < _Characters.Length)
            {
                CharacterSelected?.Invoke(this, _Characters[cursor]);
            }
        }

        public void SetCharacterList(IList<Character.Character> characters)
        {
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }

            foreach (var characterButton in _CharacterButtons)
            {
                characterButton.Text = "...";
                characterButton.Enabled = false;
            }


            for (int i = 0; i < Math.Min(characters.Count, _CharacterButtons.Length); i++)
            {
                _CharacterButtons[i].Text = characters[i].Name;
                _CharacterButtons[i].Enabled = true;
            }

            _Characters = characters.ToArray();
        }
    }
}
