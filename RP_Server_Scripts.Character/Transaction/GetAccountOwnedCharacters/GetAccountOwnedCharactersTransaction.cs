using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Database;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.Logging;

namespace RP_Server_Scripts.Character.Transaction
{
    internal sealed class GetAccountOwnedCharactersTransaction
    {
        private readonly ICharacterManagementContextFactory _ContextFactory;
        private readonly CharacterBuilder _CharacterBuilder;
        private readonly ILogger _Log;

        public GetAccountOwnedCharactersTransaction(ICharacterManagementContextFactory contextFactory, CharacterBuilder characterBuilder, ILoggerFactory loggerFactory)
        {
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _CharacterBuilder = characterBuilder ?? throw new ArgumentNullException(nameof(characterBuilder));
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public Task<IList<Character>> GetAccountOwnedCharactersAsync(Account owner)
        {
            var task = new Task<IList<Character>>(() =>
            {
                try
                {
                    using (var db = _ContextFactory.Create())
                    {
                        var characters = new List<Character>();

                        //Call to list or we will get problems with trying to create multiple result sets on the same connection without using the MARS-Mode(Multiple Active Result Sets).
                        foreach (var characterEntity in db.CharacterOwnerships.Where(os => os.Owner.AccountId == owner.AccountId).Select(os => os.Character).ToList())
                        {
                            var humanVisuals = db.CustomVisuals.FirstOrDefault(vis =>
                                vis.OwnerCharacter.CharacterId == characterEntity.CharacterId);

                            //If we have human visuals in the database, we create a human character.
                            characters.Add(humanVisuals != null
                                ? _CharacterBuilder.HumanCharacterFromEntities(characterEntity, humanVisuals)
                                : _CharacterBuilder.NonHumanCharacterFromEntities(characterEntity));
                        }

                        return characters;
                    }
                }
                catch (Exception e)
                {
                    string msg =
                        $"Something went wrong while requesting the characters of account '{owner.UserName}' form the database. Exception: {e}";
                    _Log.Error(msg);
                    throw new DatabaseRequestException(msg, e);
                }
            });
            task.Start();
            return task;
        }
    }
}
