using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RP_Server_Scripts.Database.Character
{
    [Table("CharacterStat")]
    public class CharacterStatEntity
    {
        [Key]
        public int CharacterStatId { get; set; }

        [Index(IsUnique = true)]
        [StringLength(450)]
        public string StatName { get; set; }


        [Range(0, int.MaxValue)]
        public int MinValue { get; set; }

        [Range(0,int.MaxValue)]
        public int MaxValue { get; set; }
    }
}
