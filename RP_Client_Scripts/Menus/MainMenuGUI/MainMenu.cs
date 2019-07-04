using System;
using GUC.GUI;
using GUC.Scripts.Character;
using GUC.Scripts.Sumpfkraut.GUI.MainMenu;
using RP_Shared_Script;

namespace GUC.Scripts.Menus
{
    internal class MainMenu : MenuWithViewBlocker
    {
        public event GenericEventHandler<MainMenu> CharacterSelectionSelected;
        public event GenericEventHandler<MainMenu> CharacterCreationSelected;
        public event GenericEventHandler<MainMenu> JoinGameSelected;
        public event GenericEventHandler<MainMenu> ExitGameSelected;
        public event GenericEventHandler<MainMenu> BackToLoginSelected;

        private readonly MainMenuCharacter _Character;
        private readonly GUCVisualText _CharacterNameText;
        private bool _DisplayCharacterIsHidden;

        public MainMenu()
        {
            //Increase the size so we can fit the character model into the menu
            var screenSize = GUCView.GetScreenSize();
            int width = 900;
            int height = 600;

            int posX = (screenSize.X - width) / 2;
            int posY = (screenSize.Y - height) / 2;


            Back.SetPos(posX, posY);
            Back.SetSize(width, height);

            AddButton("Welt betreten", "Die Spielwelt mit dem gewählten Character betreten.", 50, () => JoinGameSelected?.Invoke(this));
            AddButton("Charakter wählen", "Wähle deinen Spielcharakter.", 100, () => CharacterSelectionSelected?.Invoke(this));
            AddButton("Charakter erstellen", "Erstelle einen Spielcharakter", 150, () => CharacterCreationSelected?.Invoke(this));
            AddButton("Logout", "Zurück zum Login Menü", 200, () => BackToLoginSelected?.Invoke(this));
            AddButton("Spiel verlassen", "Das Spiel schließen.", 250, () => ExitGameSelected?.Invoke(this));


            _Character = AddCharacter("", 400, 0, 533, 400);
            _Character.SetVisual("HUMANS.MDS");
            _Character.SetAdditionalVisuals(HumBodyMeshs.HUM_BODY_BABE0.ToString(), (int)HumBodyTexs.G1Hero, HumHeadMeshs.HUM_HEAD_BABE.ToString(), (int)HumHeadTexs.FaceBabe_B_RedLocks);
            _Character.Enabled = false;
            _Character.Hide();

            _CharacterNameText = Back.CreateText("PlayerName", 650, 475);
            _CharacterNameText.Hide();
        }

        public void SetDisplayCharacter(Character.Character character)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            _CharacterNameText.Text = character.Name;
            _CharacterNameText.Show();
            _Character.SetVisual(character.Template.Model.BaseDef.Visual);

            //We have custom visuals from the server because it is a human character.
            if (character is HumanCharacter humanCharacter)
            {
                HumanCharacterVisuals visuals = humanCharacter.CharacterVisuals;
                _Character.SetAdditionalVisuals(visuals.BodyMesh.ToString(), (int)visuals.BodyTex, visuals.HeadMesh.ToString(), (int)visuals.HeadTex);
                _Character.SetFatness(visuals.Fatness);
                _Character.SetScale(new Types.Vec3f(visuals.BodyWidth, 1.0f, visuals.BodyWidth));
            }
            else
            {
                //Its a non human character, take the visuals from the template.
                _Character.SetAdditionalVisuals(character.Template.BodyMesh, character.Template.BodyTex, character.Template.HeadMesh, character.Template.HeadTex);
                _Character.SetFatness(1);
                _Character.SetScale(new Types.Vec3f(1.0f, 1.0f, 1.0f));
            }

            _DisplayCharacterIsHidden = false;
        }

        public void HideDisplayCharacter()
        {
            _Character.Hide();
            _CharacterNameText.Hide();
            _DisplayCharacterIsHidden = true;
        }

        public override void Open()
        {
            base.Open();
            if (_DisplayCharacterIsHidden)
            {
                HideDisplayCharacter();
            }
        }
    }
}
