using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace RP_Server_Scripts.Database.Account
{
    [DebuggerDisplay("Account:{AccountId}:{UserName}")]
    [Table("Accounts")]
    public sealed class AccountEntity
    {
        [Key]
        public int AccountId { get; set; }

        [Index(IsUnique = true)]
        [StringLength(450)]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public DateTime LastLogin { get; set; }

        [Required]
        public  DateTime CreationTime { get; set; }

        public string LastIpAddress { get; set; }

        public  bool IsBanned { get; set; }

        public  string BannedReasonText { get; set; }
    }
}
