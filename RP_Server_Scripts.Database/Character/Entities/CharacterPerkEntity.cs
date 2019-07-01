using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace RP_Server_Scripts.Database.Character
{
    [DebuggerDisplay("CharacterPerk:{" + nameof(PerkName) + "}")]
    [Table("CharacterPerk")]
    public class CharacterPerkEntity
    {
        [Key]
        public int CharacterPerkId { get; set; }

        [Index(IsUnique = true)]
        [StringLength(450)]
        public string PerkName { get; set; }
    }
}
