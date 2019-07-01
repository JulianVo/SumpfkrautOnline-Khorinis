using System;
using System.Text.RegularExpressions;

namespace RP_Shared_Script
{
    public sealed class GermanNameValidator : ICharacterNameValidator
    {
        private readonly Regex _NameRegex = new Regex("^[a-zA-ZäÄüÜöÖ ,.'-]+$");

        public bool IsValid(string characterName)
        {
            if (characterName == null)
            {
                throw new ArgumentNullException(nameof(characterName));
            }

            return !string.IsNullOrWhiteSpace(characterName) && _NameRegex.IsMatch(characterName);
        }
    }
}