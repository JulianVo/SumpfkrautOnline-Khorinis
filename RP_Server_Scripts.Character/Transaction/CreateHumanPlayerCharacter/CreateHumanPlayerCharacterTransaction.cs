using System;
using System.Threading.Tasks;
using GUC.Scripts.ReusedClasses;
using RP_Server_Scripts.Database;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Threading;
using RP_Shared_Script;

namespace RP_Server_Scripts.Character.Transaction
{
    internal class CreateHumanPlayerCharacterTransaction
    {
        private readonly CharacterService _CharacterService;
        private readonly ICharacterNameValidator _NameValidator;
        private readonly ICharacterManagementContextFactory _ContextFactory;
        private readonly ISpawnPointProvider _SpawnPointProvider;
        private readonly ICharacterTemplateSelector _CharacterTemplateSelector;
        private readonly CharacterBuilder _CharacterBuilder;
        private readonly IMainThreadDispatcher _Dispatcher;
        private readonly ILogger _Log;

        internal CreateHumanPlayerCharacterTransaction(
            CharacterService characterService,
            ICharacterNameValidator nameValidator,
            ILoggerFactory loggerFactory,
            ICharacterManagementContextFactory contextFactory,
            ISpawnPointProvider spawnPointProvider,
            ICharacterTemplateSelector characterTemplateSelector,
           CharacterBuilder characterBuilder,
            IMainThreadDispatcher dispatcher)
        {
            _CharacterService = characterService;
            _NameValidator = nameValidator ?? throw new ArgumentNullException(nameof(nameValidator));
            _ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _SpawnPointProvider = spawnPointProvider ?? throw new ArgumentNullException(nameof(spawnPointProvider));
            _CharacterTemplateSelector = characterTemplateSelector ?? throw new ArgumentNullException(nameof(characterTemplateSelector));
            _CharacterBuilder = characterBuilder ?? throw new ArgumentNullException(nameof(characterBuilder));
            _Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _Log = loggerFactory.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public Task<CharacterCreationResult> CreateHumanPlayerCharacterAsync(CharCreationInfo creationInfo)
        {
            if (creationInfo == null)
            {
                throw new ArgumentNullException(nameof(creationInfo));
            }

            return _CharacterService.CheckCharacterExistsAsync(creationInfo.Name).ContinueWith<CharacterCreationResult>(task =>
            {
                //Validate the creation info because it comes directly from the client(we do not trust the client).
                if (!creationInfo.Validate(out string invalidProperty, out object value))
                {
                    _Log.Warn($"Character creation failed due to invalid {nameof(CharCreationInfo)}, invalid attribute is '{invalidProperty}' with value '{value}'");
                    return new CharacterCreationFailed(CharacterCreationFailure.InvalidCreationInfo);
                }

                //Check whether the name can be used for a character.
                if (!_NameValidator.IsValid(creationInfo.Name))
                {
                    return (CharacterCreationResult)new CharacterCreationFailed(CharacterCreationFailure.NameIsInvalid);
                }

                //The character does already exist. 
                if (task.Result)
                {
                    return (CharacterCreationResult)new CharacterCreationFailed(CharacterCreationFailure
                        .AlreadyExists);
                }

                //Create the new character
                try
                {
                    using (CharacterManagementContext db = _ContextFactory.Create())
                    {

                        SpawnPoint spawn = _SpawnPointProvider.GetSpawnPoint();

                        //Add the character
                        var characterEntity = new CharacterEntity
                        {
                            CharacterName = creationInfo.Name,
                            PositionX = spawn.Point.X,
                            PositionY = spawn.Point.Y,
                            PositionZ = spawn.Point.Z,
                            Rotation = spawn.Rotation.Yaw,
                            WorldName = spawn.World.Path,
                            TemplateName = _CharacterTemplateSelector.GetTemplate(creationInfo)
                        };
                        db.Characters.Add(characterEntity);

                        //Add the visuals of the character.
                        var customVisuals = new CharacterCustomVisualsEntity
                        {
                            OwnerCharacter = characterEntity,
                            BodyMesh = creationInfo.BodyMesh,
                            BodyTex = creationInfo.BodyTex,
                            HeadMesh = creationInfo.HeadMesh,
                            HeadTex = creationInfo.HeadTex,
                            Voice = creationInfo.Voice,
                            Fatness = creationInfo.Fatness,
                            BodyWidth = creationInfo.BodyWidth
                        };
                        db.CustomVisuals.Add(customVisuals);

                        //Save the changes.
                        db.SaveChanges();


                        Character character = _CharacterBuilder.HumanCharacterFromEntities(characterEntity, customVisuals);

                        //Invoke the character creation event(later so we can inform the client first)
                        _Dispatcher.EnqueueAction(() =>
                        {
                            _CharacterService.OnCharacterCreated(new CharacterCreatedArgs(character));
                        });

                        //Return information about successful character creation.
                        return new CharacterCreationSuccess(character);
                    }
                }
                catch (Exception e)
                {
                    throw new DatabaseRequestException("Something went wrong while adding the new character to the database.", e);
                }
            });
        }
    }
}
