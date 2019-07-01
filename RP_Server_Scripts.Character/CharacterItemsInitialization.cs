using System;
using System.Linq;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Character
{
    /// <summary>
    /// Initialization class that adds all defined item types to the database so they can be referenced by ownership objects.
    /// </summary>
    internal sealed class CharacterItemsInitialization
    {
        public CharacterItemsInitialization(ICharacterManagementContextFactory contextFactory,IItemDefList itemDefList,ILoggerFactory loggerFactory)
        {
            if (contextFactory == null)
            {
                throw new ArgumentNullException(nameof(contextFactory));
            }

            if (itemDefList == null)
            {
                throw new ArgumentNullException(nameof(itemDefList));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            var logger = loggerFactory.GetLogger(GetType());


            using (var db = contextFactory.Create())
            {
                //Get items types that have already been registered in the database.
                var characterItemEntities = db.CharacterItems.ToDictionary(entity => entity.ItemName.ToUpperInvariant());

                //Add all items that are defined but not registered in the database to the database.
                foreach (var itemDef in itemDefList.AllDefinitions)
                {
                    if (!characterItemEntities.ContainsKey(itemDef.CodeName.ToUpperInvariant()))
                    {
                        db.CharacterItems.Add(
                            new CharacterItemEntity()
                            {
                                ItemName = itemDef.CodeName.ToUpperInvariant()
                            });
                        logger.Info($"Added item definition '{itemDef.CodeName.ToUpperInvariant()}'");
                    }
                }


                //Save the changes to the database.
                db.SaveChanges();
            }
        }
    }
}
