using GUC.Scripts.ReusedClasses;
using RP_Server_Scripts.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Autofac;
using RP_Server_Scripts.Character.Transaction;
using RP_Server_Scripts.Character.Transaction.AddCharacterOwnerShip;
using RP_Shared_Script;
using GUC.Types;
using RP_Server_Scripts.VobSystem.Instances;

namespace RP_Server_Scripts.Character
{
    public sealed class CharacterService
    {

        private readonly object _Lock = new object();

        private readonly Dictionary<int, Character> _LoadedCharacters = new Dictionary<int, Character>();
        private readonly Dictionary<string, Character> _LoadedCharactersByName = new Dictionary<string, Character>();


        public event GenericEventHandler<CharacterService, CharacterCreatedArgs> CharacterCreated;

        private readonly Dictionary<int, CharacterMapping> _ActiveMappings = new Dictionary<int, CharacterMapping>();

        private readonly Dictionary<Account, List<Character>> _CharactersByOwner = new Dictionary<Account, List<Character>>();

        private bool _Started;


        internal CreateHumanPlayerCharacterTransaction CreateHumanPlayerCharacterTransaction { get; set; }

        internal CheckCharacterExistsTransaction CharacterExistsTransaction { get; set; }

        internal AddCharacterOwnerShipTransaction AddCharacterOwnerShipTransaction { get; set; }

        internal GetAccountOwnedCharactersTransaction GetAccountOwnedCharactersTransaction { get; set; }

        internal SetAccountActiveCharacterTransaction SetAccountActiveCharacterTransaction { get; set; }

        internal GetAccountActiveCharacterTransaction GetAccountActiveCharacterTransaction { get; set; }

        internal GetCharacterOwnershipsCountTransaction GetCharacterOwnershipsCountTransaction { get; set; }

        internal AuthenticationService AuthenticationService { get; set; }


        public Task<CharacterCreationResult> CreateHumanPlayerCharacterAsync(CharCreationInfo creationInfo) =>
            CreateHumanPlayerCharacterTransaction.CreateHumanPlayerCharacterAsync(creationInfo);

        public bool IsCharacterLoaded(int characterId)
        {
            lock (_Lock)
            {
                return _LoadedCharacters.ContainsKey(characterId);
            }
        }


        public bool IsCharacterLoaded(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(name));
            }

