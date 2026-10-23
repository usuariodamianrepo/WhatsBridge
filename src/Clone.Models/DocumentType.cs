using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Clone.Models
{
    public class DocumentType : AuditableEntity
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Description")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [MaxLength(450)]
        public string Description { get; set; } = string.Empty;
    }
}