using System;
using System.Collections.Generic;
using System.Linq;
using GUC.Scripts.Sumpfkraut.Networking;
using GUC.Scripts.Sumpfkraut.Networking.MessageHandler;
using RP_Shared_Script;

namespace GUC.Scripts
{
    public sealed class CharacterList
    {
        private readonly CharListRequestSender _CharListRequestSender;
        public event GenericEventHandler<CharacterList> RefreshCompleted;

        private readonly List<Character.Character> _Characters = new List<Character.Character>();
        private int _ActiveCharacter = -1;


        internal CharacterList(CharListRequestSender charListRequestSender, CharacterListResultMessageHandler characterListMessageHandler)
        {
            if (characterListMessageHandler == null)
            {
                throw new ArgumentNullException(nameof(characterListMessageHandler));
            }

            _CharListRequestSender = charListRequestSender ?? throw new ArgumentNullException(nameof(charListRequestSender));

            characterListMessageHandler.CharacterListReceived += (sender, args) =>
            {
                _Characters.Clear();
                _Characters.AddRange(args.Characters);
                _ActiveCharacter = args.ActiveCharacterId;

                RefreshCompleted?.Invoke(this);
            };
        }

        public bool TryGetActiveCharacter(out Character.Character character)
        {
            character = _Characters.FirstOrDefault(ch => ch.CharacterId == _ActiveCharacter);
            return character != null;
        }

        public void SetActiveCharacter(Character.Character character)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            _ActiveCharacter = character.CharacterId;
        }

        public IEnumerable<Character.Character> Characters
        {
            get
            {
                foreach (var character in _Characters)
                {
                    yield return character;
                }
            }
        }


        public void RefreshFromServer()
        {
            _CharListRequestSender.SendCharacterListRequest();

        }
    }
}
