using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RP_Server_Scripts.Database.Character
{
    [Table("CharacterItemOwnerships")]
    public class CharacterItemOwnershipEntity
    {
        [Key]
        public int OwnershipId { get; set; }

        [Required]
        public virtual CharacterEntity Owner { get; set; }

        [Required]
        public virtual CharacterItemEntity Item { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int Amount { get; set; }
    }
}
