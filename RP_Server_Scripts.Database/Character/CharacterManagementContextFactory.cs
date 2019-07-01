namespace RP_Server_Scripts.Database.Character
{
    internal class CharacterManagementContextFactory : ICharacterManagementContextFactory
    {
        public CharacterManagementContext Create()
        {
            return new CharacterManagementContext();
        }
    }
}