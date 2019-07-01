using GUC.Scripts.ReusedClasses;
using RP_Server_Scripts.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RP_Server_Scripts.Character.Transaction;
using RP_Server_Scripts.Character.Transaction.AddCharacterOwnerShip;
using RP_Shared_Script;

namespace RP_Server_Scripts.Character
{
    public sealed class CharacterService
    {

        private readonly object _Lock = new object();

        private readonly Dictionary<string, Character> _LoadedCharacters = new Dictionary<string, Character>();

        public event GenericEventHandler<CharacterService, CharacterCreatedArgs> CharacterCreated;


        internal CreateHumanPlayerCharacterTransaction CreateHumanPlayerCharacterTransaction { get; set; }

        internal CheckCharacterExistsTransaction CharacterExistsTransaction { get; set; }

        internal AddCharacterOwnerShipTransaction AddCharacterOwnerShipTransaction { get; set; }

        internal GetAccountOwnedCharactersTransaction GetAccountOwnedCharactersTransaction { get; set; }

        internal SetAccountActiveCharacterTransaction SetAccountActiveCharacterTransaction { get; set; }

        internal GetAccountActiveCharacterTransaction GetAccountActiveCharacterTransaction { get; set; }

        internal GetCharacterOwnershipsCountTransaction GetCharacterOwnershipsCountTransaction { get; set; }


        public Task<CharacterCreationResult> CreateHumanPlayerCharacterAsync(Account creator, CharCreationInfo creationInfo) =>
            CreateHumanPlayerCharacterTransaction.CreateHumanPlayerCharacterAsync(creator, creationInfo);

        public bool IsCharacterLoaded(string characterName)
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(characterName));
            }
            lock (_Lock)
            {
                return _LoadedCharacters.ContainsKey(characterName.ToLower());
            }
        }


        /// <summary>
        /// Checks whether a character with a given name does exist.
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public Task<bool> CheckCharacterExistsAsync(string characterName)
        {
            return CharacterExistsTransaction.CheckCharacterExistsAsync(characterName);
        }

        public Task<AddCharacterOwnershipResult> AddCharacterOwnershipAsync(Account owner, Character character, DateTime expirationDate,
            bool deleteOnExpiration) =>
            AddCharacterOwnerShipTransaction.AddCharacterOwnershipAsync(owner, character, expirationDate,
                deleteOnExpiration);




        public Task AddCharacterOwnershipAsync(Account owner, Character character)
        {
            return AddCharacterOwnershipAsync(owner, character, DateTime.MaxValue, false);
        }

        public Task<IList<Character>> GetAccountOwnedCharactersAsync(Account owner) =>
            GetAccountOwnedCharactersTransaction.GetAccountOwnedCharactersAsync(owner);

        public Task SetAccountActiveCharacterAsync(Account account, Character character)
        {
            return SetAccountActiveCharacterTransaction.SetAccountActiveCharacterAsync(account, character);
        }

        public Task<ActiveCharacterResult> GetAccountActiveCharacterTransactionAsync(Account account) =>
            GetAccountActiveCharacterTransaction.GetAccountActiveCharacterAsync(account);

        public Task<int> GetCharacterOwnershipsCount(Account account) =>
            GetCharacterOwnershipsCountTransaction.GetCharacterOwnershipsCountAsync(account);


        internal void OnCharacterCreated(CharacterCreatedArgs args)
        {
            CharacterCreated?.Invoke(this, args);
        }
    }
}
