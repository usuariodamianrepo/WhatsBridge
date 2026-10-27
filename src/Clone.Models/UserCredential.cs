using Clone.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clone.Models
{
    public sealed class UserCredential : AuditableEntity
    {
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } = default!;

        [MaxLength(64)]
        public string HashToken { get; set; } = string.Empty;
        public DateTime Expired { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = CredentialStatus.pending.GetDisplayName();

    }
}
