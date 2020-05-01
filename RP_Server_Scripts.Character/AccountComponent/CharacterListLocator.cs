using System;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Component;

namespace RP_Server_Scripts.Character.AccountComponent
{
    internal sealed class CharacterListLocator : IComponentLocator<Account>
    {
        private readonly CharacterService _CharacterService;

        public CharacterListLocator(CharacterService characterService)
        {
            _CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
        }

        public object GetComponent(Account ownerInstance)
        {
            if (ownerInstance == null)
            {
                throw new ArgumentNullException(nameof(ownerInstance));
            }

            return new CharacterList(_CharacterService, ownerInstance);
        }

        public bool TryGetComponent(Account ownerInstance, out object component)
        {
            if (ownerInstance == null)
            {
                throw new ArgumentNullException(nameof(ownerInstance));
            }

            component = new CharacterList(_CharacterService, ownerInstance);
            return true;
        }

        public Type SupportedType => typeof(CharacterList);
    }
}
