using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RP_Server_Scripts.Database.Character
{
    public sealed class CharacterItemEntity
    {
        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        [StringLength(450)]
        public string ItemName { get; set; }
    }
}
