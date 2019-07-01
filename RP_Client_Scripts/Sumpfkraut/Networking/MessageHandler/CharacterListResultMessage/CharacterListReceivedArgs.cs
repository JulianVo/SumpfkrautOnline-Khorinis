using System;
using System.Collections.Generic;

namespace GUC.Scripts.Sumpfkraut.Networking.MessageHandler.CharacterListResultMessage
{
    public sealed class CharacterListReceivedArgs
    {
        public CharacterListReceivedArgs(IList<Character.Character> characters, int activeCharacterId)
        {
            Characters = characters ?? throw new ArgumentNullException(nameof(characters));
            ActiveCharacterId = activeCharacterId;
        }
        public int ActiveCharacterId { get; }

        public IList<Character.Character> Characters { get; }
    }
}
