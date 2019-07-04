using System;
using GUC.Scripts.Arena;

namespace GUC.Scripts.Menus.CharacterCreationGUI
{
    internal sealed class CharacterCreationCompletedArgs
    {
        public CharacterCreationCompletedArgs(CharCreationInfo charCreationInfo)
        {
            CharCreationInfo = charCreationInfo ?? throw new ArgumentNullException(nameof(charCreationInfo));
        }

        public  CharCreationInfo CharCreationInfo { get; }
    }
}
