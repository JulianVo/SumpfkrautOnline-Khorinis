using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RP_Server_Scripts.Database.Character
{
    [Table("CharacterPerkOwnerShip")]
    public class CharacterPerkOwnershipEntity
    {
        [Key]
        public int PerkOwnersPerkOwnershipId { get; set; }

        [Required]
        public virtual CharacterEntity Owner { get; set; }

        [Required]
        public virtual CharacterPerkEntity Perk { get; set; }
    }
}
