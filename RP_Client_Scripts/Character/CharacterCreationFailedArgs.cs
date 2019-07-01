using System;

namespace GUC.Scripts.Character
{
    internal sealed  class CharacterCreationFailedArgs
    {
        public CharacterCreationFailedArgs(string reasonText)
        {
            if (string.IsNullOrWhiteSpace(reasonText))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(reasonText));
            }

            ReasonText = reasonText;
        }

        public string ReasonText { get; }
    }
}
