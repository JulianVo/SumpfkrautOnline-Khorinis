using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RP_Shared_Script;

namespace RP_Server_Scripts.Database.Character
{
    [Table("CharacterCustomVisual")]
    public  class CharacterCustomVisualsEntity
    {
        [Key]
        public int CharacterCustomVisualId { get; set; }


     
        public virtual CharacterEntity OwnerCharacter { get; set; }


        [ForeignKey("OwnerCharacter")]
        public virtual int OwnerCharacterId { get; set; }

        [Required]
        public HumBodyMeshs BodyMesh { get; set; }


        [Required]
        public HumBodyTexs BodyTex { get; set; }


        [Required]
        public HumHeadMeshs HeadMesh { get; set; }


        [Required]
        public HumHeadTexs HeadTex { get; set; }


        [Required]
        public HumVoices Voice { get; set; }

        [Range(0.9F, 1.1F)]
        [Required]
        public float BodyWidth { get; set; }

        [Range(-1F, 1F)]
        [Required]
        public float Fatness { get; set; }
    }
}
