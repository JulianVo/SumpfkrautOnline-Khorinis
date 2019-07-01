using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace RP_Server_Scripts.Database.Character
{
    [DebuggerDisplay("CharacterTemporaryEffect:{" + nameof(EffectName) + "}")]
    [Table("CharacterTemporaryEffect")]
    public class CharacterTemporaryEffectEntity
    {
        [Key]
        public int CharacterTemporaryEffect { get; set; }

        [Index(IsUnique = true)]
        [StringLength(450)]
        public string EffectName { get; set; }
    }
}
