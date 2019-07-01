using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RP_Server_Scripts.Database.Account;

namespace RP_Server_Scripts.Database.Character
{
    [Table("AccountLastUsedCharacter")]
    public class AccountLastUsedCharacterEntity
    {
        [Key]
        public int Id { get; set; }
        
        public virtual AccountEntity Account { get; set; }

        [ForeignKey("Account")]
        public virtual int AccountId { get; set; }

        public virtual CharacterEntity Character { get; set; }

        [ForeignKey("Character")]
        public virtual int CharacterId { get; set; }
    }
}
