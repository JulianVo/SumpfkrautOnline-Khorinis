using System.Data.Entity;
using RP_Server_Scripts.Database.Account;

namespace RP_Server_Scripts.Database.Character
{
    public class CharacterManagementContext:DbContext
    {
        internal CharacterManagementContext() : base("name=GothicRpDB_ConnectionString")
        {
        }
        public DbSet<AccountEntity> Accounts { get; set; }

        public DbSet<CharacterEntity> Characters { get; set; }

        public DbSet<CharacterOwnershipEntity> CharacterOwnerships { get; set; }

        public DbSet<CharacterCustomVisualsEntity> CustomVisuals { get; set; }

        public DbSet<CharacterItemEntity> CharacterItems { get; set; }

        public DbSet<CharacterItemOwnershipEntity> CharacterItemOwnerships { get; set; }

        public DbSet<CharacterPerkEntity> CharacterPerks { get; set; }

        public DbSet<CharacterPerkOwnershipEntity> CharacterPerkOwnerships { get; set; }

        public DbSet<CharacterStatEntity> CharacterStats { get; set; }

        public DbSet<CharacterStatOwnershipEntity> CharacterStatOwnerships { get; set; }

        public DbSet<CharacterTemporaryEffectEntity> CharacterTemporaryEffects { get; set; }

        public DbSet<CharacterTemporaryEffectOwnershipEntity> CharacterTemporaryEffectOwnerships { get; set; }

        public DbSet<AccountLastUsedCharacterEntity> LastUsedCharacters { get; set; }
    }
}
