using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clone.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(120, ErrorMessage = "Máximo 120 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Dirección")]
		[MaxLength(120, ErrorMessage = "Máximo 120 caracteres")]
		public string? StreetAddress { get; set; }

        [Display(Name = "Ciudad")]
		[MaxLength(120, ErrorMessage = "Máximo 120 caracteres")]
		public string? City { get; set; }

        [Display(Name = "Provincia")]
		[MaxLength(60, ErrorMessage = "Máximo 60 caracteres")]
		public string? State { get; set; }

        [Display(Name = "Código Postal")]
		[MaxLength(60, ErrorMessage = "Máximo 60 caracteres")]
		public string? PostalCode { get; set; }

        [Display(Name = "Compañia")]
        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
    }
}
