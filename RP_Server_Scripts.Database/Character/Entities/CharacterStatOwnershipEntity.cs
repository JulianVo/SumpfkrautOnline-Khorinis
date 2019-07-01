using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RP_Server_Scripts.Database.Character
{
    [Table("CharacterStatOwnership")]
    public class CharacterStatOwnershipEntity
    {
        [Key]
        public int CharacterStatOwnershipId { get; set; }

        [Required]
        public virtual CharacterEntity Owner { get; set; }

        [Required]
        public virtual CharacterStatEntity Stat { get; set; }

        [Range(0, int.MaxValue)]
        [Required]
        public int Value { get; set; }
    }
}
