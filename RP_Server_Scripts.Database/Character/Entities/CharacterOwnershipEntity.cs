using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RP_Server_Scripts.Database.Account;

namespace RP_Server_Scripts.Database.Character
{
    [Table("CharacterOwnership")]
    public class CharacterOwnershipEntity
    {
        [Key]
        public int CharacterOwnershipId { get; set; }

        public virtual AccountEntity Owner { get; set; }

        [ForeignKey("Owner")]
        public virtual int OwnerId { get; set; }

        public virtual CharacterEntity Character { get; set; }

        [ForeignKey("Character")]
        public virtual int CharacterId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }=DateTime.Now;

        [Required]
        public DateTime ExpiryDate { get; set; }=DateTime.MaxValue;

        public bool DeleteCharacterOnExpiration { get; set; } = false;
    }
}
