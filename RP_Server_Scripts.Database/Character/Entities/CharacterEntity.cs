using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using RP_Server_Scripts.Database.Account;

namespace RP_Server_Scripts.Database.Character
{
    [DebuggerDisplay("Character:{CharacterId}:{CharacterName}")]
    [Table("Characters")]
    public class CharacterEntity
    {
        [Key]
        public int CharacterId { get; set; }


        [Required]
        public string TemplateName { get; set; }

        [Index(IsUnique = true)]
        [StringLength(450)]
        [Required]
        public string CharacterName { get; set; }

        [Required]
        public string WorldName { get; set; }

        [Required]
        public float PositionX { get; set; }

        [Required]
        public float PositionY { get; set; }

        [Required]
        public float PositionZ { get; set; }

        [Required]
        public float Rotation { get; set; }
    }
}
