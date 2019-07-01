using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RP_Server_Scripts.Database.Character
{
    [Table("CharacterTemporaryEffectOwnership")]
    public class CharacterTemporaryEffectOwnershipEntity
    {
        [Key]
        public int TemporaryEffectOwnershipId { get; set; }

        [Required]
        public virtual CharacterEntity Owner { get; set; }

        [Required]
        public virtual CharacterTemporaryEffectEntity Effect { get; set; }
    }
}
