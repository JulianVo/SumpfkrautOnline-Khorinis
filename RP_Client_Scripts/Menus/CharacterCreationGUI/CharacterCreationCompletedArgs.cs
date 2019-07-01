using System;

namespace GUC.Scripts.Arena.Menus
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