            lock (_Lock)
            {
                return _LoadedCharactersByName.ContainsKey(name);
            }
        }

        public bool TryGetLoadedCharacter(int characterId, out Character loadedCharacter)
        {
            if (characterId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(characterId));
            }

            lock (_Lock)
            {
                return _LoadedCharacters.TryGetValue(characterId, out loadedCharacter);
            }
        }

        public bool TryGetMapping(Character character, out CharacterMapping mapping)
        {
            lock (_Lock)
            {
                return _ActiveMappings.TryGetValue(character.CharacterId, out mapping);
            }
        }

        public bool TryGetMapping(NpcInst npc, out CharacterMapping mapping)
        {
            lock (_Lock)
            {
                mapping = _ActiveMappings.Values.FirstOrDefault(map => map.CharacterNpc.ID == npc.ID);
                return mapping != null;
            }
        }

        /// <summary>
        /// Checks whether a character with a given name does exist.
        /// </summary>
        /// <param name="characterName">The name of the character that should be checked for.</param>
        /// <returns>True if a character with the given name does exist, false if not.</returns>
        public Task<bool> CheckCharacterExistsAsync(string characterName)
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(characterName));
            }

            lock (_Lock)
            {
                if (_LoadedCharactersByName.ContainsKey(characterName.ToUpperInvariant()))
                {
                    return Task.FromResult(true);
                }
            }


            return CharacterExistsTransaction.CheckCharacterExistsDbAsync(characterName);
        }

        public Task<AddCharacterOwnershipResult> AddCharacterOwnershipAsync(Account owner, Character character, DateTime expirationDate, bool deleteOnExpiration) =>
            AddCharacterOwnerShipTransaction
                .AddCharacterOwnershipAsync(owner, character, expirationDate, deleteOnExpiration)
                .ContinueWith(task =>
                {
                    //The character ownership was successfully added in the database. Do the same in memory.
                    if (task.Result.Successful)
                    {
                        lock (_Lock)
                        {
                            if (!_CharactersByOwner.TryGetValue(owner, out List<Character> characters))
                            {
                                characters = new List<Character>();
                                _CharactersByOwner.Add(owner, characters);
                            }

                            characters.Add(character);
                        }
                    }

                    return task.Result;
                });




        public Task<AddCharacterOwnershipResult> AddCharacterOwnershipAsync(Account owner, Character character)
        {
            return AddCharacterOwnershipAsync(owner, character, DateTime.MaxValue, false);
        }

        public Task<IList<Character>> GetAccountOwnedCharactersAsync(Account owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            //Check memory first before accessing the database
            lock (_Lock)
            {
                if (_CharactersByOwner.TryGetValue(owner, out List<Character> characters))
                {
                    return Task.FromResult((IList<Character>)characters);
                }
            }


            //The character list of the given account is not loaded, access the database and load the characters.
            return GetAccountOwnedCharactersTransaction.GetAccountOwnedCharactersFromDbAsync(owner).ContinueWith(task =>
             {
                 lock (_Lock)
                 {
                     if (!_CharactersByOwner.TryGetValue(owner, out List<Character> characters))
                     {
                         characters = new List<Character>();
                         _CharactersByOwner.Add(owner, characters);
                     }

                     foreach (var character in task.Result)
                     {
                         _LoadedCharacters.Add(character.CharacterId, character);
                         _LoadedCharactersByName.Add(character.Name.ToUpperInvariant(), character);
                         characters.Add(character);
                     }
                 }
                 return task.Result;
             });
        }



        public Task SetAccountActiveCharacterAsync(Account account, Character character)
        {
            return SetAccountActiveCharacterTransaction.SetAccountActiveCharacterAsync(account, character);
        }

        public Task<ActiveCharacterResult> GetAccountActiveCharacterTransactionAsync(Account account) =>
            GetAccountActiveCharacterTransaction.GetAccountActiveCharacterAsync(account);

        public Task<int> GetCharacterOwnershipsCount(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            //Check if we do have the characters of this account in memory, so we do not have to access the database.
            lock (_Lock)
            {
                if (_CharactersByOwner.TryGetValue(account, out List<Character> characters))
                {
                    return Task.FromResult(characters.Count);
                }
            }

            //The characters a not loaded, access the database.
            return GetCharacterOwnershipsCountTransaction.GetCharacterOwnershipsCountFromDbAsync(account);
        }


        public CharacterMapping SpawnAndMapCharacter(Character character)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            if (character.IsMapped)
            {
                throw new InvalidOperationException($"The character '{character.Name}' ({character.CharacterId}) is already mapped and can not be mapped again!");
            }

            if (!character.IsValid)
            {
                throw new InvalidOperationException($"The given character object is invalid and should not be referenced or used anymore.");
            }

            NpcInst npc;


            if (character is HumanCharacter humanCharacter)
            {
                npc = new NpcInst(character.Template)
                {
                    UseCustoms = true,
                    CustomBodyTex = humanCharacter.HumanVisuals.BodyTex,
                    CustomHeadMesh = humanCharacter.HumanVisuals.HeadMesh,
                    CustomHeadTex = humanCharacter.HumanVisuals.HeadTex,
                    CustomVoice = humanCharacter.HumanVisuals.Voice,
                    CustomFatness = humanCharacter.HumanVisuals.Fatness,
                    CustomScale = new Vec3f(humanCharacter.HumanVisuals.BodyWidth, 1.0f, humanCharacter.HumanVisuals.BodyWidth),
                    CustomName = character.Name,
                    DropUnconsciousOnDeath = true,
                    UnconsciousDuration = 15 * TimeSpan.TicksPerSecond,
                };
            }
            else
            {
                npc = new NpcInst(character.Template)
                {
                    CustomName = character.Name,
                };
            }
            npc.Spawn(character.World, character.LastKnownPosition, character.Rotation);
            character.IsMapped = true;

            var mapping = new CharacterMapping(character, npc);

            lock (_Lock)
            {
                _ActiveMappings.Add(character.CharacterId, mapping);
            }

            return mapping;
        }

        public void RemoveMapping(Character character)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }
            if (!character.IsValid)
            {
                throw new InvalidOperationException($"The given character object is invalid and should not be referenced or used anymore.");
            }
            if (!character.IsMapped)
            {
                throw new InvalidOperationException($"The character '{character.Name}' ({character.CharacterId}) is not mapped! There is nothing to remove!");
            }


            NpcInst npc = null;
            lock (_Lock)
            {
                if (_ActiveMappings.TryGetValue(character.CharacterId, out CharacterMapping mapping))
                {
                    //Despawn the character npc and remove the mapping.
                    npc = mapping.CharacterNpc;
                    _ActiveMappings.Remove(character.CharacterId);
                    character.IsMapped = false;

                    //Check if we can completely unload the character.
                    bool ownerIsActive = _CharactersByOwner.Values.Any(list => list.Contains(character));

                    if (!ownerIsActive)
                    {
                        _LoadedCharacters.Remove(character.CharacterId);
                        _LoadedCharactersByName.Remove(character.Name.ToUpperInvariant());
                        character.IsValid = false;
                    }
                }
            }
            npc?.Despawn();
        }

        internal void OnCharacterCreated(CharacterCreatedArgs args)
        {
            CharacterCreated?.Invoke(this, args);
        }

        internal void Init()
        {
            if (_Started)
            {
                throw new InvalidOperationException($"The {nameof(CharacterService)} has already been started.");
            }

            _Started = true;

            AuthenticationService.ClientLoggedOut += (sender, args) =>
            {
                lock (_Lock)
                {
                    //An account has logged out. Remove its character ownerships from the dictionary and check if we can unload its characters.
                    if (_CharactersByOwner.TryGetValue(args.Account, out List<Character> characters))
                    {
                        _CharactersByOwner.Remove(args.Account);

                        foreach (var character in characters)
                        {
                            //Is another owner of the character active?
                            bool ownerIsActive = _CharactersByOwner.Values.Any(list => list.Contains(character));

                            //Is the character currently mapped? We can not unload a character that is mapped(A character does not need an active owner to be mapped e.g. Npc Character)
                            bool isMapped = _ActiveMappings.ContainsKey(character.CharacterId);

                            //No owner is active and the character is not mapped. Unload it(invalidate it and remove all references).
                            if (!ownerIsActive && !isMapped)
                            {
                                _LoadedCharacters.Remove(character.CharacterId);
                                _LoadedCharactersByName.Remove(character.Name.ToUpperInvariant());
                                character.IsValid = false;
                            }
                        }
                    }
                }
            };
        }
    }
}
