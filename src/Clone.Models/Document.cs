using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clone.Models
{
    public class Document : AuditableEntity
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [DisplayName("Published")]
        [Column(TypeName = "timestamp without time zone")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public DateTime Published { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [MaxLength(450)]
        public string Description { get; set; } = string.Empty;

        [DisplayName("Public")]
        public bool IsPublic { get; set; } = false;

        [DisplayName("URL")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [MaxLength(250)]
        public string URL { get; set; } = string.Empty;

        [DisplayName("Document Type")]
        [ForeignKey("DocumentTypeId")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public int DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; } = default!;
    }
}