using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RP_Server_Scripts.Authentication;

namespace RP_Server_Scripts.Character.AccountComponent
{
    public sealed class CharacterList
    {
        private readonly CharacterService _CharacterService;
        private readonly Account _Account;

        public CharacterList(CharacterService characterService, Account account)
        {
            _CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
            _Account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public Task<int> GetCharacterOwnershipsCountAsync() => _CharacterService.GetCharacterOwnershipsCountAsync(_Account);

        public Task<IList<Character>> GetAccountOwnedCharactersAsync() =>
            _CharacterService.GetAccountOwnedCharactersAsync(_Account);

        public Task<ActiveCharacterResult> GetAccountActiveCharacterTransactionAsync() =>
            _CharacterService.GetAccountActiveCharacterTransactionAsync(_Account);

        public Task SetAccountActiveCharacterAsync(Character character) =>
            _CharacterService.SetAccountActiveCharacterAsync(_Account, character);
    }
}
