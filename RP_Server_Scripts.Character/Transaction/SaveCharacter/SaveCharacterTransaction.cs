using System;
using System.Threading.Tasks;
using RP_Server_Scripts.Database.Character;

namespace RP_Server_Scripts.Character.Transaction.SaveCharacter
{
    internal sealed class SaveCharacterTransaction
    {
        private readonly ICharacterManagementContextFactory _ContextFactory;

        public SaveCharacterTransaction(ICharacterManagementContextFactory contextFactory)
        {
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Task SaveCharacterAsync(Character character)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }
            return Task.Run(() =>
            {
                try
                {
                    using (var db = _ContextFactory.Create())
                    {
                        var changedEntity = new CharacterEntity
                        {
                            CharacterId = character.CharacterId,
                            CharacterName = character.Name,
                            PositionX = character.LastKnownPosition.X,
                            PositionY = character.LastKnownPosition.Y,
                            PositionZ = character.LastKnownPosition.Z,
                            Rotation = character.Rotation.Yaw,
                            WorldName = character.World.Path,
                            TemplateName = character.Template.CodeName,
                        };
                        db.Characters.Attach(changedEntity);

                        //Create a character entity which represents the changes to the character. 
                        //Explicitly ignore the character name as changing this is currently not planed.
                        db.Entry(changedEntity).Property(ce => ce.PositionX).IsModified = true;
                        db.Entry(changedEntity).Property(ce => ce.PositionY).IsModified = true;
                        db.Entry(changedEntity).Property(ce => ce.PositionZ).IsModified = true;
                        db.Entry(changedEntity).Property(ce => ce.Rotation).IsModified = true;
                        db.Entry(changedEntity).Property(ce => ce.WorldName).IsModified = true;
                        db.Entry(changedEntity).Property(ce => ce.TemplateName).IsModified = true;

                        //Save the changes
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }
    }
}
